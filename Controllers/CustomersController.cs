using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LyaShop.Controllers
{
    public class CustomersController : Controller
    {
        // דף הכניסה (Login)
        public IActionResult Login()
        {
            return View();
        }

        // קבלת הנתונים מהטופס
        [HttpPost]
        public IActionResult Login(string customerName, string customerID)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerID))
            {
                ViewBag.Error = "אנא הכנס שם ותעודת זהות תקינים.";
                return View();
            }

            // שמירת הפרטים בזיכרון (Session) כדי שנשתמש בהם ביצירת הזר
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("CustomerID", customerID);

            // הפניה לדף הבית אחרי התחברות מוצלחת
            return RedirectToAction("Index", "Home");
        }
    }
}