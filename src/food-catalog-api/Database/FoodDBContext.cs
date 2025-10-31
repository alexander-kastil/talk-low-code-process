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
            // Seed data adjusted per request. Reuse existing image filenames when available; otherwise use "blind-image.jpg".
            list.Add(new FoodItem
            {
                ID = 1,
                Name = "Wiener Schnitzel",
                InStock = 30,
                MinStock = 6,
                Price = 18m,
                Description = "A paper-thin veal cutlet, breaded and fried until golden; served with lemon.",
                PictureUrl = imgBaseUrl + "wiener-schnitzel.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 2,
                Name = "Germknoedel",
                InStock = 26,
                MinStock = 6,
                Price = 7m,
                Description = "A steamed yeast dumpling filled with sweet plum jam, served with melted butter and poppy seeds.",
                PictureUrl = imgBaseUrl + "germknoedel.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 3,
                Name = "Kaiserschmarrn",
                InStock = 22,
                MinStock = 5,
                Price = 9m,
                Description = "Fluffy shredded pancake, caramelized and served with fruit compote — a classic Austrian dessert.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 7,
                Name = "Weißwurst mit Brezn",
                InStock = 20,
                MinStock = 5,
                Price = 10m,
                Description = "Bavarian white sausages with pretzel and sweet mustard — a regional breakfast favorite.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 9,
                Name = "Schweinshaxe mit Kraut",
                InStock = 12,
                MinStock = 3,
                Price = 19m,
                Description = "Roasted pork knuckle with dumplings and sauerkraut — rich, crispy and traditional.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 12,
                Name = "Pizza Napoli",
                InStock = 28,
                MinStock = 6,
                Price = 9m,
                Description = "Classic Neapolitan pizza with simple tomato, mozzarella and fresh basil.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 13,
                Name = "Arancini Napoletana",
                InStock = 15,
                MinStock = 4,
                Price = 7m,
                Description = "Crispy fried rice balls filled with ragù, peas and mozzarella — a Sicilian snack.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 14,
                Name = "Pad Ka Prao",
                InStock = 18,
                MinStock = 4,
                Price = 10m,
                Description = "A spicy Thai stir-fry with holy basil, garlic and chilies; often served with a fried egg.",
                PictureUrl = imgBaseUrl + "pad-ka-prao.jpg"
            });
            list.Add(new FoodItem
            {
                ID = 16,
                Name = "Green Curry",
                InStock = 17,
                MinStock = 4,
                Price = 13m,
                Description = "Fragrant Thai green curry with fresh herbs, chilies and coconut milk.",
                PictureUrl = imgBaseUrl + "blind-image.jpg"
            });
            modelBuilder.Entity<FoodItem>()
                .Property(food => food.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<FoodItem>().HasData(list.ToArray());
        }
    }
}