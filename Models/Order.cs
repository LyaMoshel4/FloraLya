using System;

namespace LyaShop.Models
{
    public class Order
    {
        public int Id { get; set; } // מספר הזמנה אוטומטי
        public string CustomerId { get; set; } // תעודת הזהות של הלקוח שביצע את ההזמנה
        public DateTime OrderDate { get; set; } // תאריך ושעת ההזמנה
        public double TotalPrice { get; set; } // סכום ההזמנה הכולל
        public string Status { get; set; } // סטטוס (למשל: "הושלמה", "בטיפול")
        public string ItemsSummary { get; set; } // פירוט קצר של מה שנקנה (למשל: "זר ורדים אדומים")

        // קישור למודל הלקוח
        public Customer? Customer { get; set; }
    }
}