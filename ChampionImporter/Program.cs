using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGTitansManager.Server.Database;

namespace ChampionImporter;

class Program
{
    static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = config.GetConnectionString("DefaultConnectionString") 
                               ?? throw new ArgumentNullException();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddDbContext<ManagerContext>(o =>
        {
            o.UseNpgsql(connectionString);
        });

        services.AddScoped<ImportService>();
        
        await using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var importService = scope.ServiceProvider.GetRequiredService<ImportService>();
        await importService.StartImport();
    }
}