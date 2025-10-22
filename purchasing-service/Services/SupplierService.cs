using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;

namespace PurchasingService.Services;

public class SupplierService : ISupplierService
{
    private readonly PurchasingDbContext _context;

    public SupplierService(PurchasingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers
            .Include(s => s.SupplierProducts)
            .ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        return await _context.Suppliers
            .Include(s => s.SupplierProducts)
            .FirstOrDefaultAsync(s => s.SupplierId == id);
    }

    public async Task<Supplier?> GetSupplierByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return await _context.Suppliers
            .Include(s => s.SupplierProducts)
            .FirstOrDefaultAsync(s => EF.Functions.Like(s.CompanyName, name));
    }

    public async Task<List<Supplier>> GetSuppliersForProductAsync(string product)
    {
        return await _context.Suppliers
            .Include(s => s.SupplierProducts)
            .Where(s => s.SupplierProducts.Any(p => EF.Functions.Like(p.ProductName, product)))
            .ToListAsync();
    }
}
