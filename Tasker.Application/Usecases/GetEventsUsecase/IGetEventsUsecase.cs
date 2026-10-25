using Tasker.Application.DTOs;
using Tasker.Domain.Entities;

namespace Tasker.Application.Usecases.GetEventsUsecase;

public interface IGetEventsUsecase
{
    Task<List<Events>> Execute(Guid userId,EventsFilterRequest eventsFilterRequest,CancellationToken cancellationToken);
}