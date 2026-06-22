using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LyaShop.Controllers
{
    // הגדרת הראוטינג (ניתוב): ה-API יהיה נגיש בכתובת api/PaymentApi
    [Route("api/[controller]")]
    [ApiController] // מאפשר למערכת לדעת שזהו קונטרולר מסוג API שמחזיר מידע (JSON) ולא דפי HTML (Views)
    public class PaymentApiController : ControllerBase
    {
        // מודל פנימי (DTO) שמייצג את מבנה הנתונים שמגיעים מה-Client (דף התשלום בדפדפן)
        public class PaymentRequest
        {
            public string CardNumber { get; set; }
            public string ExpiryDate { get; set; }
            public string CVV { get; set; }
        }

        // יצירת נקודת קצה (Endpoint) שמקבלת בקשות מסוג HTTP POST לכתובת api/PaymentApi/validate
        [HttpPost("validate")]
        public IActionResult ValidateCard([FromBody] PaymentRequest request)
        {
            // מערך של מספרי כרטיסי אשראי "מדומים" (Mock Data) לצורך בדיקת תקינות בלבד
            var validCards = new[] { "1234567812345678", "4242424242424242", "2222222222222222", "8946826839641111" };
            var validCvv = "123";

            // שלב הגנה 1: בדיקה שהבקשה אינה ריקה ושנשלח מספר כרטיס
            if (request == null || string.IsNullOrEmpty(request.CardNumber))
            {
                // החזרת קוד שגיאה HTTP 400 (Bad Request) עם הודעה מתאימה בפורמט JSON
                return BadRequest(new { success = false, message = "נתוני כרטיס חסרים." });
            }

            // שלב הגנה 2: בדיקה הלוגית - האם כרטיס האשראי שנשלח קיים במערך והאם ה-CVV הוא '123'
            if (validCards.Contains(request.CardNumber) && request.CVV == validCvv)
            {
                // החזרת קוד הצלחה HTTP 200 (OK) יחד עם אובייקט אנונימי שמסמן שהכל עבר בהצלחה
                return Ok(new { success = true, message = "התשלום אושר בהצלחה בשרת FloraLya!" });
            }

            // אם הבדיקה נכשלה (הפרטים לא נכונים), מחזירים קוד שגיאה HTTP 400 עם הודעת כישלון
            return BadRequest(new { success = false, message = "פרטי האשראי שגויים או שאינם קיימים במאגר המורשה." });
        }
    }
}