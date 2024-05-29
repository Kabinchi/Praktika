using Microsoft.EntityFrameworkCore;
using Airport.Models;

public class MyDbContext : DbContext
{
    public DbSet<Airplane> Airplanes { get; set; }
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<CrewMember> CrewMembers { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=DESKTOP-A928918\SQLEXPRESS;Database=Airport1;Trusted_Connection=True;Encrypt=False");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airplane>().ToTable("Airplane");
        modelBuilder.Entity<Airplane>()
            .HasOne(a => a.Airline)
            .WithMany(al => al.Airplanes)
            .HasForeignKey(a => a.Airline_ID);

        modelBuilder.Entity<Flight>().ToTable("Flights");
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Airline)
            .WithMany(al => al.Flights)
            .HasForeignKey(f => f.Airline_ID);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Airplane)
            .WithMany(ap => ap.Flights)
            .HasForeignKey(f => f.Airplane_ID);

        modelBuilder.Entity<CrewMember>().ToTable("CrewMembers");
        modelBuilder.Entity<CrewMember>()
            .HasOne(cm => cm.Airplane)
            .WithMany(ap => ap.CrewMembers)
            .HasForeignKey(cm => cm.Airplane_ID);

        modelBuilder.Entity<Passenger>().ToTable("Passengers");
        modelBuilder.Entity<Passenger>()
            .HasOne(p => p.Airplane)
            .WithMany(a => a.Passengers)
            .HasForeignKey(p => p.Airplane_ID);

        modelBuilder.Entity<Ticket>().ToTable("Tickets");
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Passenger)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.Passenger_ID);

        base.OnModelCreating(modelBuilder);
    }
}
