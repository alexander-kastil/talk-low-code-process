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
    }
}
