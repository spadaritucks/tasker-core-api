using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tasker.API.DTOs.Errors;

namespace Tasker.API.Middleware;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is TaskerException taskerException)
        {
            context.HttpContext.Response.StatusCode = GetStatusCode(taskerException);
            context.Result = new ObjectResult(new ErrorMessagesDTO(taskerException.GetErrors()));
        }
        else
        {
            ThrowUnknownError(context);
        }
    }

    private void ThrowUnknownError(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(new ErrorMessagesDTO("Erro Desconhecido"));
    }

    private static int GetStatusCode(TaskerException exception)
    {
        return exception switch
        {
                NotFoundException => StatusCodes.Status404NotFound,
                AuthException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                BadRequestValidationException => StatusCodes.Status400BadRequest,
                

            _ => StatusCodes.Status500InternalServerError
        };
    }
}