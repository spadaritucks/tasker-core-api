namespace Tasker.Application.Usecases.GoogleCalendarStatusUsecase;

public interface IGoogleCalendarStatusUsecase
{
    Task<bool> Execute(Guid userId, CancellationToken cancellationToken);
}