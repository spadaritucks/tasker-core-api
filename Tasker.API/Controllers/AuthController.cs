using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs.Auth;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Usecases.GoogleClientUsecase;

namespace Tasker.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IGoogleClientUsecase _googleClientUsecase;
    private readonly IConfiguration _configuration;

    public AuthController(
        IConfiguration configuration,
        IGoogleClientUsecase googleClientUsecase)
    {
        _configuration = configuration;
        _googleClientUsecase = googleClientUsecase;
    }

    [HttpPost("google")]
    public async Task<ActionResult<GlobalApiResponseBody<OAuth2GoogleValidatorResponse>>> AuthGoogle(
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

        return Success(result);
    }
}
