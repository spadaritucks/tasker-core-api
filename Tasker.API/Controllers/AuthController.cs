using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasker.Application.DTOs.Auth;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Exceptions;
using Tasker.Application.Usecases.AuthMeUsecase;
using Tasker.Application.Usecases.GoogleCalendarConnectUsecase;
using Tasker.Application.Usecases.GoogleCalendarStatusUsecase;
using Tasker.Application.Usecases.GoogleClientUsecase;

namespace Tasker.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IGoogleClientUsecase _googleClientUsecase;
    private readonly IGoogleCalendarConnectUsecase _googleCalendarConnectUsecase;
    private readonly IAuthMeUsecase _authMeUsecase;
    private readonly IConfiguration _configuration;
    private readonly IGoogleCalendarStatusUsecase  _googleCalendarStatusUsecase;

    public AuthController(
        IConfiguration configuration,
        IAuthMeUsecase authMeUsecase,
        IGoogleClientUsecase googleClientUsecase,
        IGoogleCalendarConnectUsecase googleCalendarConnectUsecase,
        IGoogleCalendarStatusUsecase googleCalendarStatusUsecase
        )
    {
        _configuration = configuration;
        _authMeUsecase = authMeUsecase;
        _googleClientUsecase = googleClientUsecase;
        _googleCalendarConnectUsecase = googleCalendarConnectUsecase;
        _googleCalendarStatusUsecase = googleCalendarStatusUsecase;
    }

    [HttpPost("google")]
    public async Task<ActionResult<GlobalApiResponseBody<AuthResponse>>> AuthGoogle(
        [FromBody] AuthGoogleRequest authGoogleRequest, CancellationToken cancellationToken)
    {
        var googleClientId = _configuration["Authentication:Google:ClientId"]
                             ?? throw new InvalidOperationException(
                                 "Google ClientId não foi configurado.");

        var result = await _googleClientUsecase.Execute(
            authGoogleRequest.IdToken,
            googleClientId,
            authGoogleRequest.DeviceId,
            authGoogleRequest.DeviceInfo,
            cancellationToken
        );

        Response.Cookies.Append(
                "X-Tasker-Access-Token", result.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    MaxAge = TimeSpan.FromSeconds(result.ExpiresIn)
                }
            );

        return Success(new AuthResponse
        {
            Authenticated = true,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GlobalApiResponseBody<AuthMeResponse>>> AuthMe(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            throw new AuthException("Sessão do usuário inválida.");
        }

        var accessToken = Request.Cookies["X-Tasker-Access-Token"];
        var expirationClaim = User.FindFirstValue("exp");

        if (string.IsNullOrWhiteSpace(accessToken)
            || !long.TryParse(expirationClaim, out var expirationUnixTime))
        {
            throw new AuthException("Sessão do usuário inválida ou expirada.");
        }

        var expiresAt = DateTimeOffset
            .FromUnixTimeSeconds(expirationUnixTime)
            .UtcDateTime;

        var response = await _authMeUsecase.AuthMe(
            userId,
            accessToken,
            expiresAt,
            cancellationToken);

        return Success(response);
    }
    
    [HttpPost("google-calendar/connect")]
    [Authorize]
    public async Task<ActionResult<GlobalApiResponseBody<GoogleCalendarStatusResponse>>> GoogleCalendarConnect(
        [FromBody] GoogleCalendarConnectRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
        
        if (!Guid.TryParse(subject, out var userId))
        {
            throw new AuthException("Sessão do usuário inválida.");
        }

        var result = await _googleCalendarConnectUsecase.Execute(
            request.Code,
            userId,
            cancellationToken);

        if (!result)
        {
            throw new AuthException("Não foi possível conectar o Google Calendar.");
        }
        
        return Success(new GoogleCalendarStatusResponse
        {
            Connected = true
        });

    }

    [HttpGet("google-calendar/status")]
    [Authorize]
    public async Task<ActionResult<GlobalApiResponseBody<GoogleCalendarStatusResponse>>> GoogleCalendarStatus(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
        
        if (!Guid.TryParse(subject, out var userId))
        {
            throw new AuthException("Sessão do usuário inválida.");
        }

        var result = await _googleCalendarStatusUsecase.Execute(userId, cancellationToken);


        return Success(new GoogleCalendarStatusResponse
        {
            Connected = result
        });
    }
    
}
