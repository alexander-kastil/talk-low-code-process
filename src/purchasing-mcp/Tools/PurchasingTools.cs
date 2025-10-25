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
    private readonly ISupplierService supplierService;
    private readonly IInquiryService inquiryService;
    private readonly IOrderService orderService;
    private readonly ILogger<PurchasingTools> logger;

    public PurchasingTools(
        ISupplierService supplierService,
        IInquiryService inquiryService,
        IOrderService orderService,
        ILogger<PurchasingTools> logger)
    {
        this.supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        this.inquiryService = inquiryService ?? throw new ArgumentNullException(nameof(inquiryService));
        this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [McpServerTool]
    [Description("Retrieves the complete list of suppliers. It includes the supplierId, companyName information what suppliers can deliver which products in the availableProducts array.")]
    public async Task<SupplierCollection> GetSuppliers()
    {
        logger.LogInformation("Fetching all suppliers");
        var suppliers = await supplierService.GetAllSuppliersAsync();
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
            var response = await inquiryService.RequestOfferAsync(request);
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
    [Description("Places an order with a supplier based on a validated offer. Requires a valid offer ID. If order details are provided, they will be validated against the offer. If not provided, the order will be placed using the offer's details without additional validation.")]
    public async Task<string> PlaceOrder(
        [Description("The unique identifier of the offer (offerId) to accept")] string offerId,
        [Description("The unique identifier of the supplier")] int supplierId,
        [Description("Optional: JSON array of order items. If provided, must match the offer details. If not provided, order will use the offer's details. Example: [{\"productName\":\"Chai\",\"price\":18.50,\"quantity\":100}]")] string? orderDetailsJson = null,
        [Description("Optional: A unique identifier for this order request.")] string? requestId = null)
    {
        logger.LogInformation("Placing order for supplier {SupplierId}, offer {OfferId}", supplierId, offerId);

        if (string.IsNullOrWhiteSpace(offerId))
        {
            throw new ArgumentException("Offer ID must be provided.", nameof(offerId));
        }

        List<OrderDetail> orderDetails;
        if (string.IsNullOrWhiteSpace(orderDetailsJson))
        {
            // Fetch offer to get details
            if (!Guid.TryParse(offerId, out var offerGuid))
            {
                throw new ArgumentException("Invalid OfferId format.", nameof(offerId));
            }

            var offer = await inquiryService.GetOfferByIdAsync(offerGuid);
            if (offer == null)
            {
                throw new ArgumentException($"Offer with id {offerId} was not found.", nameof(offerId));
            }

            orderDetails = offer.OfferDetails.Select(od => new OrderDetail
            {
                ProductName = od.ProductName,
                Price = od.Price,
                Quantity = od.Quantity
            }).ToList();
        }
        else
        {
            try
            {
                orderDetails = JsonSerializer.Deserialize<List<OrderDetail>>(orderDetailsJson)
                    ?? throw new ArgumentException("Failed to parse order details JSON.", nameof(orderDetailsJson));
            }
            catch (JsonException ex)
            {
                throw new ArgumentException($"Invalid JSON format for order details: {ex.Message}", nameof(orderDetailsJson));
            }
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
            var response = await orderService.PlaceOrderAsync(order);
            return JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (InvalidOperationException ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    [McpServerTool]
    [Description("Retrieves an offer by its unique identifier, including all offer details such as supplier ID, transportation cost, timestamp, status, email, and a list of offer details with product names, prices, requested quantities, available quantities, and delivery duration days. This information is essential for validating offers and preparing order placements.")]
    public async Task<string> GetOfferById(
        [Description("The unique identifier of the offer")] string offerId)
    {
        if (!Guid.TryParse(offerId, out var guid))
        {
            return "Error: Invalid GUID format.";
        }

        logger.LogInformation("Retrieving offer {OfferId}", offerId);

        try
        {
            var offer = await inquiryService.GetOfferByIdAsync(guid);
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
