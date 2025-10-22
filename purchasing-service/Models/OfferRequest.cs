using System.ComponentModel.DataAnnotations;

namespace PurchasingService.Models;

public class OfferRequest
{
    [Required]
    public int SupplierId { get; set; }

    [Required]
    public string RequestId { get; set; } = string.Empty;

    [Required]
    public List<OfferRequestDetail> OfferDetails { get; set; } = new();

    [EmailAddress]
    public string? Email { get; set; }
}
