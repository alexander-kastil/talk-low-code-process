using System.ComponentModel.DataAnnotations;

namespace PurchasingService.Models;

public class OfferRequestDetail
{
    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RequestedAmount { get; set; }
}
