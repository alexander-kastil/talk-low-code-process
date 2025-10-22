using PurchasingService.Models;

namespace PurchasingService.Services;

public interface IOrderService
{
    Task<object> PlaceOrderAsync(Order order);
    Task<IEnumerable<object>> GetOrdersAsync();
}
