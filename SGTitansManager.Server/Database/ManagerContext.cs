using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGTitansManager.Models;
using SGTitansManager.Server.Services;

namespace SGTitansManager.Server.Database;

public class ManagerContext : DbContext
{
    public ManagerContext(DbContextOptions<ManagerContext> options) : base(options){}
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Champion> Champions { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<AppUser> Users { get; set; }
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
                v => string.Join(',', v.Select(e => e.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Enum.Parse<Position>(s, ignoreCase: true))
                    .ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<Position>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (hash, e) => HashCode.Combine(hash, e)),
                c => c.ToList()));
        
        modelBuilder.Entity<Champion>()
            .Property(p => p.Tags)
            .HasConversion(
                v => string.Join(',', v.Select(e => e.ToString())),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Enum.Parse<Tag>(s, ignoreCase: true))
                    .ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<Tag>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (hash, e) => HashCode.Combine(hash, e)),
                c => c.ToList()));
        
        // 1:1 Beziehungen
        
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Player)
            .WithOne()
            .HasForeignKey<Member>(m => m.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Member)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ChampionPoolItem>()
            .HasOne(c  => c.Champion)
            .WithOne()
            .HasForeignKey<Champion>(c => c.Id)
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

        modelBuilder.Entity<ChampionPoolItem>()
            .HasOne<Player>()
            .WithMany(p => p.ChampionPool)
            .HasForeignKey(c => c.PlayerId);
        
        // Set Primary Key

        modelBuilder.Entity<ChampionPoolItem>(e =>
        {
            e.HasKey(c => new {c.ChampionId, c.PlayerId});
            e.HasOne(c => c.Champion)
                .WithMany()
                .HasForeignKey(c => c.ChampionId);
        });
        
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
        
        modelBuilder.Entity<Player>(e =>
        {
            e.Property(a => a.Created)
                .HasConversion<UtcToLocalDateTimeConverter>();
            e.Property(a => a.Deleted)
                .HasConversion<UtcToLocalDateTimeConverter>();
        });
        
        modelBuilder.Entity<AppUser>(e =>
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
        
        modelBuilder.Entity<AppUser>()
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