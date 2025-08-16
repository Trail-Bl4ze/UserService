using UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserService.Domain;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<Relationship> UserActivities => Set<Relationship>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<User> Users => Set<User>();
}