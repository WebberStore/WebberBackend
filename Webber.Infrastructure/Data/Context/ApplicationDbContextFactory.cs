using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Webber.Infrastructure.Data.Context;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<WebberDbContext>
{
    /// <summary>
    /// Creates a new instance of the ApplicationDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the factory.</param>
    /// <returns>
    /// A new instance of the ApplicationDbContext configured with the connection string from 'appsettings.json'.
    /// </returns>
    public WebberDbContext CreateDbContext(string[] args)
    {
        // Configure the builder to use the 'appsettings.json' file
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        // Get the connection string from the configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Create and return the DbContext instance
        var builder = new DbContextOptionsBuilder<WebberDbContext>();
        builder.UseSqlServer(connectionString);
        return new WebberDbContext(builder.Options);
    }
}