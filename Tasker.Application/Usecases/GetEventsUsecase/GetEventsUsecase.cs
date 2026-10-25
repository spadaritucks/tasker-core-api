using Tasker.Application.DTOs;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;

namespace Tasker.Application.Usecases.GetEventsUsecase;

public class GetEventsUsecase : IGetEventsUsecase
{
    private readonly IEventRepository _eventRepository;

    public GetEventsUsecase(
        IEventRepository eventRepository
    )
    {
        _eventRepository = eventRepository;
    }

    public async Task<List<Events>> Execute(Guid userId, EventsFilterRequest eventsFilterRequest,
        CancellationToken cancellationToken)
    {
        return await _eventRepository.GetAllEventsByUserIdAsync(
            userId,
            eventsFilterRequest.Title,
            eventsFilterRequest.StartDate,
            eventsFilterRequest.EndDate,
            cancellationToken);
    }
}