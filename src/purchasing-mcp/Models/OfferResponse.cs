namespace PurchasingService.Models;

public class OfferResponse
{
    public Guid OfferId { get; set; } = Guid.NewGuid();

    public int SupplierId { get; set; }

    public decimal TransportationCost { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public IReadOnlyList<OfferDetails> RequestDetails { get; set; } = Array.Empty<OfferDetails>();

    public string? Email { get; set; }
}
