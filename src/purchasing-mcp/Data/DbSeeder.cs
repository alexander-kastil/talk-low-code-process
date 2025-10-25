using Microsoft.EntityFrameworkCore;
using PurchasingService.Models;

namespace PurchasingService.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(PurchasingDbContext context)
    {
        // Check if data already exists
        if (await context.Products.AnyAsync() || await context.Suppliers.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed OfferRandomizer configuration settings with flattened hierarchy
        var configSettings = new List<ConfigurationSetting>
        {
            new() { Key = "OfferRandomizer_TransportationCost", Value = "30.00" },
            new() { Key = "OfferRandomizer_Pricing_BaseProbability", Value = "0.4" },
            new() { Key = "OfferRandomizer_Pricing_DiscountProbability", Value = "0.25" },
            new() { Key = "OfferRandomizer_Pricing_MarkupProbability", Value = "0.35" },
            new() { Key = "OfferRandomizer_Pricing_DiscountMin", Value = "0.01" },
            new() { Key = "OfferRandomizer_Pricing_DiscountMax", Value = "0.10" },
            new() { Key = "OfferRandomizer_Pricing_MarkupMin", Value = "0.05" },
            new() { Key = "OfferRandomizer_Pricing_MarkupMax", Value = "0.25" },
            new() { Key = "OfferRandomizer_Quantity_FulfillProbability", Value = "0.8" },
            new() { Key = "OfferRandomizer_Quantity_ReducedProbability", Value = "0.1" },
            new() { Key = "OfferRandomizer_Quantity_UnavailableProbability", Value = "0.1" },
            new() { Key = "OfferRandomizer_Quantity_ReducedMin", Value = "0.01" },
            new() { Key = "OfferRandomizer_Quantity_ReducedMax", Value = "0.30" },
            new() { Key = "OfferRandomizer_Delivery_CommonProbability", Value = "0.7" },
            new() { Key = "OfferRandomizer_Delivery_CommonDays", Value = "2,3" },
            new() { Key = "OfferRandomizer_Delivery_AdditionalDaysBase", Value = "4" },
            new() { Key = "OfferRandomizer_Delivery_AdditionalDaysRange", Value = "4" },
            new() { Key = "OfferRandomizer_Delivery_SameDaySingleDeliveryPercentage", Value = "80" }
        };

        await context.ConfigurationSettings.AddRangeAsync(configSettings);
        await context.SaveChangesAsync();

        // Create products with base prices
        var products = new List<Product>
        {
            new() { ProductId = 1, Name = "Wiener Schnitzel", BasePrice = 14.00m },
            new() { ProductId = 2, Name = "Germknoedel", BasePrice = 7.00m },
            new() { ProductId = 3, Name = "Kaiserschmarrn", BasePrice = 8.00m },
            new() { ProductId = 4, Name = "Weisswurst mit Brezn", BasePrice = 10.00m },
            new() { ProductId = 5, Name = "Schweinshaxe mit Kraut", BasePrice = 15.00m },
            new() { ProductId = 6, Name = "Pizza Napoli", BasePrice = 9.00m },
            new() { ProductId = 7, Name = "Arancini Napoletana", BasePrice = 6.00m },
            new() { ProductId = 8, Name = "Pad Ka Prao", BasePrice = 5.00m },
            new() { ProductId = 9, Name = "Green Curry", BasePrice = 7.00m }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Create suppliers
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
                Products = new List<Product>
                {
                    products[0], // Wiener Schnitzel
                    products[1], // Germknoedel
                    products[2], // Kaiserschmarrn
                }
            },
            new()
            {
                SupplierId = 2,
                CompanyName = "Muenchner Gewuerze GmbH",
                ContactName = "Juergen Mueller",
                ContactTitle = "Verkaufsleiter",
                City = "Muenchen",
                Region = "Bayern",
                Country = "Germany",
                PostalCode = "80331",
                Phone = "+49 89 123456",
                EMail = "info@muenchner-gewuerze.de",
                Address = "Marienplatz 1",
                HomePage = "https://muenchner-gewuerze.de",
                Products = new List<Product>
                {
                    products[3], // Weisswurst mit Brezn
                    products[4], // Schweinshaxe mit Kraut
                }
            },
            new()
            {
                SupplierId = 3,
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
                Products = new List<Product>
                {
                    products[5], // Pizza Napoli
                    products[6], // Arancini Napoletana
                }
            },
            new()
            {
                SupplierId = 4,
                CompanyName = "Same, same Foods Co., Ltd.",
                ContactName = "Alek Nohep",
                ContactTitle = "Operations Manager",
                City = "Bangkok",
                Region = string.Empty,
                Country = "Thailand",
                PostalCode = "10100",
                Phone = "+66 2 123 4567",
                EMail = "alek.nohep@bangkokfoods.th",
                Address = "Sukhumvit Rd. 45",
                HomePage = "https://bangkokfoods.th",
                Products = new List<Product>
                {
                    products[7], // Pad Ka Prao
                    products[8], // Green Curry
                }
            }
        };

        await context.Suppliers.AddRangeAsync(suppliers);
        await context.SaveChangesAsync();
    }
}
