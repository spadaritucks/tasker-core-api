using Microsoft.EntityFrameworkCore;
using Tasker.Domain.Entities;
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

    public async Task AddAsync(
        Domain.Entities.Events events,
        CancellationToken cancellationToken)
    {
        await _context.Events.AddAsync(events, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Events?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Domain.Entities.Events>> GetAllEventsByUserIdAsync(
        Guid userId,
        string? title = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        title = title?.Trim();

        return await _context.Events
            .AsNoTracking()
            .Where(eventEntity =>
                eventEntity.UserId == userId &&
                (title == null || title == string.Empty || eventEntity.Title.Contains(title)) &&
                (!startDate.HasValue || eventEntity.DateEnd >= startDate.Value) &&
                (!endDate.HasValue || eventEntity.DateStart <= endDate.Value))
            .OrderBy(eventEntity => eventEntity.DateStart)
            .ThenBy(eventEntity => eventEntity.Title)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateSyncStatusAsync(
        int syncStatus,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
