using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LyaShop.Controllers
{
    public class BouquetsController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public BouquetsController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // 1. דף ניהול - רק מנהל רואה את רשימת כל הזרים + ניקוי אוטומטי של זרים מקולקלים
        public async Task<IActionResult> Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            if (!isAdmin)
                return RedirectToAction("Index", "Home");

            // --- תחילת מנגנון ניקוי אוטומטי לזרים הישנים ---
            var oldBouquets = _context.Bouquet
                .Where(b => b.BouquetDesignHtml == null || !b.BouquetDesignHtml.Contains("bouquet-saved-canvas"))
                .ToList();

            if (oldBouquets.Any())
            {
                // מחיקת קשרי הפרחים של הזרים הישנים כדי למנוע שגיאות מפתח זר
                foreach (var oldBouquet in oldBouquets)
                {
                    var relationItems = _context.FlowerInBouquet.Where(f => f.BouquetId == oldBouquet.Id);
                    _context.FlowerInBouquet.RemoveRange(relationItems);
                }

                // מחיקת הזרים עצמם
                _context.Bouquet.RemoveRange(oldBouquets);
                await _context.SaveChangesAsync();
            }
            // --- סוף מנגנון הניקוי ---

            return View(await _context.Bouquet.ToListAsync());
        }

        // 2. דף המעצב (Designer) - יצירה
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View();
        }

        // 3. שמירת הזר (למשתמשים רשומים / מנהל)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bouquet bouquet, int[] selectedFlowers, string shippingAddress, string customerPhone)
        {
            HttpContext.Session.SetString("TempBouquetName", bouquet.Name ?? "זר בעיצוב אישי");
            HttpContext.Session.SetString("TempBouquetPrice", bouquet.Price.ToString());
            HttpContext.Session.SetString("TempBouquetHtml", bouquet.BouquetDesignHtml ?? "");

            if (selectedFlowers != null)
                HttpContext.Session.SetString("TempSelectedFlowers", JsonSerializer.Serialize(selectedFlowers));

            // אם המשתמש לא מחובר, נפנה אותו לדף פרטי אורח
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerName")) && HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("GuestDetails");
            }

            if (ModelState.IsValid)
            {
                // הוספת שם הלקוח המחובר לשם הזר כדי שהמנהל יראה מי עיצב אותו
                var customerName = HttpContext.Session.GetString("CustomerName");
                if (!string.IsNullOrEmpty(customerName))
                {
                    bouquet.Name = (bouquet.Name ?? "זר בעיצוב אישי") + " (לקוח: " + customerName + ")";
                }

                // עדכון השדות החדשים עבור הזר הנוכחי
                bouquet.ShippingAddress = shippingAddress;
                bouquet.CustomerPhone = customerPhone;
                bouquet.CreatedAt = DateTime.Now;

                _context.Add(bouquet);
                await _context.SaveChangesAsync();

                if (selectedFlowers != null)
                {
                    foreach (var flowerId in selectedFlowers)
                    {
                        _context.FlowerInBouquet.Add(new FlowerInBouquet { BouquetId = bouquet.Id, FlowerId = flowerId, Quantity = 1 });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Checkout", new { id = bouquet.Id });
            }

            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View(bouquet);
        }

        // --- מחיקת הזר ---
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet == null) return NotFound();

            return View(bouquet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet != null)
            {
                var flowersInBouquet = _context.FlowerInBouquet.Where(f => f.BouquetId == id);
                _context.FlowerInBouquet.RemoveRange(flowersInBouquet);

                _context.Bouquet.Remove(bouquet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BouquetExists(int id)
        {
            return _context.Bouquet.Any(e => e.Id == id);
        }

        // 4. דף תשלום
        [HttpGet]
        public async Task<IActionResult> Checkout(int id)
        {
            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet == null) return RedirectToAction("Create");
            return View(bouquet);
        }

        // 5. פרטי אורח
        [HttpGet]
        public IActionResult GuestDetails()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("TempBouquetHtml")))
                return RedirectToAction("Create");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveGuestOrder(string guestName, string guestPhone, string guestAddress)
        {
            var bouquet = new Bouquet
            {
                Name = (HttpContext.Session.GetString("TempBouquetName") ?? "זר") + " (אורח: " + guestName + ")",
                Price = decimal.TryParse(HttpContext.Session.GetString("TempBouquetPrice"), out decimal p) ? p : 0,
                BouquetDesignHtml = HttpContext.Session.GetString("TempBouquetHtml"),
                ShippingAddress = guestAddress,
                CustomerPhone = guestPhone,
                CreatedAt = DateTime.Now
            };

            _context.Add(bouquet);
            await _context.SaveChangesAsync();

            var flowersJson = HttpContext.Session.GetString("TempSelectedFlowers");
            if (!string.IsNullOrEmpty(flowersJson))
            {
                var selectedFlowers = JsonSerializer.Deserialize<int[]>(flowersJson);
                foreach (var flowerId in selectedFlowers)
                {
                    _context.FlowerInBouquet.Add(new FlowerInBouquet { BouquetId = bouquet.Id, FlowerId = flowerId, Quantity = 1 });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Checkout", new { id = bouquet.Id });
        }

        // 6. דף אישור סופי + שמירה אוטומטית לטבלת Orders של הלקוח הרשום
        [HttpGet]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var customerId = HttpContext.Session.GetString("CustomerID");

            if (!string.IsNullOrEmpty(customerId))
            {
                var bouquet = await _context.Bouquet.FindAsync(id);
                if (bouquet != null && _context.Orders != null)
                {
                    // מניעת כפילויות בריפרש
                    var orderExists = await _context.Orders.AnyAsync(o => o.CustomerId == customerId && o.OrderDate > DateTime.Now.AddMinutes(-2));

                    if (!orderExists)
                    {
                        var newOrder = new Order
                        {
                            CustomerId = customerId,
                            OrderDate = DateTime.Now,
                            TotalPrice = (double)bouquet.Price, // המרה מ-decimal ל-double בהתאם למודל שלך
                            Status = "בטיפול",
                            ItemsSummary = bouquet.Name
                        };

                        _context.Orders.Add(newOrder);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // ניקוי סשן זמני
            HttpContext.Session.Remove("TempBouquetName");
            HttpContext.Session.Remove("TempBouquetPrice");
            HttpContext.Session.Remove("TempBouquetHtml");
            HttpContext.Session.Remove("TempSelectedFlowers");

            ViewBag.OrderId = id;
            return View();
        }
    }
}