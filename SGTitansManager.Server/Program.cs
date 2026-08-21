using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SGTitansManager.Models;
using SGTitansManager.Server.Database;
using SGTitansManager.Server.Services;

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
        var config = builder.Configuration;
        var connectionString = config.GetConnectionString("DefaultConnectionString");
        var botUrl = config.GetSection("DiscordBot").GetSection("BotUrl").Value ?? throw new ArgumentNullException();
        var internalApiKey = config.GetSection("DiscordBot").GetSection("ApiKey").Value ?? throw new ArgumentNullException();
        
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Please set connection string in appsettings.json");
        
        builder.Services.AddOpenApi();

        builder.Services.AddControllers().AddNewtonsoftJson(options => 
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        
        builder.Services.AddDbContext<ManagerContext>(options => 
            options.UseNpgsql(connectionString));

        builder.Services.AddScoped<AuthorizationService>();
        builder.Services.AddScoped<VerificationService>();
        
        builder.Services.AddHttpClient("PrometheusBot", client =>
        {
            client.BaseAddress = new Uri(botUrl);
            client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("PasswordChange", opt =>
            {
                opt.PermitLimit = 3;
                opt.Window = TimeSpan.FromMinutes(10);
            });
        });
        
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.AdminOnly, p =>
                p.RequireRole(nameof(Role.Admin)))
            .AddPolicy(Policies.CoachOnly, p =>
                p.RequireRole(nameof(Role.Admin), nameof(Role.Coach)))
            .AddPolicy(Policies.CoreTeam, p =>
                p.RequireRole(nameof(Role.Admin), nameof(Role.Coach),  nameof(Role.CorePlayer)))
            .AddPolicy(Policies.Organizer, p =>
                p.RequireRole(nameof(Role.Admin), nameof(Role.Caster), nameof(Role.Manager)));
        
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
            using (var scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<ManagerContext>().Database.Migrate();
        
        app.Run();
    }
}