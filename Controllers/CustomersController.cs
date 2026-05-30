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

        // 2. קבלת הנתונים מהטופס ושמירה ב-Session (מאוחד למשתמש ומנהל)
        [HttpPost]
        public IActionResult Login(string customerName, string customerID)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerID))
            {
                ViewBag.Error = "אנא הכנס שם ותעודת זהות תקינים.";
                return View();
            }

            // בדיקה: האם מדובר במנהל המערכת?
            // (תוכלי לשנות את admin@lyashop.com והסיסמה למה שתרצי)
            if (customerName == "admin@lyashop.com" && customerID == "MySecretPassword123")
            {
                // הגדרת ה-Session בדיוק כפי שהלייאאוט החדש מצפה לו עבור מנהל
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.Remove("CustomerName"); // ניקוי שם לקוח ליתר ביטחון

                // הפניה לדף הניהול הראשי של המנהל
                return RedirectToAction("Dashboard", "Admin");
            }

            // במידה וזה לא המנהל - הוא נרשם כלקוח רגיל עם הפרטים שהקליד
            HttpContext.Session.SetString("CustomerName", customerName);
            HttpContext.Session.SetString("CustomerID", customerID);
            HttpContext.Session.SetString("IsAdmin", "false"); // מוודא שהוא לא נחשב מנהל

            // הפניה לדף הבית אחרי התחברות מוצלחת
            return RedirectToAction("Index", "Home");
        }

        // 3. יציאה מהמערכת (Logout)
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // מוחק את כל נתוני המשתמש והמנהל מהזיכרון
            return RedirectToAction("Index", "Home");
        }
    }
}