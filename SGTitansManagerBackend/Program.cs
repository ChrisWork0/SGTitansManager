using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SGTitansManagerBackend.Database;

namespace SGTitansManagerBackend;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebApplication(args);
    }

    private static void CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Please set connection string in appsettings.json");
        
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        builder.Services.AddControllers().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        
        builder.Services.AddDbContext<ManagerContext>(options => 
            options.UseNpgsql(connectionString));
        
        StartApplication(builder.Build());
    }

    private static void StartApplication(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
            using (var scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<ManagerContext>().Database.Migrate();
        
        app.Run();
    }
}