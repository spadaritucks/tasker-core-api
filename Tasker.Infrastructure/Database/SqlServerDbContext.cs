using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tasker.Domain.Entities;

namespace Tasker.Infrastructure.Database;

public class SqlServerDbContext : DbContext
{
    
    
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)                                                                                                             
        : base(options)                                                                                                                                                           
    {                                                                                                                                                                             
    }       
    
    public DbSet<User> User { get; set; }
    public DbSet<Sessions> Sessions { get; set; }
    
    public DbSet<Events> Events { get; set; }
    public DbSet<GoogleCalendarSessions> GoogleCalendarSessions { get; set; }

    
}