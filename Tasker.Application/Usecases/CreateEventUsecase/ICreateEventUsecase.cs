using Tasker.Application.DTOs.Events;

namespace Tasker.Application.Usecases.CreateEventUsecase;

public interface ICreateEventUsecase
{
    Task Execute(CreateEventRequest createEventRequest, Guid userId, CancellationToken cancellationToken);
}