using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;
using PurchasingService.Models;

namespace PurchasingService.Services;

public class OrderService : IOrderService
{
    private readonly PurchasingDbContext _dbContext;

    public OrderService(PurchasingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<object> PlaceOrderAsync(Order order)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == order.SupplierId);

        if (supplier is null)
        {
            throw new InvalidOperationException($"Supplier with id {order.SupplierId} was not found.");
        }

        var total = order.OrderDetails.Sum(detail => detail.Price * detail.Quantity);

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
}
