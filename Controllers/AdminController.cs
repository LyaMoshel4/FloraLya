using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace LyaShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // דף ההתחברות (GET)
        public IActionResult Login()
        {
            // אם כבר מחוברים, מעביר ישר לדף הבית
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Bouquets");
            }
            return View();
        }

        // ביצוע ההתחברות (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // קריאת ההגדרות מתוך appsettings.json בצורה בטוחה
            var adminEmail = _configuration["AdminSettings:Email"];
            var adminPassword = _configuration["AdminSettings:Password"];

            // בדיקה אם המשתמש הכניס את הפרטים הנכונים
            if (email == adminEmail && password == adminPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // משאיר את המשתמש מחובר גם אם סגר את הדפדפן
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // התחברות הצליחה! מעביר לגלריה
                return RedirectToAction("Index", "Bouquets");
            }

            // אם הסיסמה שגויה
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // יציאה מהמערכת (Logout)
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}