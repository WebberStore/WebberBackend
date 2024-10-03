using Webber.API.Utilities;
using Webber.Application.Interfaces;
using Webber.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Webber.Infrastructure.Data.Context;
using Webber.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Webber.API.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// </summary>
/// <typeparam name="TStartup"></typeparam>

public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
        });
        
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContextOptions
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<WebberDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing
            services.AddDbContext<WebberDbContext>(options =>
            {
                options.UseInMemoryDatabase("WebberTestDb");
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            
            // Seed data
            services.AddScoped(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                services.AddSettings(configuration);
                var context = sp.GetRequiredService<WebberDbContext>();
                return new DataSeeder(context);
            });
        });
        
        

        builder.UseEnvironment("Testing");
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Seed database on host startup
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        seeder.SeedAsync().Wait();

        return host;
    }
}
