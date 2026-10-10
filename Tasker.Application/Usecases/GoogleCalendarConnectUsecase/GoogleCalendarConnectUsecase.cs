using System.Net.Http.Json;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Tasker.Application.DTOs.Auth;
using Tasker.Application.Exceptions;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.GoogleCalendarConnectUsecase;

public class GoogleCalendarConnectUsecase : IGoogleCalendarConnectUsecase
{
    private readonly IUserRepository _userRepository;
    private readonly IGoogleCalendarSessionsRepository _googleCalendarSessionsRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GoogleCalendarConnectUsecase(
        IUserRepository userRepository,
        IGoogleCalendarSessionsRepository googleCalendarSessionsRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _googleCalendarSessionsRepository = googleCalendarSessionsRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<bool> Execute(string code, Guid userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BadRequestValidationException(["O authorization code não pode ser vazio."]);
        }

        var user = await _userRepository.GetById(userId, cancellationToken)
                   ?? throw new NotFoundException("Usuário não existe.");

        var clientId = _configuration["Authentication:Google:ClientId"];
        var clientSecret = _configuration["Authentication:Google:ClientSecret"];
        var redirectUri = _configuration["Authentication:Google:RedirectUri"]
                          ?? "http://localhost:5173";

        if (string.IsNullOrWhiteSpace(clientId)
            || string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new InfrastructureException(
                "As credenciais OAuth do Google não foram configuradas.");
        }

        var httpClient = _httpClientFactory.CreateClient();
        using var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["redirect_uri"] = redirectUri,
            ["grant_type"] = "authorization_code"
        });

        using var tokenResponse = await httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            tokenRequest,
            cancellationToken);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var googleError = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new AuthException(
                $"Não foi possível conectar o Google Calendar. Resposta do Google: {googleError}");
        }

        var tokens = await tokenResponse.Content.ReadFromJsonAsync<GoogleCalendarConnectResponse>(
            cancellationToken: cancellationToken)
            ?? throw new AuthException("O Google retornou uma resposta OAuth inválida.");

        if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
        {
            throw new AuthException(
                "O Google não retornou refresh_token. Revogue o acesso do Tasker na sua conta Google e tente conectar novamente.");
        }

        if (string.IsNullOrWhiteSpace(tokens.IdToken))
        {
            throw new AuthException("O Google não retornou o id_token da autorização.");
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                tokens.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [clientId]
                });
        }
        catch (Exception ex) when (ex is InvalidJwtException or InvalidOperationException)
        {
            throw new AuthException("O id_token retornado pelo Google é inválido.");
        }

        if (!string.Equals(user.Email, payload.Email, StringComparison.OrdinalIgnoreCase))
        {
            throw new AuthException(
                "A conta Google autorizada não pertence ao usuário autenticado no Tasker.");
        }

        await _googleCalendarSessionsRepository.AddAsync(
            new GoogleCalendarSessions
            {
                UserId = user.Id,
                GoogleSubject = payload.Subject,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                Scope = tokens.Scope,
                AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn),
                CreatedAt = DateTime.UtcNow
            },
            cancellationToken);

        return true;
    }
}
