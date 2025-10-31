using PurchasingService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class Supplier
{
    public int SupplierId { get; set; }
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
    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public List<string> AvailableProducts { get; set; } = new();
}
