using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Exceptions;

namespace Tasker.Application.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionFilter(
        ILogger<ExceptionFilter> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is TaskerException taskerException)
        {
            context.HttpContext.Response.StatusCode = GetStatusCode(taskerException);
            context.Result = new ObjectResult(new GlobalApiResponseBody<object?>
                {
                    Success = false,
                    Data = { },
                    Error = new GlobalErrorApiBody
                    {
                        Code = context.HttpContext.Response.StatusCode.ToString(),
                        Message = taskerException.GetErrors().Count == 1
                            ? taskerException.GetErrors()[0]
                            : "Ocorreu um ou mais erros de validação",
                        Details = taskerException.GetErrors().Count > 1
                            ? taskerException.GetErrors()
                            : []
                    },
                    Metadata = new MetaDataApiBody
                    {
                        Path = context.HttpContext.Request.Path.ToString(),
                        Timestamp = DateTime.UtcNow
                    }

                }
            );

        }
        else
        {
            ThrowUnknownError(context);
        }
    }

    private void ThrowUnknownError(ExceptionContext context)
    {
        _logger.LogError(
            context.Exception,
            "Erro não tratado em {Path}",
            context.HttpContext.Request.Path);

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new GlobalApiResponseBody<object?>
            {
                Success = false,
                Data = null,
                Error = new GlobalErrorApiBody
                {
                    Code = context.HttpContext.Response.StatusCode.ToString(),
                    Message = context.Exception.Message,
                    Details = [context.Exception.ToString()]
                },
                Metadata = new MetaDataApiBody
                {
                    Path = context.HttpContext.Request.Path.ToString(),
                    Timestamp = DateTime.UtcNow
                }
            }
        );
    }

    private static int GetStatusCode(TaskerException exception)
    {
        return exception switch
        {
                NotFoundException => StatusCodes.Status404NotFound,
                AuthException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                BadRequestValidationException => StatusCodes.Status400BadRequest,
                InfrastructureException => StatusCodes.Status503ServiceUnavailable,
                

            _ => StatusCodes.Status500InternalServerError
        };
    }
}
