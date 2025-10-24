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
        return await _dbContext.Suppliers
            .Include(s => s.Products)
            .ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        return await _dbContext.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.SupplierId == id);
    }

    public async Task<Supplier?> GetSupplierByNameAsync(string name)
    {
        return await _dbContext.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.CompanyName == name);
    }

    public async Task<List<Supplier>> GetSuppliersForProductAsync(string product)
    {
        return await _dbContext.Suppliers
            .Include(s => s.Products)
            .Where(s => s.Products.Any(p => p.Name == product))
            .ToListAsync();
    }
}
