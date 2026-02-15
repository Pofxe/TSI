using Microsoft.EntityFrameworkCore;
using TransportCompanyIS.Models;

namespace TransportCompanyIS.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=transport.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>()
            .HasOne(shipment => shipment.AssignedVehicle)
            .WithMany()
            .HasForeignKey(shipment => shipment.AssignedVehicleId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Trip>()
            .HasOne(trip => trip.Shipment)
            .WithMany()
            .HasForeignKey(trip => trip.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trip>()
            .HasOne(trip => trip.Vehicle)
            .WithMany()
            .HasForeignKey(trip => trip.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Trip>()
            .HasOne(trip => trip.Driver)
            .WithMany()
            .HasForeignKey(trip => trip.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
