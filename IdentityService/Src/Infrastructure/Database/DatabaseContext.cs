using IdentityService.Modules.Identity.Mapping;
using IdentityService.Modules.Identity.Models;
using IdentityService.Modules.Identity.Models.Role;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Database;

internal sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }
    
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}