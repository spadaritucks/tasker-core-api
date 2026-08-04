using Tasker.Domain.Entities;

namespace Tasker.Domain.Repositories;

public interface IUserRepository
{ 
   Task AddAsync(User user, CancellationToken cancellationToken = default);
   Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}