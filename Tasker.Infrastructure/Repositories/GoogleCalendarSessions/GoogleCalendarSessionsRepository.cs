using Microsoft.EntityFrameworkCore;
using Tasker.Domain.Repositories;
using Tasker.Infrastructure.Database;
using GoogleCalendarSessionEntity = Tasker.Domain.Entities.GoogleCalendarSessions;

namespace Tasker.Infrastructure.Repositories.GoogleCalendarSessions;

public class GoogleCalendarSessionsRepository : IGoogleCalendarSessionsRepository
{
    private readonly SqlServerDbContext _context;

    public GoogleCalendarSessionsRepository(SqlServerDbContext context)
    {
        _context = context;
    }

    public async Task<GoogleCalendarSessionEntity?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.GoogleCalendarSessions
            .Where(session => session.UserId == userId
                              && session.RefreshToken != null
                              && session.RefreshToken != string.Empty)
            .OrderByDescending(session => session.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(
        GoogleCalendarSessionEntity session,
        CancellationToken cancellationToken = default)
    {
        session.Id = Guid.NewGuid();
        await _context.GoogleCalendarSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
