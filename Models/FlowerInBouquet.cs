using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LyaShop.Models
{
    public class FlowerInBouquet
    {
        public int Id { get; set; }

        public int FlowerId { get; set; }
        public Flower? Flower { get; set; }

        public int BouquetId { get; set; }
        public Bouquet? Bouquet { get; set; }

        public int Quantity { get; set; }
    }
}