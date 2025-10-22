using Microsoft.EntityFrameworkCore;
using PurchasingService.Models;

namespace PurchasingService.Data;

public class PurchasingDbContext : DbContext
{
    public PurchasingDbContext(DbContextOptions<PurchasingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<SupplierProduct> SupplierProducts { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Supplier entity
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(s => s.SupplierId);
            entity.Property(s => s.CompanyName).IsRequired();
        });

        // Configure SupplierProduct entity
        modelBuilder.Entity<SupplierProduct>(entity =>
        {
            entity.HasKey(sp => sp.Id);
            entity.Property(sp => sp.ProductName).IsRequired();
            
            entity.HasOne(sp => sp.Supplier)
                .WithMany(s => s.SupplierProducts)
                .HasForeignKey(sp => sp.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.RequestId).IsRequired();
            
            entity.HasOne(o => o.Supplier)
                .WithMany()
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure OrderDetail entity
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(od => od.Id);
            entity.Property(od => od.ProductName).IsRequired();
            entity.Property(od => od.Price).HasPrecision(18, 2);
            
            entity.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var suppliers = new[]
        {
            new Supplier
            {
                SupplierId = 1,
                CompanyName = "Wiener Feinkost GmbH",
                ContactName = "Anna Schmid",
                ContactTitle = "Einkaufsleiterin",
                City = "Vienna",
                Region = "Wien",
                Country = "Austria",
                PostalCode = "1010",
                Phone = "+43 1 234 5678",
                EMail = "anna.schmid@wiener-feinkost.example",
                Address = "Graben 21",
                HomePage = "https://wiener-feinkost.example"
            },
            new Supplier
            {
                SupplierId = 2,
                CompanyName = "Muenchner Gewuerze GmbH",
                ContactName = "Juergen Mueller",
                ContactTitle = "Verkaufsleiter",
                City = "Muenchen",
                Region = "Bayern",
                Country = "Germany",
                PostalCode = "80331",
                Phone = "+49 89 123456",
                EMail = "info@muenchner-gewuerze.example",
                Address = "Marienplatz 1",
                HomePage = "https://muenchner-gewuerze.example"
            },
            new Supplier
            {
                SupplierId = 3,
                CompanyName = "Antica Cucina S.r.l.",
                ContactName = "Lucia Romano",
                ContactTitle = "Responsabile Acquisti",
                City = "Milano",
                Region = "Lombardia",
                Country = "Italy",
                PostalCode = "20121",
                Phone = "+39 02 3456789",
                EMail = "info@anticacucina.example",
                Address = "Via Dante 34",
                HomePage = "https://anticacucina.example"
            },
            new Supplier
            {
                SupplierId = 4,
                CompanyName = "Bangkok Foods Co., Ltd.",
                ContactName = "Somsak Chaiyawan",
                ContactTitle = "Operations Manager",
                City = "Bangkok",
                Region = string.Empty,
                Country = "Thailand",
                PostalCode = "10100",
                Phone = "+66 2 123 4567",
                EMail = "somsak.chaiyawan@bangkokfoods.example",
                Address = "Sukhumvit Rd. 45",
                HomePage = "https://bangkokfoods.example"
            }
        };

        modelBuilder.Entity<Supplier>().HasData(suppliers);

        var supplierProducts = new List<SupplierProduct>();
        var productId = 1;

        // Supplier 1 products
        var supplier1Products = new[]
        {
            "Butter Chicken", "Blini with Salmon", "Wiener Schnitzel",
            "Cevapcici", "Germknoedel", "Greek Salad", "Spare Ribs", "Falaffel with Humus."
        };
        foreach (var product in supplier1Products)
        {
            supplierProducts.Add(new SupplierProduct
            {
                Id = productId++,
                SupplierId = 1,
                ProductName = product
            });
        }

        // Supplier 2 products
        var supplier2Products = new[]
        {
            "Butter Chicken", "Blini with Salmon", "Wiener Schnitzel",
            "Cevapcici", "Germknoedel", "Greek Salad", "Spare Ribs", "Falaffel with Humus."
        };
        foreach (var product in supplier2Products)
        {
            supplierProducts.Add(new SupplierProduct
            {
                Id = productId++,
                SupplierId = 2,
                ProductName = product
            });
        }

        // Supplier 3 products
        var supplier3Products = new[]
        {
            "Butter Chicken", "Blini with Salmon", "Wiener Schnitzel",
            "Cevapcici", "Germknoedel", "Greek Salad", "Spare Ribs", "Falaffel with Humus."
        };
        foreach (var product in supplier3Products)
        {
            supplierProducts.Add(new SupplierProduct
            {
                Id = productId++,
                SupplierId = 3,
                ProductName = product
            });
        }

        // Supplier 4 products
        supplierProducts.Add(new SupplierProduct
        {
            Id = productId++,
            SupplierId = 4,
            ProductName = "Pad Ka Prao"
        });

        modelBuilder.Entity<SupplierProduct>().HasData(supplierProducts);
    }
}
