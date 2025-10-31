using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;

namespace PurchasingService.Services;

public class SupplierService : ISupplierService
{
    private readonly PurchasingDbContext _dbContext;

    public SupplierService(PurchasingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        var suppliers = await _dbContext.Suppliers
            .Include(s => s.Products)
            .ToListAsync();
        foreach (var s in suppliers)
        {
            s.AvailableProducts = s.Products.Select(p => p.Name).ToList();
        }
        return suppliers;
    }

    public async Task<List<Supplier>> GetSupplierByIdAsync(int id)
    {
        var supplier = await _dbContext.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.SupplierId == id);
        if (supplier != null)
        {
            supplier.AvailableProducts = supplier.Products.Select(p => p.Name).ToList();
            return new List<Supplier> { supplier };
        }
        return new List<Supplier>();
    }

    public async Task<List<Supplier>> GetSupplierByNameAsync(string name)
    {
        var supplier = await _dbContext.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.CompanyName == name);
        if (supplier != null)
        {
            supplier.AvailableProducts = supplier.Products.Select(p => p.Name).ToList();
            return new List<Supplier> { supplier };
        }
        return new List<Supplier>();
    }

    public async Task<List<Supplier>> GetSuppliersForProductAsync(string product)
    {
        var suppliers = await _dbContext.Suppliers
            .Include(s => s.Products)
            .Where(s => s.Products.Any(p => p.Name == product))
            .ToListAsync();
        foreach (var s in suppliers)
        {
            s.AvailableProducts = s.Products.Select(p => p.Name).ToList();
        }
        return suppliers;
    }
}
