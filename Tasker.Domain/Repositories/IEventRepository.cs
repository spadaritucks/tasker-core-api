using Tasker.Domain.Entities;
using Tasker.Domain.Enums;

namespace Tasker.Domain.Repositories;

public interface IEventRepository
{
    Task AddAsync(Events events, CancellationToken cancellationToken);
    Task<Events?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<List<Events>> GetAllEventsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateSyncStatusAsync(int syncStatus, CancellationToken cancellationToken);
}