using Microsoft.AspNetCore.Mvc;
using PurchasingService.Models;
using PurchasingService.Services;

namespace PurchasingService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("placeOrder")]
    public async Task<ActionResult<object>> PlaceOrder([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _orderService.PlaceOrderAsync(order);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("getOrders")]
    public async Task<ActionResult<IEnumerable<object>>> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }
}
