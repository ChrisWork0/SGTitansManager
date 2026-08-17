using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGTitansManager.Server.Database;

namespace ChampionImporter;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
        
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