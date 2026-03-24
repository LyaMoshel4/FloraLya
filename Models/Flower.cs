using System.ComponentModel.DataAnnotations;

namespace LyaShop.Models
{
    public class Flower
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "נא להזין שם")]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public string? Color { get; set; }

        public decimal Price { get; set; }

        public int StockQnty { get; set; }

        public string? ImageUrl { get; set; }

        // השדה הזה יפתור את השגיאה ב-FlowersController
        public string? ImagePath { get; set; }

        // השדה הזה יפתור את השגיאה הלבנה (Invalid column name) אחרי ה-Update
        public int HueRotation { get; set; } = 0;
    }
}