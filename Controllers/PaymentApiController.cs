using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LyaShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {
        // מודל קטן שמייצג את המידע שנקבל מדף התשלום
        public class PaymentRequest
        {
            public string CardNumber { get; set; }
            public string ExpiryDate { get; set; }
            public string CVV { get; set; }
        }

        [HttpPost("validate")]
        public IActionResult ValidateCard([FromBody] PaymentRequest request)
        {
            // רשימת כרטיסים מדומים "תקינים"
            var validCards = new[] { "1234567812345678", "4242424242424242", "2222222222222222", "8946826839641111" };
            var validCvv = "123";

            // אם הפרטים חסרים בכלל
            if (request == null || string.IsNullOrEmpty(request.CardNumber))
            {
                return BadRequest(new { success = false, message = "נתוני כרטיס חסרים." });
            }

            // בדיקה: אם המספר מופיע ברשימה וה-CVV תואם
            if (validCards.Contains(request.CardNumber) && request.CVV == validCvv)
            {
                // מחזירים תשובה חיובית (HTTP 200)
                return Ok(new { success = true, message = "התשלום אושר בהצלחה בשרת FloraLya!" });
            }

            // אם הפרטים לא תואמים - מחזירים שגיאה (HTTP 400)
            return BadRequest(new { success = false, message = "פרטי האשראי שגויים או שאינם קיימים במאגר המורשה." });
        }
    }
}