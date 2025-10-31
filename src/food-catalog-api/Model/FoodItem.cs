using System;

namespace FoodApi
{
    public class FoodItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public int MinStock { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
    }

    public class FoodDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; }
        public int MinStock { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
    }
}