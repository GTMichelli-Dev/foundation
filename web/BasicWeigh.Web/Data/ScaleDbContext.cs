using BasicWeigh.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicWeigh.Web.Data;

public class ScaleDbContext : DbContext
{
    public ScaleDbContext(DbContextOptions<ScaleDbContext> options) : base(options) { }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Truck> Trucks => Set<Truck>();
    public DbSet<Commodity> Commodities => Set<Commodity>();
    public DbSet<AppSetup> AppSetup => Set<AppSetup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions");
            e.HasIndex(t => t.DateIn);
            e.HasIndex(t => t.Customer);
            e.HasIndex(t => t.Carrier);
        });

        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Carrier>().ToTable("Carriers");
        modelBuilder.Entity<Location>().ToTable("Locations");
        modelBuilder.Entity<Destination>().ToTable("Destinations");
        modelBuilder.Entity<Truck>().ToTable("Trucks");
        modelBuilder.Entity<Commodity>().ToTable("Commodities");
        modelBuilder.Entity<AppSetup>().ToTable("AppSetup");

        modelBuilder.Entity<AppSetup>().HasData(new AppSetup
        {
            Id = 1,
            TicketNumber = 1,
            Header1 = "Basic Weigh",
            Header2 = "",
            Header3 = "",
            Header4 = "",
            TicketsPerPage = 1
        });
    }
}
