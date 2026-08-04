using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs.Global;
using Tasker.Application.DTOs.User;
using Tasker.Application.Usecases.RegisterUserUsecase;

namespace Tasker.API.Controllers;

public class UserController : BaseApiController
{
    private readonly IRegisterUserUsecase _registerUserUsecase;

    public UserController(IRegisterUserUsecase registerUserUsecase)
    {
        _registerUserUsecase = registerUserUsecase;
    }

    [HttpPost("register")]
    public async Task<ActionResult<GlobalApiResponseBody<UserRegisterResponse>>> Register(
        [FromBody] UserRegisterRequest userRegisterRequest,
        CancellationToken cancellationToken)
    {
        var result = await _registerUserUsecase.Execute(
            userRegisterRequest,
            cancellationToken);

        return Success(result);
    }
}
