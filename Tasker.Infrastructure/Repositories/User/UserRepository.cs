using Microsoft.EntityFrameworkCore;
using Tasker.Domain.Entities;
using Tasker.Domain.Repositories;
using Tasker.Infrastructure.Database;

namespace Tasker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly SqlServerDbContext _context;
    public UserRepository(SqlServerDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetById(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.User.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user;
    }
    
    
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
         _context.User.Add(user);
         await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
       var user =  await _context.User.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
       return user;
    }
}