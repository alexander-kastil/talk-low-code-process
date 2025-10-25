using Microsoft.EntityFrameworkCore;
using PurchasingService.Data;
using System.Globalization;

namespace PurchasingService.Services;

public interface IConfigurationService
{
    Task<OfferRandomizerOptions> GetOfferRandomizerOptionsAsync();
}

public class ConfigurationService : IConfigurationService
{
    private readonly PurchasingDbContext _dbContext;

    public ConfigurationService(PurchasingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<OfferRandomizerOptions> GetOfferRandomizerOptionsAsync()
    {
        var settings = await _dbContext.ConfigurationSettings
            .Where(s => s.Key.StartsWith("OfferRandomizer_"))
            .ToDictionaryAsync(s => s.Key, s => s.Value);

        var options = new OfferRandomizerOptions
        {
            TransportationCost = GetDecimalValue(settings, "OfferRandomizer_TransportationCost", 30m),
            Pricing = new OfferRandomizerOptions.PricingOptions
            {
                BaseProbability = GetDoubleValue(settings, "OfferRandomizer_Pricing_BaseProbability", 0.5d),
                DiscountProbability = GetDoubleValue(settings, "OfferRandomizer_Pricing_DiscountProbability", 0.2d),
                MarkupProbability = GetDoubleValue(settings, "OfferRandomizer_Pricing_MarkupProbability", 0.3d),
                DiscountMin = GetDoubleValue(settings, "OfferRandomizer_Pricing_DiscountMin", 0.01d),
                DiscountMax = GetDoubleValue(settings, "OfferRandomizer_Pricing_DiscountMax", 0.10d),
                MarkupMin = GetDoubleValue(settings, "OfferRandomizer_Pricing_MarkupMin", 0.05d),
                MarkupMax = GetDoubleValue(settings, "OfferRandomizer_Pricing_MarkupMax", 0.25d)
            },
            Quantity = new OfferRandomizerOptions.QuantityOptions
            {
                FulfillProbability = GetDoubleValue(settings, "OfferRandomizer_Quantity_FulfillProbability", 0.8d),
                ReducedProbability = GetDoubleValue(settings, "OfferRandomizer_Quantity_ReducedProbability", 0.1d),
                UnavailableProbability = GetDoubleValue(settings, "OfferRandomizer_Quantity_UnavailableProbability", 0.1d),
                ReducedMin = GetDoubleValue(settings, "OfferRandomizer_Quantity_ReducedMin", 0.01d),
                ReducedMax = GetDoubleValue(settings, "OfferRandomizer_Quantity_ReducedMax", 0.30d)
            },
            Delivery = new OfferRandomizerOptions.DeliveryOptions
            {
                CommonProbability = GetDoubleValue(settings, "OfferRandomizer_Delivery_CommonProbability", 0.7d),
                CommonDays = GetIntArrayValue(settings, "OfferRandomizer_Delivery_CommonDays", new[] { 2, 3 }),
                AdditionalDaysBase = GetIntValue(settings, "OfferRandomizer_Delivery_AdditionalDaysBase", 4),
                AdditionalDaysRange = GetIntValue(settings, "OfferRandomizer_Delivery_AdditionalDaysRange", 4),
                SameDaySingleDeliveryPercentage = GetDoubleValue(settings, "OfferRandomizer_Delivery_SameDaySingleDeliveryPercentage", 80d)
            }
        };

        return options;
    }

    private static decimal GetDecimalValue(Dictionary<string, string> settings, string key, decimal defaultValue)
    {
        if (settings.TryGetValue(key, out var value) && decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return defaultValue;
    }

    private static double GetDoubleValue(Dictionary<string, string> settings, string key, double defaultValue)
    {
        if (settings.TryGetValue(key, out var value) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return defaultValue;
    }

    private static int GetIntValue(Dictionary<string, string> settings, string key, int defaultValue)
    {
        if (settings.TryGetValue(key, out var value) && int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        return defaultValue;
    }

    private static int[] GetIntArrayValue(Dictionary<string, string> settings, string key, int[] defaultValue)
    {
        if (settings.TryGetValue(key, out var value))
        {
            var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<int>();
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var intValue))
                {
                    result.Add(intValue);
                }
            }
            if (result.Count > 0)
            {
                return result.ToArray();
            }
        }
        return defaultValue;
    }
}
