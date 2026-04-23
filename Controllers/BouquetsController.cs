using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LyaShop.Controllers
{
    public class BouquetsController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public BouquetsController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // 1. דף ניהול - הגנה: רק מנהל רואה את רשימת כל הזרים
        public async Task<IActionResult> Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            // אם לא מנהל - זורק אותו לדף הבית
            if (!isAdmin)
                return RedirectToAction("Index", "Home");

            return View(await _context.Bouquet.ToListAsync());
        }

        // 2. דף המעצב (Designer)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View();
        }

        // 3. שמירת הזר
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bouquet bouquet, int[] selectedFlowers)
        {
            HttpContext.Session.SetString("TempBouquetName", bouquet.Name ?? "זר בעיצוב אישי");
            HttpContext.Session.SetString("TempBouquetPrice", bouquet.Price.ToString());
            HttpContext.Session.SetString("TempBouquetHtml", bouquet.BouquetDesignHtml ?? "");

            if (selectedFlowers != null)
                HttpContext.Session.SetString("TempSelectedFlowers", JsonSerializer.Serialize(selectedFlowers));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CustomerName")))
            {
                return RedirectToAction("GuestDetails");
            }

            if (ModelState.IsValid)
            {
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
                BouquetDesignHtml = HttpContext.Session.GetString("TempBouquetHtml")
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