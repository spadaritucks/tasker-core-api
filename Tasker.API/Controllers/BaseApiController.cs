using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Factories;

namespace Tasker.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<GlobalApiResponseBody<T>> Success<T>(T data)
    {
        var response = ApiResponseFactory.Success(
            data,
            HttpContext.Request.Path.ToString());

        return Ok(response);
    }
}
