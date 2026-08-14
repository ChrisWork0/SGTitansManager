using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Database;

public class ManagerContext : DbContext
{
    public ManagerContext(DbContextOptions<ManagerContext> options) : base(options){}
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerRank> PlayerRanks { get; set; }
    public DbSet<History> Histories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        // Enums werden als String in Datenbank gespeichert
        // zur vereinfachten Lesbarkeit

        modelBuilder.Entity<Appointment>()
            .Property(a => a.AppointmentType)
            .HasConversion<string>();

        modelBuilder.Entity<PlayerRank>()
            .Property(pr => pr.RankType)
            .HasConversion<string>();

        modelBuilder.Entity<PlayerRank>()
            .Property(pr => pr.Rank)
            .HasConversion<string>();

        modelBuilder.Entity<Player>()
            .Property(p => p.MainPosition)
            .HasConversion<string>();

        modelBuilder.Entity<Player>()
            .Property(p => p.Positions)
            .HasConversion(
                v => v.Select(e => e.ToString()).ToArray(),
                v => v.Select(Enum.Parse<Position>).ToList());
        
        // 1:1 Beziehungen
        
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Player)
            .WithOne()
            .HasForeignKey<Member>(m => m.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<User>()
            .HasOne(u => u.Member)
            .WithOne()
            .HasForeignKey<User>(u => u.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 1:n Beziehungen

        modelBuilder.Entity<PlayerRank>()
            .HasOne(pr => pr.Player)
            .WithMany(p => p.PlayerRanks)
            .HasForeignKey(pr => pr.PlayerId);
        
        modelBuilder.Entity<Availability>()
            .HasOne<Player>()
            .WithMany(p => p.Availabilities)
            .HasForeignKey(a => a.PlayerId);
        
        // Convert DateTime

        modelBuilder.Entity<Availability>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<Appointment>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<History>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<Member>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<Player>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<User>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<PlayerRank>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        // Default Data
        
        modelBuilder.Entity<Member>()
            .HasData(new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                DiscordName = "Admin",
                MemberSince = new DateOnly(2026, 8, 14),
                Created = DateTime.MinValue
            });
        
        modelBuilder.Entity<User>()
            .HasData(new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UserName = "admin",
                PasswordHash = "admin".Sha256(),
                Role = Role.Admin,
                IsActive = true,
                MemberId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Created = DateTime.MinValue
            });
    }
}