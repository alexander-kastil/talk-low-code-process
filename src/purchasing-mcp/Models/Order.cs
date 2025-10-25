using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PurchasingService.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? RequestId { get; set; }

    [Range(1, int.MaxValue)]
    public int SupplierId { get; set; }

    [MinLength(1)]
    public List<OrderDetail> OrderDetails { get; set; } = new();

    [Required]
    public string OfferId { get; set; } = string.Empty;

    public decimal? TransportationCost { get; set; }

}
