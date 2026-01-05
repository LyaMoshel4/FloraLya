namespace LyaFlowerShop.Models
{
    public class FlowerInBouquet
    {
        public int Id { get; set; }

        // מפתח זר לפרח
        public int FlowerId { get; set; }
        public virtual Flower Flower { get; set; } = null!;

        // מפתח זר לזר
        public int BouquetId { get; set; }
        public virtual Bouquet Bouquet { get; set; } = null!;
    }
}