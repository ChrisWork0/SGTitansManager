using Microsoft.EntityFrameworkCore;
using SGTitansManager.Models;

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
    }
}