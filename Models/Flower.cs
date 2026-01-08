namespace LyaShop.Models
{
    public class Flower
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Price { get; set; }
        public int StockQnty { get; set; }
        public string? ImagePath { get; set; }
    }
}