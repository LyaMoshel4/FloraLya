namespace LyaFlowerShop.Models
{
    public class Flower
    {
        public int Id { get; set; }

        // הוספת סימן שאלה מאפשרת לשדה להיות ריק ומצמצמת אזהרות קוד
        public string? Name { get; set; }
        public string? Description { get; set; }

        public double Price { get; set; }
        public int StockQnty { get; set; }

        public string? ImagePath { get; set; }
    }
}