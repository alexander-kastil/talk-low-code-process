using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using PurchasingService.Configuration;
using PurchasingService.Data;
using PurchasingService.Graph;
using PurchasingService.Models;
using PurchasingService.Services;
using PurchasingService.Tools;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace PurchasingService.Tests;

// Mock IChatClient for testing
internal class MockChatClient : IChatClient
{
    public ChatClientMetadata Metadata => new("MockClient");

    public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = new ChatResponse([new ChatMessage(ChatRole.Assistant, "Mock response")]);
        return Task.FromResult(response);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        yield break;
    }

    public object? GetService(Type serviceType, object? key = null) => null;

    public TService? GetService<TService>(object? key = null) where TService : class => null;

    public void Dispose() { }
}

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
        var configService = new ConfigurationService(_dbContext);
        _offerRandomizer = new OfferRandomizer(randomProvider, configService, _dbContext);

        // Create mock dependencies for optional services in tests
        var graphOptions = Microsoft.Extensions.Options.Options.Create(new GraphOptions 
        { 
            TenantId = "test", 
            ClientId = "test", 
            ClientSecret = "test", 
            MailSender = "test@test.com" 
        });
        var graphHelper = new GraphHelper(graphOptions);
        var emailOptions = Microsoft.Extensions.Options.Options.Create(new EmailOptions());
        
        // Create a minimal mock chat client - we won't use it in tests
        var chatClient = new MockChatClient();
        
        _inquiryService = new InquiryService(_offerRandomizer, graphHelper, chatClient, _dbContext, emailOptions);
        
        var orderService = new OrderService(_dbContext);
        
        // Mock logger
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<PurchasingTools>();
        
        _tools = new PurchasingTools(_supplierService, _inquiryService, orderService, logger, graphHelper, chatClient, emailOptions);
    }

    private void SeedTestData()
    {
        // Seed configuration settings for OfferRandomizer
        var configSettings = new List<ConfigurationSetting>
        {
            new() { Id = 1, Key = "OfferRandomizer_TransportationCost", Value = "30.00" },
            new() { Id = 2, Key = "OfferRandomizer_Pricing_BaseProbability", Value = "1.0" }, // Always base price for tests
            new() { Id = 3, Key = "OfferRandomizer_Pricing_DiscountProbability", Value = "0.0" },
            new() { Id = 4, Key = "OfferRandomizer_Pricing_MarkupProbability", Value = "0.0" },
            new() { Id = 5, Key = "OfferRandomizer_Pricing_DiscountMin", Value = "0.01" },
            new() { Id = 6, Key = "OfferRandomizer_Pricing_DiscountMax", Value = "0.10" },
            new() { Id = 7, Key = "OfferRandomizer_Pricing_MarkupMin", Value = "0.05" },
            new() { Id = 8, Key = "OfferRandomizer_Pricing_MarkupMax", Value = "0.25" },
            new() { Id = 9, Key = "OfferRandomizer_Quantity_FulfillProbability", Value = "1.0" }, // Always fulfill for tests
            new() { Id = 10, Key = "OfferRandomizer_Quantity_ReducedProbability", Value = "0.0" },
            new() { Id = 11, Key = "OfferRandomizer_Quantity_UnavailableProbability", Value = "0.0" },
            new() { Id = 12, Key = "OfferRandomizer_Quantity_ReducedMin", Value = "0.01" },
            new() { Id = 13, Key = "OfferRandomizer_Quantity_ReducedMax", Value = "0.30" },
            new() { Id = 14, Key = "OfferRandomizer_Delivery_CommonProbability", Value = "0.7" },
            new() { Id = 15, Key = "OfferRandomizer_Delivery_CommonDays", Value = "2,3" },
            new() { Id = 16, Key = "OfferRandomizer_Delivery_AdditionalDaysBase", Value = "4" },
            new() { Id = 17, Key = "OfferRandomizer_Delivery_AdditionalDaysRange", Value = "4" },
            new() { Id = 18, Key = "OfferRandomizer_Delivery_SameDaySingleDeliveryPercentage", Value = "100" } // Always same day for tests
        };
        _dbContext.ConfigurationSettings.AddRange(configSettings);
        _dbContext.SaveChanges();

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
            offer.OfferId.ToString(),
            1,
            orderDetailsJson,
            "REQ-001");

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("Error:", result);
        
        // Verify offer status was updated to Accepted (1)
        var updatedOffer = await _dbContext.Offers.FindAsync(offer.OfferId);
        Assert.NotNull(updatedOffer);
        Assert.Equal(OfferStatus.Accepted, updatedOffer.Status);
    }

    [Fact]
    public async Task PlaceOrder_WithInvalidOfferId_ThrowsException()
    {
        // Arrange
        var orderDetailsJson = JsonSerializer.Serialize(new[]
        {
            new { ProductName = "Wiener Schnitzel", Price = 14.00m, Quantity = 10 }
        });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => 
            await _tools.PlaceOrder(
                Guid.NewGuid().ToString(), // Non-existent offer ID
                1,
                orderDetailsJson,
                "REQ-002"));
        
        Assert.Contains("was not found", ex.Message);
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
            offer.OfferId.ToString(),
            1,
            orderDetailsJson,
            "REQ-003");

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
            offer.OfferId.ToString(),
            1,
            orderDetailsJson,
            "REQ-004");

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("exceeds the offered quantity", result);
    }

    [Fact]
    public async Task PlaceOrder_WithValidOfferId_PlacesOrderSuccessfully()
    {
        // Arrange - First request an offer
        var offerDetails = new List<OfferRequestDetail>
        {
            new() { Product = "Wiener Schnitzel", RequestedQuantity = 10 }
        };
        var offerJson = await _tools.RequestOffer(1, offerDetails);
        var offer = JsonSerializer.Deserialize<Offer>(offerJson);
        Assert.NotNull(offer);

        // Act - Place order without providing orderDetailsJson (use offer details)
        var result = await _tools.PlaceOrder(
            offer.OfferId.ToString(),
            1,
            null,
            "REQ-005");

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("Error:", result);
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
            offer.OfferId.ToString(),
            2, // Different supplier
            orderDetailsJson,
            "REQ-006");

        // Assert
        Assert.Contains("Error:", result);
        Assert.Contains("does not match the order supplier", result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
