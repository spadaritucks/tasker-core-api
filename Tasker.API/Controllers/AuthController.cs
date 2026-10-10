using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasker.Application.DTOs.Auth;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Usecases.AuthMeUsecase;
using Tasker.Application.Usecases.GoogleCalendarConnectUsecase;
using Tasker.Application.Usecases.GoogleClientUsecase;

namespace Tasker.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IGoogleClientUsecase _googleClientUsecase;
    private readonly IGoogleCalendarConnectUsecase _googleCalendarConnectUsecase;
    private readonly IAuthMeUsecase _authMeUsecase;
    private readonly IConfiguration _configuration;

    public AuthController(
        IConfiguration configuration,
        IAuthMeUsecase authMeUsecase,
        IGoogleClientUsecase googleClientUsecase,
        IGoogleCalendarConnectUsecase googleCalendarConnectUsecase
        )
    {
        _configuration = configuration;
        _authMeUsecase = authMeUsecase;
        _googleClientUsecase = googleClientUsecase;
        _googleCalendarConnectUsecase = googleCalendarConnectUsecase;
    }

    [HttpPost("google")]
    public async Task<ActionResult<GlobalApiResponseBody<AuthValidationResponse>>> AuthGoogle(
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

        return Ok(new AuthResponse
        {
            Authenticated = true,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> AuthMe(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var accessToken = Request.Cookies["X-Tasker-Access-Token"];
        var expirationClaim = User.FindFirstValue("exp");

        if (string.IsNullOrWhiteSpace(accessToken)
            || !long.TryParse(expirationClaim, out var expirationUnixTime))
        {
            return Unauthorized();
        }

        var expiresAt = DateTimeOffset
            .FromUnixTimeSeconds(expirationUnixTime)
            .UtcDateTime;

        var response = await _authMeUsecase.AuthMe(
            userId,
            accessToken,
            expiresAt,
            cancellationToken);

        return Ok(response);
    }
    
    [HttpPost("google-calendar/connect")]
    [Authorize]
    public async Task<IActionResult> GoogleCalendarConnect(
        [FromBody] GoogleCalendarConnectRequest request,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");
        
        if (!Guid.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var result = await _googleCalendarConnectUsecase.Execute(
            request.Code,
            userId,
            cancellationToken);

        if (!result)
        {
            return Unauthorized();
        }
        
        return Ok(new
        {
            Connected = true,
        });

    }
    
}
