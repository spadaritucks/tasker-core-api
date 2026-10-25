using Tasker.Domain.Entities;

namespace Tasker.Domain.Repositories;

public interface IGoogleCalendarSessionsRepository
{
    Task<GoogleCalendarSessions?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        GoogleCalendarSessions session,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        GoogleCalendarSessions session,
        CancellationToken cancellationToken = default);
}
