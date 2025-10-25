using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;
using PurchasingService.Models;
using System.Security.Cryptography;

namespace PurchasingService.Services;

public interface IRandomProvider
{
    double NextDouble();

    double NextDouble(double minInclusive, double maxInclusive);
}

public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly Random _random;

    public SystemRandomProvider(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public double NextDouble() => _random.NextDouble();

    public double NextDouble(double minInclusive, double maxInclusive)
    {
        if (maxInclusive < minInclusive)
        {
            throw new ArgumentOutOfRangeException(nameof(maxInclusive), "Maximum must be greater than minimum.");
        }

        return minInclusive + (_random.NextDouble() * (maxInclusive - minInclusive));
    }
}

/// <summary>
/// Cryptographically strong random provider. Produces uniform doubles in [0,1).
/// This is non-deterministic and should be used when unpredictability is preferred.
/// </summary>
public sealed class SecureRandomProvider : IRandomProvider, IDisposable
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    // Generate a double in [0, 1) using 53 bits of randomness (same precision as double mantissa)
    public double NextDouble()
    {
        Span<byte> buffer = stackalloc byte[8];
        _rng.GetBytes(buffer);
        ulong value = BitConverter.ToUInt64(buffer);

        // Use the top 53 bits to create a double in [0,1)
        const ulong mask53 = (1UL << 53) - 1UL;
        var top53 = value & mask53;
        return top53 / (double)(1UL << 53);
    }

    public double NextDouble(double minInclusive, double maxInclusive)
    {
        if (maxInclusive < minInclusive)
            throw new ArgumentOutOfRangeException(nameof(maxInclusive), "Maximum must be greater than minimum.");

        var r = NextDouble();
        return minInclusive + (r * (maxInclusive - minInclusive));
    }

    public void Dispose() => _rng.Dispose();
}

public interface IOfferRandomizer
{
    decimal TransportationCost { get; }

    Task<OfferDetails> GenerateOfferAsync(string productName, int requestedAmount);
}

public sealed class OfferRandomizer : IOfferRandomizer
{
    private readonly IRandomProvider _random;
    private readonly IConfigurationService _configService;
    private readonly PurchasingDbContext _dbContext;
    private OfferRandomizerOptions? _options;

    public OfferRandomizer(IRandomProvider randomProvider, IConfigurationService configService, PurchasingDbContext dbContext)
    {
        _random = randomProvider ?? throw new ArgumentNullException(nameof(randomProvider));
        _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task<OfferRandomizerOptions> GetOptionsAsync()
    {
        if (_options == null)
        {
            _options = await _configService.GetOfferRandomizerOptionsAsync();
        }
        return _options;
    }

    public decimal TransportationCost
    {
        get
        {
            // For synchronous access, we need to block here
            // This is not ideal but necessary for the interface contract
            return GetOptionsAsync().GetAwaiter().GetResult().TransportationCost;
        }
    }

    private async Task<decimal?> TryGetBasePriceAsync(string productName)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Name == productName);
        return product?.BasePrice;
    }

    public async Task<OfferDetails> GenerateOfferAsync(string productName, int requestedAmount)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Product name must be provided.", nameof(productName));
        }

        if (requestedAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedAmount), "Requested amount must be greater than zero.");
        }

        var options = await GetOptionsAsync();

        var basePrice = await TryGetBasePriceAsync(productName);
        if (!basePrice.HasValue)
        {
            throw new ArgumentException($"No base price configured for product '{productName}'.", nameof(productName));
        }

        var offeredPrice = CalculateOfferedPrice(basePrice.Value, options);
        var sameDayProbability = Math.Clamp(options.Delivery.SameDaySingleDeliveryPercentage, 0d, 100d) / 100d;
        var isSameDaySingleDelivery = _random.NextDouble() < sameDayProbability;

        int offeredAmount;
        bool isAvailable;
        int deliveryDurationDays;

        if (isSameDaySingleDelivery)
        {
            offeredAmount = requestedAmount;
            isAvailable = true;
            // Present same-day delivery as 1 day to avoid zero-day promises in output.
            deliveryDurationDays = 1;
        }
        else
        {
            (offeredAmount, isAvailable) = CalculateOfferedAmount(requestedAmount, options);
            deliveryDurationDays = CalculateDeliveryDurationDays(options);
        }

        return new OfferDetails
        {
            ProductName = productName,
            Price = offeredPrice,
            RequestedQuantity = requestedAmount,
            Quantity = offeredAmount,
            DeliveryDurationDays = deliveryDurationDays,
        };
    }

    private decimal CalculateOfferedPrice(decimal basePrice, OfferRandomizerOptions options)
    {
        var decision = _random.NextDouble();
        var pricing = options.Pricing;

        if (decision < pricing.BaseProbability)
        {
            return basePrice;
        }

        var cumulative = pricing.BaseProbability;

        if (decision < (cumulative += pricing.DiscountProbability))
        {
            var discount = (decimal)_random.NextDouble(pricing.DiscountMin, pricing.DiscountMax);
            return RoundMoney(basePrice * (1 - discount));
        }

        // fallback to markup
        var markup = (decimal)_random.NextDouble(pricing.MarkupMin, pricing.MarkupMax);
        return RoundMoney(basePrice * (1 + markup));
    }

    private (int OfferedAmount, bool IsAvailable) CalculateOfferedAmount(int requestedAmount, OfferRandomizerOptions options)
    {
        var decision = _random.NextDouble();
        var qty = options.Quantity;

        if (decision < qty.FulfillProbability)
        {
            return (requestedAmount, true);
        }

        var cumulative = qty.FulfillProbability;

        if (decision < (cumulative += qty.ReducedProbability))
        {
            var reduction = (decimal)_random.NextDouble(qty.ReducedMin, qty.ReducedMax);
            var reducedAmount = (int)Math.Round(requestedAmount * (double)(1 - reduction), MidpointRounding.AwayFromZero);
            reducedAmount = Math.Clamp(reducedAmount, 1, requestedAmount);
            return (reducedAmount, true);
        }

        return (0, false);
    }

    private int CalculateDeliveryDurationDays(OfferRandomizerOptions options)
    {
        var decision = _random.NextDouble();
        var delivery = options.Delivery;

        if (decision < delivery.CommonProbability)
        {
            // pick one of the common days
            var pick = _random.NextDouble();
            return pick < 0.5d ? delivery.CommonDays[0] : delivery.CommonDays[1];
        }

        var additional = (int)Math.Floor(_random.NextDouble() * delivery.AdditionalDaysRange);
        return delivery.AdditionalDaysBase + additional;
    }

    private static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
