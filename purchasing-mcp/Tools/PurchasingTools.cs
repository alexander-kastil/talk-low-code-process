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
internal class PurchasingTools
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
    [Description("Retrieves the complete list of suppliers. Use this as the default supplier lookup when no filters are required.")]
    public async Task<SupplierCollection> GetSuppliers()
    {
        _logger.LogInformation("Fetching all suppliers");
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return new SupplierCollection { Suppliers = suppliers };
    }

    // [McpServerTool]
    // [Description("Retrieves a supplier by their unique ID")]
    // public async Task<Supplier?> GetSupplierById(
    //     [Description("The unique identifier of the supplier")] int supplierId)
    // {
    //     _logger.LogInformation("Fetching supplier with ID {SupplierId}", supplierId);
    //     return await _supplierService.GetSupplierByIdAsync(supplierId);
    // }

    // [McpServerTool]
    // [Description("Retrieves a supplier by their company name")]
    // public async Task<Supplier?> GetSupplierByName(
    //     [Description("The name of the supplier company")] string name)
    // {
    //     if (string.IsNullOrWhiteSpace(name))
    //     {
    //         throw new ArgumentException("Name must be provided.", nameof(name));
    //     }

    //     _logger.LogInformation("Fetching supplier with name {Name}", name);
    //     return await _supplierService.GetSupplierByNameAsync(name);
    // }

    // [McpServerTool]
    // [Description("Retrieves all suppliers that offer a specific product. Prefer GetSuppliers unless product-specific filtering is needed.")]
    // public async Task<SupplierCollection> GetSuppliersForProduct(
    //     [Description("The name of the product to search for")] string productName)
    // {
    //     if (string.IsNullOrWhiteSpace(productName))
    //     {
    //         throw new ArgumentException("Product name must be provided.", nameof(productName));
    //     }

    //     _logger.LogInformation("Fetching suppliers for product {ProductName}", productName);
    //     var suppliers = await _supplierService.GetSuppliersForProductAsync(productName);
    //     return new SupplierCollection { Suppliers = suppliers };
    // }

    [McpServerTool]
    [Description("Requests an offer from a supplier for specified products. Returns offer details including pricing and availability.")]
    public async Task<string> RequestOffer(
        [Description("The unique identifier of the supplier")] int supplierId,
        [Description("A unique identifier for this offer request")] string requestId,
        [Description("JSON array of products to request. Each item must have 'productName' (string) and 'requestedAmount' (integer). Example: [{\"productName\":\"Chai\",\"requestedAmount\":100}]")] string offerDetailsJson,
        [Description("Optional email address to send the offer to")] string? email = null)
    {
        _logger.LogInformation("Requesting offer for supplier {SupplierId}, request {RequestId}", supplierId, requestId);

        if (string.IsNullOrWhiteSpace(requestId))
        {
            throw new ArgumentException("Request ID must be provided.", nameof(requestId));
        }

        if (string.IsNullOrWhiteSpace(offerDetailsJson))
        {
            throw new ArgumentException("Offer details must be provided.", nameof(offerDetailsJson));
        }

        List<OfferRequestDetail> offerDetails;
        try
        {
            offerDetails = JsonSerializer.Deserialize<List<OfferRequestDetail>>(offerDetailsJson)
                ?? throw new ArgumentException("Failed to parse offer details JSON.", nameof(offerDetailsJson));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON format for offer details: {ex.Message}", nameof(offerDetailsJson));
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
        [Description("The date of the order in ISO 8601 format (e.g., '2024-01-15T10:30:00Z')")] string orderDate,
        [Description("JSON array of order items. Each item must have 'productName' (string), 'price' (decimal), and 'quantity' (integer). Example: [{\"productName\":\"Chai\",\"price\":18.50,\"quantity\":100}]")] string orderDetailsJson)
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

        if (!DateTime.TryParse(orderDate, out var parsedDate))
        {
            throw new ArgumentException("Order date must be a valid ISO 8601 date time string.", nameof(orderDate));
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
            Date = parsedDate,
            OrderDetails = orderDetails
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
}
