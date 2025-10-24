using System.Collections.ObjectModel;

namespace PurchasingService.Data;

public static class SupplierStore
{
    private static readonly IReadOnlyList<Supplier> Suppliers = new ReadOnlyCollection<Supplier>(
        new List<Supplier>
        {
            new()
            {
                SupplierId = 1,
                CompanyName = "Wiener Feinkost GmbH",
                ContactName = "Anna Schmid",
                ContactTitle = "Einkaufsleiterin",
                City = "Vienna",
                Region = "Wien",
                Country = "Austria",
                PostalCode = "1010",
                Phone = "+43 1 234 5678",
                EMail = "anna.schmid@wiener-feinkost.example",
                Address = "Graben 21",
                HomePage = "https://wiener-feinkost.example",
                Products = new List<string>
                {
                    "Butter Chicken",
                    "Blini with Salmon",
                    "Wiener Schnitzel",
                    "Cevapcici",
                    "Germknoedel",
                    "Greek Salad",
                    "Spare Ribs",
                    "Falafel with Humus."
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
                EMail = "info@muenchner-gewuerze.example",
                Address = "Marienplatz 1",
                HomePage = "https://muenchner-gewuerze.example",
                Products = new List<string>
                {
                    "Butter Chicken",
                    "Blini with Salmon",
                    "Wiener Schnitzel",
                    "Cevapcici",
                    "Germknoedel",
                    "Greek Salad",
                    "Spare Ribs",
                    "Falafel with Humus."
                }
            },
            new()
            {
                SupplierId = 3,
                CompanyName = "Antica Cucina S.r.l.",
                ContactName = "Lucia Romano",
                ContactTitle = "Responsabile Acquisti",
                City = "Milano",
                Region = "Lombardia",
                Country = "Italy",
                PostalCode = "20121",
                Phone = "+39 02 3456789",
                EMail = "info@anticacucina.example",
                Address = "Via Dante 34",
                HomePage = "https://anticacucina.example",
                Products = new List<string>
                {
                    "Butter Chicken",
                    "Blini with Salmon",
                    "Wiener Schnitzel",
                    "Cevapcici",
                    "Germknoedel",
                    "Greek Salad",
                    "Spare Ribs",
                    "Falafel with Humus."
                }
            },
            new()
            {
                SupplierId = 4,
                CompanyName = "Bangkok Foods Co., Ltd.",
                ContactName = "Somsak Chaiyawan",
                ContactTitle = "Operations Manager",
                City = "Bangkok",
                Region = string.Empty,
                Country = "Thailand",
                PostalCode = "10100",
                Phone = "+66 2 123 4567",
                EMail = "somsak.chaiyawan@bangkokfoods.example",
                Address = "Sukhumvit Rd. 45",
                HomePage = "https://bangkokfoods.example",
                Products = new List<string>
                {
                    "Pad Ka Prao"
                }
            }
        });

    public static IReadOnlyList<Supplier> GetSuppliers() => Suppliers;

    public static Supplier? GetSupplierById(int supplierId) =>
        Suppliers.FirstOrDefault(s => s.SupplierId == supplierId);

    public static Supplier? GetSupplierByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return Suppliers.FirstOrDefault(
            s => string.Equals(s.CompanyName, name, StringComparison.OrdinalIgnoreCase));
    }
}
