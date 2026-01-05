using System.Collections.Generic;
using System.Linq;

namespace LyaFlowerShop.Models
{
    public class Bouquet
    {
        public int Id { get; set; }
        public string? NameByItem { get; set; }
        public string? Customer { get; set; }

        // הקשר הישיר לטבלת המקשרת
        public virtual List<FlowerInBouquet> FlowerInBouquets { get; set; } = new List<FlowerInBouquet>();

        // Property עזר לתצוגה בלבד - לא נשמר בבסיס הנתונים
        public List<Flower> Flowers => FlowerInBouquets.Select(fb => fb.Flower).ToList();
    }
}