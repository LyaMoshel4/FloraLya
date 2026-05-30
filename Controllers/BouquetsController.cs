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

        // 1. דף ניהול - רק מנהל רואה את רשימת כל הזרים
        public async Task<IActionResult> Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            if (!isAdmin)
                return RedirectToAction("Index", "Home");

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

        // --- עריכת הזר ---
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet == null) return NotFound();

            // טעינת הפרחים כדי שיופיעו בתפריט הצד של המעצב
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View(bouquet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bouquet bouquet)
        {
            if (id != bouquet.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // שומרים על תאריך היצירה המקורי בעת עריכה (אם קיים)
                    var existingBouquet = await _context.Bouquet.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existingBouquet != null)
                    {
                        bouquet.CreatedAt = existingBouquet.CreatedAt;
                        if (string.IsNullOrEmpty(bouquet.ShippingAddress)) bouquet.ShippingAddress = existingBouquet.ShippingAddress;
                        if (string.IsNullOrEmpty(bouquet.CustomerPhone)) bouquet.CustomerPhone = existingBouquet.CustomerPhone;
                    }

                    _context.Update(bouquet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BouquetExists(bouquet.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
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
                // מחיקת הפרחים המקושרים לזר קודם (כדי למנוע שגיאת Foreign Key)
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
            // יצירת הזר עם הפרטים שהאורח מילא בטופס הפרטים שלו
            var bouquet = new Bouquet
            {
                Name = (HttpContext.Session.GetString("TempBouquetName") ?? "זר") + " (אורח: " + guestName + ")",
                Price = decimal.TryParse(HttpContext.Session.GetString("TempBouquetPrice"), out decimal p) ? p : 0,
                BouquetDesignHtml = HttpContext.Session.GetString("TempBouquetHtml"),
                ShippingAddress = guestAddress, // שמירת הכתובת של האורח
                CustomerPhone = guestPhone,     // שמירת הטלפון של האורח
                CreatedAt = DateTime.Now        // שעת היצירה המדויקת של האורח
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

        // 6. דף אישור סופי
        [HttpGet]
        public IActionResult OrderConfirmation(int id)
        {
            HttpContext.Session.Remove("TempBouquetName");
            HttpContext.Session.Remove("TempBouquetPrice");
            HttpContext.Session.Remove("TempBouquetHtml");
            HttpContext.Session.Remove("TempSelectedFlowers");

            ViewBag.OrderId = id;
            return View();
        }
    }
}