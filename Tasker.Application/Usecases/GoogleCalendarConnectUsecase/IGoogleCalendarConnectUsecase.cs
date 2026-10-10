using Tasker.Application.DTOs.Auth;

namespace Tasker.Application.Usecases.GoogleCalendarConnectUsecase;

public interface IGoogleCalendarConnectUsecase
{
    Task<bool> Execute(string code, Guid userId, CancellationToken cancellationToken);
}