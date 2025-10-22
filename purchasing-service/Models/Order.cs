using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurchasingService.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string RequestId { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SupplierId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    // Navigation properties
    public Supplier? Supplier { get; set; }
    
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

