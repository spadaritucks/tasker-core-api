using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasker.Application.DTOs;
using Tasker.Application.DTOs.Events;
using Tasker.Application.DTOs.Global;
using Tasker.Application.Exceptions;
using Tasker.Application.Factories;
using Tasker.Application.Usecases.CreateEventUsecase;
using Tasker.Application.Usecases.GetEventsUsecase;
using Tasker.Domain.Entities;

namespace Tasker.API.Controllers;

public class EventController : BaseApiController
{
    private readonly ICreateEventUsecase _createEventUsecase;
    private readonly IGetEventsUsecase _getEventsUsecase;

    public EventController(ICreateEventUsecase createEventUsecase, IGetEventsUsecase getEventsUsecase)
    {
        _createEventUsecase = createEventUsecase;
        _getEventsUsecase = getEventsUsecase;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<ActionResult<GlobalApiResponseBody<object>>> Create(
        [FromBody] CreateEventRequest createEventRequest,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            throw new AuthException("Sessão do usuário inválida.");
        }

        await _createEventUsecase.Execute(createEventRequest, userId, cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponseFactory.Success<object>(
                new { Created = true },
                HttpContext.Request.Path));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<GlobalApiResponseBody<List<Events>>>> GetAllEvents(
        [FromQuery] EventsFilterRequest eventsFilterRequest,
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                      User.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            throw new AuthException("Sessão do usuário inválida.");
        }

       var events = await _getEventsUsecase.Execute(
            userId,
            eventsFilterRequest,
            cancellationToken);

       return Success(events);
    }
}