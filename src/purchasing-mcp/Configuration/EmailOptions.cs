namespace PurchasingService.Configuration;

public class EmailOptions
{
    public string OfferPromptyFile { get; set; } = "OfferEmailGeneration.prompty";
    public string OrderPromptyFile { get; set; } = "OrderEmailGeneration.prompty";
}