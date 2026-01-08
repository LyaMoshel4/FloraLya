using LyaShop.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LyaShop.Models // שונה מ-LyaFlowerShop ל-LyaShop
{
    public class Bouquet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Bouquet Name")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Customer { get; set; }

        // הקשר לטבלה המקשרת
        public virtual ICollection<FlowerInBouquet> FlowersInBouquet { get; set; } = new List<FlowerInBouquet>();
    }
}