using Webber.API.Utilities;
using Webber.Infrastructure.Data.Context;

namespace Webber.API;

/// <summary>
/// The main entry point for the application.
/// </summary>

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args).Build();
        
        
        // Seed data to the database for testing purposes, uncomment if needed
        // using (var serviceScope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        // {
        //     var context = serviceScope.ServiceProvider.GetService<WebberDbContext>();
        //     var seeder = new DataSeeder(context!);
        //     await seeder.SeedAsync();
        // }
        

        await builder.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}