using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using Google.Apis.Auth;
using Google.Apis.Logging;
using Google.Apis.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Tasker.Application.Exceptions;
using Tasker.Application.DTOs.Auth;
using Tasker.Application.Services;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;
using JsonWebToken = Microsoft.IdentityModel.JsonWebTokens.JsonWebToken;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;


namespace Tasker.Application.Usecases.GoogleClientUsecase
{
    public class GoogleClientUsecase : IGoogleClientUsecase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;
        private readonly ILogger<GoogleClientUsecase> _logger;
        private readonly ISessionRepository _sessionRepository;

        public GoogleClientUsecase(
            IUserRepository userRepository,
            ILogger<GoogleClientUsecase> logger,
            JwtTokenService jwtTokenService,
            ISessionRepository sessionRepository 
        )
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _sessionRepository = sessionRepository;    
        }

        public async Task<OAuth2GoogleValidatorResponse> Execute(
            string idToken, 
            string clientId, 
            string deviceId,
            string deviceInfo,
            CancellationToken cancellationToken
            )
        {
            try
            {
                if (string.IsNullOrEmpty(idToken))
                {
                    _logger.LogError("idToken: " + idToken +
                                     " O IdToken não esta presente! Lançando BadRequestValidationException");
                    throw new BadRequestValidationException(["O IdToken não pode ser nulo ou vazio "]);
                }

                var config = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, config);
                _logger.LogInformation("Validando assinatura do token do google");

                var user = await _userRepository.GetByEmailAsync(payload.Email);
                _logger.LogInformation("Assinatura validada! Verificando se o usuario " + payload.Name + " existe");

                if (user is null)
                {
                    _logger.LogInformation("Usuario não existe! Efetuando o cadastro de um novo usuario");
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = payload.Email,
                        Name = payload.Name,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user, cancellationToken);
                    _logger.LogInformation("Usuario " + user.Name + " cadastrado com sucesso");
                }

                _logger.LogInformation("Gerando o token JWT");
                var token = _jwtTokenService.CreateToken(user);
                _logger.LogInformation("Token JWT gerado com sucesso");

                _logger.LogInformation("Preparando a sessão do usuario");
                var session = new Sessions
                {
                    Id = Guid.NewGuid(),
                    AccessToken = token.AccessToken,
                    ExpiresIn = token.ExpiresIn.ToString(),
                    UserId = user.Id,
                    DeviceId = deviceId,
                    DeviceInfo = deviceInfo,
                    Revoke = false
                };
                
                await _sessionRepository.AddAsync(session, cancellationToken);  
                
                return new OAuth2GoogleValidatorResponse
                {
                    AccessToken = token.AccessToken,
                    TokenType = token.TokenType,
                    ExpiresIn = token.ExpiresIn,
                };
            }
            catch (InvalidJwtException ex)
            {
                throw new AuthException("Token Invalido");
            }
        }
    }
}