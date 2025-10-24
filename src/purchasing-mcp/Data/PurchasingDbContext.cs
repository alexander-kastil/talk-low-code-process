using Microsoft.EntityFrameworkCore;
using PurchasingService.Models;

namespace PurchasingService.Data;

public class PurchasingDbContext : DbContext
{
    public PurchasingDbContext(DbContextOptions<PurchasingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<OfferDetail> OfferDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransportationCost).HasColumnType("decimal(18,2)");
            
            entity.HasMany(e => e.OfferDetails)
                .WithOne(d => d.Offer)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OfferDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OfferedPrice).HasColumnType("decimal(18,2)");
        });
    }
}
