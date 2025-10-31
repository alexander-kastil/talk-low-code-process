namespace PurchasingService.Services;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<List<Supplier>> GetSupplierByIdAsync(int id);
    Task<List<Supplier>> GetSupplierByNameAsync(string name);
    Task<List<Supplier>> GetSuppliersForProductAsync(string product);
}
