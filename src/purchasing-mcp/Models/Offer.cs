using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingService.Models;

public class Offer
{
    [Key]
    public Guid Id { get; set; }

    public int SupplierId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TransportationCost { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string? Email { get; set; }

    // Navigation property for offer details
    public List<OfferDetail> OfferDetails { get; set; } = new List<OfferDetail>();
}

public class OfferDetail
{
    [Key]
    public int Id { get; set; }

    public Guid OfferId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OfferedPrice { get; set; }

    public int RequestedQuantity { get; set; }

    public int OfferedQuantity { get; set; }

    public int DeliveryDurationDays { get; set; }

    public bool IsAvailable { get; set; }

    // Navigation property back to Offer
    [ForeignKey("OfferId")]
    public Offer Offer { get; set; } = null!;
}
