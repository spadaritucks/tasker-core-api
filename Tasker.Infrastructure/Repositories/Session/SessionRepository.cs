using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;
using Tasker.Infrastructure.Database;

namespace Tasker.Infrastructure.Repositories.Session;

public class SessionRepository : ISessionRepository
{
    private readonly SqlServerDbContext _context;
    private readonly ILogger<SessionRepository> _logger;

    public SessionRepository(
        SqlServerDbContext context,
        ILogger<SessionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(
        Sessions sessions,
        CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(sessions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Sessão do usuario {UserId} adicionada com sucesso",
            sessions.UserId);
    }

    public async Task<Sessions?> GetActiveByAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                session => session.AccessToken == accessToken && !session.Revoke,
                cancellationToken);
    }
}
