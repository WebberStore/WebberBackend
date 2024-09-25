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

        await builder.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}