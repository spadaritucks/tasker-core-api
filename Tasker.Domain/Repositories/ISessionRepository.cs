using Tasker.Domain.Entities;

namespace Tasker.Domain.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Sessions sessions, CancellationToken cancellationToken = default);
    Task<Sessions?> GetActiveByAccessTokenAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}
