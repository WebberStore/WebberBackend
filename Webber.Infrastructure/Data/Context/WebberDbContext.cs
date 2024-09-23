using Webber.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Webber.Infrastructure.Data.Context;

/// <summary>
/// Represents the database context for the Webber application, responsible for managing entities and their relationships.
/// </summary>
public class WebberDbContext(DbContextOptions<WebberDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Configures the relationships between entities in the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Product - Seller (User) Relationship 
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Seller)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Order (One-to-Many)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // User - Address (One-to-One)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<User>(u => u.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

}