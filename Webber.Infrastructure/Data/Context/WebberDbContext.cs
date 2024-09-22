using Webber.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Webber.Infrastructure.Data.Context;

/// <summary>
/// Represents the database context for the Webber application, responsible for managing entities and their relationships.
/// </summary>
public class WebberDbContext(DbContextOptions<WebberDbContext> options) : DbContext(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Configures the relationships between entities in the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
}