using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;
using PurchasingService.Models;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly PurchasingDbContext _context;

    public OrderController(PurchasingDbContext context)
    {
        _context = context;
    }

    [HttpPost("placeOrder")]
    public async Task<ActionResult<object>> PlaceOrder([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var supplier = await _context.Suppliers.FindAsync(order.SupplierId);

        if (supplier is null)
        {
            return NotFound($"Supplier with id {order.SupplierId} was not found.");
        }

        var total = order.OrderDetails.Sum(detail => detail.Price * detail.Quantity);

        // Persist the order to the database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var response = new
        {
            Message = "Order placed successfully.",
            order.RequestId,
            order.SupplierId,
            order.Date,
            Total = total
        };

        return Ok(response);
    }

    [HttpGet("getOrders")]
    public async Task<ActionResult<IEnumerable<object>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Supplier)
            .Include(o => o.OrderDetails)
            .Select(o => new
            {
                o.Id,
                o.RequestId,
                o.Date,
                OrderTotal = o.OrderDetails.Sum(od => od.Price * od.Quantity),
                SupplierName = o.Supplier != null ? o.Supplier.CompanyName : string.Empty
            })
            .ToListAsync();

        return Ok(orders);
    }
}
