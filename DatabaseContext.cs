using Microsoft.EntityFrameworkCore;
using Stopwatch.Configurations;
using Stopwatch.Domain;

namespace Stopwatch;
public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<TimedSession> TimedSessions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}