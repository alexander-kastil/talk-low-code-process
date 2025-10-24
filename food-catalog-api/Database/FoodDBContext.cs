using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FoodApi
{
    public class FoodDBContext : DbContext //Use DbContext if not using Identity
    {
        private readonly string imgBaseUrl;

        public FoodDBContext(DbContextOptions<FoodDBContext> options, IConfiguration configuration) : base(options)
        {
            imgBaseUrl = configuration?["App:ImgBaseUrl"] ?? string.Empty;
            Database.EnsureCreated();
        }

        public DbSet<FoodItem> Food { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<FoodItem> list = new List<FoodItem>();
            list.Add(new FoodItem
            {
                ID = 1,
                Name = "Butter Chicken",
                InStock = 12,
                MinStock = 3,
                Price = 12m,
                Description = "Tender chicken simmered in a smoky tomato-butter sauce that’s bright with garam masala. Each bite balances gentle heat with velvety richness, making it a comfort dish you’ll keep dipping naan into.",
                PictureUrl = imgBaseUrl + "butter-chicken.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 2,
                Name = "Blini with Salmon",
                InStock = 16,
                MinStock = 4,
                Price = 9m,
                Description = "Fluffy buckwheat blini crowned with cool crème fraîche and ribbons of smoked salmon. The briny pop of capers and dill keeps every forkful light, elegant, and perfect for lingering brunches.",
                PictureUrl = imgBaseUrl + "blini-with-salmon.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 3,
                Name = "Wiener Schnitzel",
                InStock = 30,
                MinStock = 6,
                Price = 18m,
                Description = "A paper-thin veal cutlet fried until the golden crust crackles at the first bite. Finished with a squeeze of lemon, it’s a plate that brings alpine coziness and Oktoberfest swagger to the table.",
                PictureUrl = imgBaseUrl + "wiener-schnitzel.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 4,
                Name = "Cevapcici",
                InStock = 20,
                MinStock = 5,
                Price = 11m,
                Description = "Small, spiced grilled beef-and-pork sausages that char at the edges and stay juicy inside. Serve with warm flatbread, tangy ajvar, and a shower of fresh parsley — perfect for sharing.",
                PictureUrl = imgBaseUrl + "cevapcici.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 5,
                Name = "Germknödel",
                InStock = 26,
                MinStock = 6,
                Price = 7m,
                Description = "A pillowy yeast dumpling filled with sweet plum jam, steamed until cloud-soft and finished with melted butter and a dusting of poppy seeds — alpine comfort in every spoonful.",
                PictureUrl = imgBaseUrl + "germknoedel.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 6,
                Name = "Greek Salad",
                InStock = 23,
                MinStock = 5,
                Price = 8m,
                Description = "Crisp cucumber, ripe tomatoes, Kalamata olives and creamy feta tossed with oregano and olive oil. Bright, briny, and refreshing — the sunlit flavors of the Aegean on a plate.",
                PictureUrl = imgBaseUrl + "greek-saled.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 7,
                Name = "Spare Ribs",
                InStock = 13,
                MinStock = 3,
                Price = 16m,
                Description = "Slow-cooked until the meat slips from the bone, glazed in a sticky, smoky sauce with a hint of sweetness. Pair with crisp slaw and plenty of napkins for the full experience.",
                PictureUrl = imgBaseUrl + "spare-ribs.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 8,
                Name = "Pad Ka Prao",
                InStock = 18,
                MinStock = 4,
                Price = 10m,
                Description = "A classic Thai street-food stir-fry of minced meat or tofu with garlic, fiery chiles and holy basil — pungent, savory, and finished with a runny fried egg on top. Serve with jasmine rice for maximum comfort.",
                PictureUrl = imgBaseUrl + "pad-ka-prao.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 9,
                Name = "Falafel with Humus",
                InStock = 26,
                MinStock = 6,
                Price = 9m,
                Description = "Crispy, golden falafel served alongside silky hummus, bright pickles and warm pita — a satisfying vegetarian plate packed with herbaceous crunch and creamy comfort.",
                PictureUrl = imgBaseUrl + "falafel.jpg"
            });
            modelBuilder.Entity<FoodItem>()
                .Property(food => food.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<FoodItem>().HasData(list.ToArray());
        }
    }
}