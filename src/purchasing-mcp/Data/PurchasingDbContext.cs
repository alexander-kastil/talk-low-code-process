using Microsoft.EntityFrameworkCore;
using PurchasingService.Models;

namespace PurchasingService.Data;

public class PurchasingDbContext : DbContext
{
    public PurchasingDbContext(DbContextOptions<PurchasingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Offer> Offers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId);
            entity.Property(e => e.OfferId).ValueGeneratedNever();
            entity.Property(e => e.SupplierId).IsRequired();
            entity.Property(e => e.TransportationCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.OwnsMany(e => e.OfferDetails, od =>
            {
                od.Property(d => d.ProductName).IsRequired().HasMaxLength(255);
                od.Property(d => d.Price).HasColumnType("decimal(18,2)");
                od.Property(d => d.RequestedQuantity).IsRequired();
                od.Property(d => d.Quantity).IsRequired();
                od.Property(d => d.DeliveryDurationDays).IsRequired();
            });
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.SupplierId).ValueGeneratedNever();
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContactName).HasMaxLength(255);
            entity.Property(e => e.ContactTitle).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Region).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.EMail).HasMaxLength(255);
            entity.Property(e => e.HomePage).HasMaxLength(500);
            
            // Ignore the computed property
            entity.Ignore(e => e.AvailableProducts);
            
            // Configure the many-to-many relationship
            entity.HasMany(s => s.Products)
                .WithMany()
                .UsingEntity(j => j.ToTable("SupplierProducts"));
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Name).IsUnique();
        });
    }
}
