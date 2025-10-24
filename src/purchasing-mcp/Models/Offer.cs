namespace PurchasingService.Models;

public class Offer
{
    public Guid OfferId { get; set; } = Guid.NewGuid();

    public int SupplierId { get; set; }

    public decimal TransportationCost { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public List<OfferDetails> OfferDetails { get; set; } = new List<OfferDetails>();

    public string? Email { get; set; }

    public OfferStatus Status { get; set; } = OfferStatus.Pending;
}
