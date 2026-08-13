using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SGTitansManager.Server.Database;

namespace SGTitansManager.Server;

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
        
        builder.Services.AddOpenApi();

        builder.Services.AddControllers().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        
        builder.Services.AddDbContext<ManagerContext>(options => 
            options.UseNpgsql(connectionString));
        
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        builder.Configuration.GetSection("JwtSettings").GetSection("JwtSecret").Value
                        ?? throw new ArgumentNullException())),
                    ValidIssuer = builder.Configuration.GetSection("JwtSettings").GetSection("JwtIssuer").Value,
                    ValidateAudience = false
                };
            });
        
        StartApplication(builder.Build());
    }

    private static void StartApplication(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseAuthentication();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
            using (var scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<ManagerContext>().Database.Migrate();
        
        app.Run();
    }
}