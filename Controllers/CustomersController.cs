using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LyaShop.Controllers
{
    public class CustomersController : Controller
    {
        // 1. דף הכניסה (Login) - תצוגה
        public IActionResult Login()
        {
            return View();
        }

        // 2. קבלת הנתונים מהטופס ושמירה ב-Session
        [HttpPost]
        public IActionResult Login(string customerName, string customerID)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerID))
            {
                ViewBag.Error = "אנא הכנס שם ותעודת זהות תקינים.";
                return View();
            }

            // שמירת הפרטים ב-Session
            // זה מה שגורם ללייאאוט להראות "שלום, יוסי"
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("CustomerID", customerID);

            // הפניה לדף הבית אחרי התחברות
            return RedirectToAction("Index", "Home");
        }

        // 3. יציאה מהמערכת (Logout)
        // חשוב! כדי שהכפתור בלייאאוט יעבוד
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // מוחק את כל נתוני המשתמש מהזיכרון
            return RedirectToAction("Index", "Home");
        }
    }
}