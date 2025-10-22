using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PurchasingService.Models;

public class Supplier
{
    [Key]
    public int SupplierId { get; set; }
    
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string HomePage { get; set; } = string.Empty;
    
    // Navigation property for EF Core
    public ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
    
    // Not mapped - for compatibility with existing code
    [NotMapped]
    public IReadOnlyList<string> Products
    {
        get => SupplierProducts.Select(sp => sp.ProductName).ToList();
        set
        {
            SupplierProducts = value.Select(p => new SupplierProduct { ProductName = p }).ToList();
        }
    }
}
