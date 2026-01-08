using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LyaShop.Models
{
    public class FlowerInBouquet
    {
        [Key]
        public int Id { get; set; }

        public int BouquetId { get; set; }
        [ForeignKey("BouquetId")]
        public virtual Bouquet? Bouquet { get; set; }

        public int FlowerId { get; set; }
        [ForeignKey("FlowerId")]
        public virtual Flower? Flower { get; set; }

        public int Quantity { get; set; } = 1;
    }
}