using Microsoft.EntityFrameworkCore;
using Tasker.Domain.Repositories;
using Tasker.Infrastructure.Database;

namespace Tasker.Infrastructure.Repositories.Events;

public class EventRepository : IEventRepository
{

    private readonly SqlServerDbContext _context;
    
    public EventRepository(SqlServerDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Domain.Entities.Events events, CancellationToken cancellationToken)
    {
        await _context.Events.AddAsync(events,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Events?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Events.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Domain.Entities.Events>> GetAllEventsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.Events.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task UpdateSyncStatusAsync(int syncStatus, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}