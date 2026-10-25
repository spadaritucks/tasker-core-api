using Microsoft.Extensions.Logging;
using Tasker.Application.DTOs.Events;
using Tasker.Application.Exceptions;
using Tasker.Application.Services;
using Tasker.Domain.Entities;
using Tasker.Domain.Enums;
using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.CreateEventUsecase;

public class CreateEventUsecase : ICreateEventUsecase
{
    private readonly IEventRepository _eventRepository;
    private readonly GoogleCalendarAccessTokenService _googleCalendarAccessTokenService;
    private readonly GoogleCalendarService _googleCalendarService;
    private readonly ILogger<CreateEventUsecase> _logger;

    public CreateEventUsecase(
        IEventRepository eventRepository,
        GoogleCalendarAccessTokenService googleCalendarAccessTokenService,
        GoogleCalendarService googleCalendarService,
        ILogger<CreateEventUsecase> logger
        )
    {
        _googleCalendarAccessTokenService = googleCalendarAccessTokenService;
        _eventRepository = eventRepository;
        _googleCalendarService = googleCalendarService;
        _logger = logger;
    }

    public async Task Execute(CreateEventRequest createEventRequest, Guid userId, CancellationToken cancellationToken)
    {
        var calendarAccessToken = await _googleCalendarAccessTokenService
            .GetValidAccessTokenAsync(userId, cancellationToken);

        var googleCalendarPayload = new CreateGoogleCalendarEventRequest
        {
            Start = new EventStartAttributes {
                DateTime = createEventRequest.StartDate,
                TimeZone = "America/Sao_Paulo"
            },
            End =  new EventEndAttributes {
                DateTime = createEventRequest.EndDate,
                TimeZone = "America/Sao_Paulo"
            },
            Summary = createEventRequest.Title,
            Description = createEventRequest.Description ?? ""
        };
        

        var request = calendarAccessToken != null
            ? await _googleCalendarService.CreateEvent(
                calendarAccessToken,
                googleCalendarPayload,
                cancellationToken
            )
            : null;

        if (request == null)
        {
            _logger.LogWarning("Usuario " + userId + "não autenticado no google agenda");
        }

        var newEvent = new Events
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GoogleEventId = request != null ? request.Id :  "",
            Title = createEventRequest.Title,
            Description = createEventRequest.Description ?? "",
            DateStart = createEventRequest.StartDate,
            DateEnd = createEventRequest.EndDate,
            GoogleHtmlLink = request != null ? request.HtmlLink : "",
            SyncStatus = request is null
                ? SyncStatus.Created
                : string.IsNullOrWhiteSpace(request.Id)
                    ? SyncStatus.SyncFailed
                    : SyncStatus.Synced,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(newEvent, cancellationToken);
    }
}
