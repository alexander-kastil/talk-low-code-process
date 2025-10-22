using System.ComponentModel.DataAnnotations;

namespace PurchasingService.Models;

public class SupplierProduct
{
    [Key]
    public int Id { get; set; }
    
    public int SupplierId { get; set; }
    
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    // Navigation property
    public Supplier Supplier { get; set; } = null!;
}
