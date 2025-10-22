using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;
using PurchasingService.Models;

namespace PurchasingService.Services;

public class OrderService : IOrderService
{
    private readonly PurchasingDbContext _context;

    public OrderService(PurchasingDbContext context)
    {
        _context = context;
    }

    public async Task<object> PlaceOrderAsync(Order order)
    {
        var supplier = await _context.Suppliers.FindAsync(order.SupplierId);

        if (supplier is null)
        {
            throw new InvalidOperationException($"Supplier with id {order.SupplierId} was not found.");
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

        return response;
    }

    public async Task<IEnumerable<object>> GetOrdersAsync()
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

        return orders;
    }
}
