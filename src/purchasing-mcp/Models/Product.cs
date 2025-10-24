namespace PurchasingService.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
}
