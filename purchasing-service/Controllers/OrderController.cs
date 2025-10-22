using Microsoft.AspNetCore.Mvc;
using PurchasingService.Data;
using PurchasingService.Models;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    [HttpPost("placeOrder")]
    public ActionResult<object> PlaceOrder([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var supplier = SupplierStore.GetSupplierById(order.SupplierId);

        if (supplier is null)
        {
            return NotFound($"Supplier with id {order.SupplierId} was not found.");
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

        return Ok(response);
    }
}
