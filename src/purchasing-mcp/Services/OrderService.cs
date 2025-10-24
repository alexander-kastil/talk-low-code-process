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

        // If OfferId is provided, validate against the offer
        if (!string.IsNullOrWhiteSpace(order.OfferId))
        {
            if (!Guid.TryParse(order.OfferId, out var offerGuid))
            {
                throw new InvalidOperationException("Invalid OfferId format.");
            }

            var offer = await _dbContext.Offers
                .Include(o => o.OfferDetails)
                .FirstOrDefaultAsync(o => o.OfferId == offerGuid);

            if (offer is null)
            {
                throw new InvalidOperationException($"Offer with id {order.OfferId} was not found.");
            }

            // Validate that the offer matches the order request
            if (offer.SupplierId != order.SupplierId)
            {
                throw new InvalidOperationException("Offer supplier does not match the order supplier.");
            }

            // Validate order details against offer details
            foreach (var orderDetail in order.OrderDetails)
            {
                var offerDetail = offer.OfferDetails.FirstOrDefault(od => 
                    string.Equals(od.ProductName, orderDetail.ProductName, StringComparison.OrdinalIgnoreCase));

                if (offerDetail is null)
                {
                    throw new InvalidOperationException($"Product '{orderDetail.ProductName}' is not included in the offer.");
                }

                if (offerDetail.Quantity == 0)
                {
                    throw new InvalidOperationException($"Product '{orderDetail.ProductName}' is not available in the offer.");
                }

                if (orderDetail.Quantity > offerDetail.Quantity)
                {
                    throw new InvalidOperationException($"Requested quantity for '{orderDetail.ProductName}' exceeds the offered quantity.");
                }

                if (orderDetail.Price != offerDetail.Price)
                {
                    throw new InvalidOperationException($"Price for '{orderDetail.ProductName}' does not match the offer price.");
                }
            }

            // Update offer status to Accepted
            offer.Status = 1; // Accepted
            await _dbContext.SaveChangesAsync();

            var total = order.OrderDetails.Sum(detail => detail.Price * detail.Quantity) + offer.TransportationCost;

            var response = new
            {
                Message = "Order placed successfully.",
                order.RequestId,
                order.SupplierId,
                order.Date,
                OfferId = order.OfferId,
                TransportationCost = offer.TransportationCost,
                Total = total
            };

            return response;
        }
        else
        {
            // No offer validation - original behavior
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
}
