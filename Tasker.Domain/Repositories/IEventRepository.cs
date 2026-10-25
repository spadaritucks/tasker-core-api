using Tasker.Domain.Entities;
using Tasker.Domain.Enums;

namespace Tasker.Domain.Repositories;

public interface IEventRepository
{
    Task AddAsync(Events events, CancellationToken cancellationToken);
    Task<Events?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<List<Events>> GetAllEventsByUserIdAsync(
        Guid userId,
        string? title = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
    Task UpdateSyncStatusAsync(int syncStatus, CancellationToken cancellationToken);
}
