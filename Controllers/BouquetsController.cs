using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;

namespace LyaShop.Controllers
{
    public class BouquetsController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public BouquetsController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // 1. תצוגת כל הזרים שנוצרו (Index)
        public async Task<IActionResult> Index()
        {
            // כולל את הפרחים שנמצאים בתוך כל זר כדי שנוכל להציג אותם
            var bouquets = await _context.Bouquet
                .Include(b => b.FlowersInBouquet)
                .ThenInclude(fb => fb.Flower)
                .ToListAsync();
            return View(bouquets);
        }

        // 2. פתיחת דף העיצוב (GET: Create)
        public async Task<IActionResult> Create()
        {
            // שליחת רשימת הפרחים מהמלאי כדי שיופיעו בתיבת הבחירה ב-Canvas
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View();
        }

        // 3. שמירת הזר והקשרים לפרחים (POST: Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bouquet bouquet, int[] selectedFlowers)
        {
            if (ModelState.IsValid)
            {
                // שמירת הזר עצמו
                _context.Add(bouquet);
                await _context.SaveChangesAsync();

                // שמירת הפרחים שנבחרו לתוך טבלת הקשר (FlowerInBouquet)
                if (selectedFlowers != null && selectedFlowers.Length > 0)
                {
                    foreach (var flowerId in selectedFlowers)
                    {
                        var flowerInBouquet = new FlowerInBouquet
                        {
                            BouquetId = bouquet.Id, // ה-ID שנוצר הרגע
                            FlowerId = flowerId,
                            Quantity = 1
                        };
                        _context.FlowerInBouquet.Add(flowerInBouquet);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // אם הטופס לא תקין, נשלח שוב את רשימת הפרחים כדי שהדף לא יישבר
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View(bouquet);
        }

        // 4. מחיקת זר
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bouquet = await _context.Bouquet
                .FirstOrDefaultAsync(m => m.Id == id);

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
                _context.Bouquet.Remove(bouquet);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}