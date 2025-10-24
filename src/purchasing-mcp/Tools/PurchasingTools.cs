using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using PurchasingService.Models;
using PurchasingService.Services;

namespace PurchasingService.Tools;

/// <summary>
/// Provides purchasing management tools for the MCP server.
/// Enables AI agents to manage suppliers, request offers, and place orders.
/// </summary>
[McpServerToolType]
public class PurchasingTools
{
    private readonly ISupplierService _supplierService;
    private readonly IInquiryService _inquiryService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PurchasingTools> _logger;

    public PurchasingTools(
        ISupplierService supplierService,
        IInquiryService inquiryService,
        IOrderService orderService,
        ILogger<PurchasingTools> logger)
    {
        _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        _inquiryService = inquiryService ?? throw new ArgumentNullException(nameof(inquiryService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [McpServerTool]
    [Description("Retrieves the complete list of suppliers. It includes the supplierId, companyName information what suppliers can deliver which products in the availableProducts array.")]
    public async Task<SupplierCollection> GetSuppliers()
    {
        _logger.LogInformation("Fetching all suppliers");
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return new SupplierCollection { Suppliers = suppliers };
    }

    [McpServerTool]
    [Description("Requests an offer (inquiry) from a supplier for specified products. Returns offer details including pricing and availability.")]
    public async Task<string> RequestOffer(
        [Description("The unique identifier of the supplier: supplierId")] int supplierId,
        [Description("Array of products to request. Each item must have 'Product' (string) and 'RequestedQuantity' (integer).")] List<OfferRequestDetail> offerDetails,
        [Description("Optional email address to send the offer to")] string? email = null)
    {
        if (offerDetails is null || offerDetails.Count == 0)
        {
            throw new ArgumentException("Offer details must be provided.", nameof(offerDetails));
        }

        var request = new OfferRequest
        {
            SupplierId = supplierId,
            RequestDetails = offerDetails,
            Email = email
        };

        try
        {
            var response = await _inquiryService.RequestOfferAsync(request);
            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (InvalidOperationException ex)
        {
            return $"Error: {ex.Message}";
        }
        catch (ArgumentException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Places an order with a supplier for specified products. Returns order confirmation with total cost.")]
    public async Task<string> PlaceOrder(
        [Description("A unique identifier for this order request")] string requestId,
        [Description("The unique identifier of the supplier")] int supplierId,
        [Description("JSON array of order items. Each item must have 'productName' (string), 'price' (decimal), and 'quantity' (integer). Example: [{\"productName\":\"Chai\",\"price\":18.50,\"quantity\":100}]")] string orderDetailsJson,
        [Description("Optional: The unique identifier of the offer to validate against")] string? offerId = null)
    {
        _logger.LogInformation("Placing order for supplier {SupplierId}, request {RequestId}", supplierId, requestId);

        if (string.IsNullOrWhiteSpace(requestId))
        {
            throw new ArgumentException("Request ID must be provided.", nameof(requestId));
        }

        if (string.IsNullOrWhiteSpace(orderDetailsJson))
        {
            throw new ArgumentException("Order details must be provided.", nameof(orderDetailsJson));
        }

        List<OrderDetail> orderDetails;
        try
        {
            orderDetails = JsonSerializer.Deserialize<List<OrderDetail>>(orderDetailsJson)
                ?? throw new ArgumentException("Failed to parse order details JSON.", nameof(orderDetailsJson));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON format for order details: {ex.Message}", nameof(orderDetailsJson));
        }

        var order = new Order
        {
            RequestId = requestId,
            SupplierId = supplierId,
            OrderDetails = orderDetails,
            OfferId = offerId
        };

        try
        {
            var response = await _orderService.PlaceOrderAsync(order);
            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (InvalidOperationException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Retrieves an offer by its unique identifier, including all offer details.")]
    public async Task<string> GetOfferById(
        [Description("The unique identifier of the offer")] string offerId)
    {
        if (!Guid.TryParse(offerId, out var guid))
        {
            return "Error: Invalid GUID format.";
        }

        _logger.LogInformation("Retrieving offer {OfferId}", offerId);

        try
        {
            var offer = await _inquiryService.GetOfferByIdAsync(guid);
            if (offer == null)
            {
                return $"Error: Offer with ID {offerId} was not found.";
            }
            return JsonSerializer.Serialize(offer, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
