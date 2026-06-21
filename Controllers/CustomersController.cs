using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace LyaShop.Controllers
{
    public class CustomersController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public CustomersController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. מסך התחברות (Login)
        // ==========================================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string? customerName, string? customerID)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerID))
            {
                ViewBag.Error = "אנא הכנס שם ותעודת זהות תקינים.";
                return View();
            }

            if (customerName == "admin@lyashop.com" && customerID == "MySecretPassword123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.Remove("CustomerName");
                return RedirectToAction("Dashboard", "Admin");
            }

            // החזרה ל-Customers המקורי והתקין של המודל
            var existingCustomer = _context.Customers != null
                ? _context.Customers.FirstOrDefault(c => c.Id == customerID)
                : null;

            if (existingCustomer != null)
            {
                HttpContext.Session.SetString("CustomerName", existingCustomer.Name ?? "");
                HttpContext.Session.SetString("CustomerID", existingCustomer.Id ?? "");
                HttpContext.Session.SetString("IsAdmin", "false");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "שגיאה: משתמש זה אינו רשום במערכת. אנא לחצי על לחצן 'הרשמה' למעלה כדי להירשם לראשונה.";
            return View();
        }

        // ==========================================
        // 2. מסך הרשמה (Register)
        // ==========================================
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string? customerName, string? customerID)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(customerID))
            {
                ViewBag.Error = "אנא הכנס שם ותעודת זהות תקינים לצורך הרשמה.";
                return View();
            }

            if (_context.Customers == null)
            {
                ViewBag.Error = "שגיאה בחיבור לבסיס הנתונים.";
                return View();
            }

            var checkExists = _context.Customers.FirstOrDefault(c => c.Id == customerID);
            if (checkExists != null)
            {
                ViewBag.Error = "תעודת זהות זו כבר רשומה במערכת. הנך מועבר למסך התחברות.";
                return View("Login");
            }

            var newCustomer = new Customer
            {
                Id = customerID,
                Name = customerName
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            // הוסר הסימן המיותר ששיגע את הקומפיילר
            HttpContext.Session.SetString("CustomerName", newCustomer.Name ?? "");
            HttpContext.Session.SetString("CustomerID", newCustomer.Id ?? "");
            HttpContext.Session.SetString("IsAdmin", "false");

            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        //  פרופיל לקוח והיסטוריית הזמנות
        // ==========================================
        public IActionResult Profile()
        {
            var customerId = HttpContext.Session.GetString("CustomerID");

            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            var customerOrders = _context.Orders != null
                ? _context.Orders.Where(o => o.CustomerId == customerId).OrderByDescending(o => o.OrderDate).ToList()
                : new List<Order>();

            return View(customerOrders);
        }

        // ==========================================
        //  יציאה מהמערכת (Logout)
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}