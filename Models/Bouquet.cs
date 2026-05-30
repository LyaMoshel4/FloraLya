using System;
using System.ComponentModel.DataAnnotations;

namespace LyaShop.Models
{
    public class Bouquet
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string BouquetDesignHtml { get; set; }

        // השדות החדשים לפרטי משלוח ותאריך
        public string ShippingAddress { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}