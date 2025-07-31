using UserService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserService.Domain;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<User> Users => Set<User>();
}