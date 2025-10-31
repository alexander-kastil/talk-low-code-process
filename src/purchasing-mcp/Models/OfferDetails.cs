namespace PurchasingService.Models;

public class OfferDetails
{
    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int RequestedQuantity { get; set; }

    public int Quantity { get; set; }

    public int DeliveryDurationDays { get; set; }
}
