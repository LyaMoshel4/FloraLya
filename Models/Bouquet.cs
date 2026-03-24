using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LyaShop.Models
{
    public class Bouquet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "הזר שלי";

        public decimal Price { get; set; }
        public string? BouquetDesignHtml { get; set; }
        public List<BouquetItem> FlowersInBouquet { get; set; } = new List<BouquetItem>();
    }

    public class BouquetItem
    {
        [Key]
        public int Id { get; set; }
        public int BouquetId { get; set; }
        public Bouquet? Bouquet { get; set; }
        public int FlowerId { get; set; }
        public Flower? Flower { get; set; }
        public int Quantity { get; set; } = 1;
    }
}