using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PurchasingService.Data;
using PurchasingService.Graph;
using PurchasingService.Models;
using PurchasingService.Services;
using PurchasingService.Tools;
using System.Text.Json;
using Xunit;

namespace PurchasingService.Tests;

public class PurchasingToolsTests : IDisposable
{
    private readonly PurchasingDbContext _dbContext;
    private readonly ISupplierService _supplierService;
    private readonly IOfferRandomizer _offerRandomizer;
    private readonly IInquiryService _inquiryService;
    private readonly PurchasingTools _tools;

    public PurchasingToolsTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<PurchasingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new PurchasingDbContext(options);
        SeedTestData();

        // Setup services
        _supplierService = new SupplierService(_dbContext);
        
        var randomProvider = new SystemRandomProvider(42); // Fixed seed for deterministic tests
        var randomizerOptions = new OfferRandomizerOptions
        {
            TransportationCost = 30m,
            Pricing = new OfferRandomizerOptions.PricingOptions
            {
                BaseProbability = 1.0, // Always use base price for predictable tests
                DiscountProbability = 0.0,
                MarkupProbability = 0.0
            },
            Quantity = new OfferRandomizerOptions.QuantityOptions
            {
                FulfillProbability = 1.0, // Always fulfill for predictable tests
                ReducedProbability = 0.0,
                UnavailableProbability = 0.0
            },
            Delivery = new OfferRandomizerOptions.DeliveryOptions
            {
                SameDaySingleDeliveryPercentage = 100, // Always same day for predictable tests
                CommonProbability = 0.7,
                CommonDays = new[] { 2, 3 },
                AdditionalDaysBase = 4,
                AdditionalDaysRange = 4
            }
        };
        _offerRandomizer = new OfferRandomizer(randomProvider, randomizerOptions, _dbContext);

        // Create null dependencies for optional services in tests
        GraphHelper? graphHelper = null;
        Microsoft.Extensions.AI.IChatClient? chatClient = null;
        
        _inquiryService = new InquiryService(_offerRandomizer, graphHelper, chatClient, _dbContext);
        
        var orderService = new OrderService(_dbContext);
        
        // Mock logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<PurchasingTools>();
        
        _tools = new PurchasingTools(_supplierService, _inquiryService, orderService, logger);
    }

    private void SeedTestData()
    {
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Wiener Schnitzel", BasePrice = 14.00m },
            new() { ProductId = 2, Name = "Germknoedel", BasePrice = 7.00m },
            new() { ProductId = 3, Name = "Pizza Napoli", BasePrice = 9.00m }
        };
        _dbContext.Products.AddRange(products);
        _dbContext.SaveChanges();

        var suppliers = new List<Supplier>
        {
            new()
            {
                SupplierId = 1,
                CompanyName = "Wiener Feinkost GmbH",
                ContactName = "Anna Stöger",
                ContactTitle = "Einkaufsleiterin",
                City = "Vienna",
                Region = "Wien",
                Country = "Austria",
                PostalCode = "1010",
                Phone = "+43 1 234 5678",
                EMail = "anna.stoeger@wiener-feinkost.at",
                Address = "Graben 21",
                HomePage = "https://wiener-feinkost.at",
                Products = new List<Product> { products[0], products[1] }
            },
            new()
            {
                SupplierId = 2,
                CompanyName = "Partenope Gastronomia S.r.l.",
                ContactName = "Antonio Bianchi",
                ContactTitle = "Manager Operativo",
                City = "Napoli",
                Region = "Campania",
                Country = "Italy",
                PostalCode = "80132",
                Phone = "+39 081 555 7890",
                EMail = "antonio.bianchi@partenope.it",
                Address = "Corso Umberto I 15",
                HomePage = "https://pizza-napoli.it",
                Products = new List<Product> { products[2] }
            }
        };
        _dbContext.Suppliers.AddRange(suppliers);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetSuppliers_ReturnsSupplierCollection()
    {
        // Act
        var result = await _tools.GetSuppliers();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Suppliers);
        Assert.Equal(2, result.Suppliers.Count);
        
        var supplier1 = result.Suppliers.First(s => s.SupplierId == 1);
        Assert.Equal("Wiener Feinkost GmbH", supplier1.CompanyName);
        Assert.Equal(2, supplier1.Products.Count);
    }

    [Fact]
    public async Task RequestOffer_ReturnsValidOffer_WhenProductsAvailable()
    {
        // Arrange
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 },
            new() { Product = "Germknoedel", RequestedQuantity = 5 }
        };

        // Act
        var result = await _tools.RequestOffer(1, offerDetails);

        // Assert
        Assert.NotNull(result);
        var offer = JsonSerializer.Deserialize<Offer>(result);
        Assert.NotNull(offer);
        Assert.Equal(1, offer.SupplierId);
        Assert.Equal(2, offer.OfferDetails.Count);
        
        var schnitzelOffer = offer.OfferDetails.First(o => o.ProductName == "Wiener Schnitzel");
        Assert.Equal(14.00m, schnitzelOffer.Price);
        Assert.Equal(10, schnitzelOffer.RequestedQuantity);
        Assert.Equal(10, schnitzelOffer.Quantity);
    }

    [Fact]
    public async Task RequestOffer_ReturnsZeroQuantity_WhenProductNotAvailable()
    {
        // Arrange
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Pizza Napoli", RequestedQuantity = 10 }
        };

        // Act
        var result = await _tools.RequestOffer(1, offerDetails); // Supplier 1 doesn't have Pizza

        // Assert
        Assert.NotNull(result);
        var offer = JsonSerializer.Deserialize<Offer>(result);
        Assert.NotNull(offer);
        
        var pizzaOffer = offer.OfferDetails.First(o => o.ProductName == "Pizza Napoli");
        Assert.Equal(0m, pizzaOffer.Price);
        Assert.Equal(10, pizzaOffer.RequestedQuantity);
        Assert.Equal(0, pizzaOffer.Quantity);
    }

    [Fact]
    public async Task RequestOffer_ThrowsException_WhenNoDetailsProvided()
    {
        // Arrange
        var offerDetails = new List<OfferRequestDetail>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _tools.RequestOffer(1, offerDetails));
    }

    [Fact]
    public async Task PlaceOrder_WithValidOffer_UpdatesOfferStatus()
    {
        // Arrange - First request an offer
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 }
        };
        var offerJson = await _tools.RequestOffer(1, offerDetails);
        var offer = JsonSerializer.Deserialize<Offer>(offerJson);
        Assert.NotNull(offer);

        // Act - Place an order with the offer
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 10 }
        });
        var result = await _tools.PlaceOrder(
            "REQ-001",
            1,
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson,
            offer.OfferId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("Error:", result);
        
        // Verify offer status was updated to Accepted (1)
        var updatedOffer = await _dbContext.Offers.FindAsync(offer.OfferId);
        Assert.NotNull(updatedOffer);
        Assert.Equal(1, updatedOffer.Status);
    }

    [Fact]
    public async Task PlaceOrder_WithInvalidOfferId_ReturnsError()
    {
        // Arrange
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 10 }
        });

        // Act
        var result = await _tools.PlaceOrder(
            "REQ-002",
            1,
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson,
            Guid.NewGuid().ToString()); // Non-existent offer ID

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("was not found", result);
    }

    [Fact]
    public async Task PlaceOrder_WithMismatchedPrice_ReturnsError()
    {
        // Arrange - First request an offer
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 }
        };
        var offerJson = await _tools.RequestOffer(1, offerDetails);
        var offer = JsonSerializer.Deserialize<Offer>(offerJson);
        Assert.NotNull(offer);

        // Act - Try to place an order with wrong price
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 15.00m, Quantity = 10 } // Wrong price
        });
        var result = await _tools.PlaceOrder(
            "REQ-003",
            1,
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson,
            offer.OfferId.ToString());

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("does not match the offer price", result);
    }

    [Fact]
    public async Task PlaceOrder_WithExceededQuantity_ReturnsError()
    {
        // Arrange - First request an offer
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 }
        };
        var offerJson = await _tools.RequestOffer(1, offerDetails);
        var offer = JsonSerializer.Deserialize<Offer>(offerJson);
        Assert.NotNull(offer);

        // Act - Try to place an order with more quantity than offered
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 20 } // More than offered
        });
        var result = await _tools.PlaceOrder(
            "REQ-004",
            1,
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson,
            offer.OfferId.ToString());

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("exceeds the offered quantity", result);
    }

    [Fact]
    public async Task PlaceOrder_WithoutOfferId_PlacesOrderSuccessfully()
    {
        // Arrange - No offer needed
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 10 }
        });

        // Act - Place order without offer validation
        var result = await _tools.PlaceOrder(
            "REQ-005",
            1,
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("Error:", result);
        Assert.Contains("Order placed successfully", result);
    }

    [Fact]
    public async Task PlaceOrder_WithMismatchedSupplier_ReturnsError()
    {
        // Arrange - Request offer from supplier 1
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 }
        };
        var offerJson = await _tools.RequestOffer(1, offerDetails);
        var offer = JsonSerializer.Deserialize<Offer>(offerJson);
        Assert.NotNull(offer);

        // Act - Try to place order with different supplier
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 10 }
        });
        var result = await _tools.PlaceOrder(
            "REQ-006",
            2, // Different supplier
            DateTime.UtcNow.ToString("o"),
            orderDetailsJson,
            offer.OfferId.ToString());

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("does not match the order supplier", result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
