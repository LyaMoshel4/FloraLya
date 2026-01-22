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
            var bouquets = await _context.Bouquet
                .Include(b => b.FlowersInBouquet)
                .ThenInclude(fb => fb.Flower)
                .ToListAsync();
            return View(bouquets);
        }

        // 2. פתיחת דף העיצוב (GET: Create)
        public async Task<IActionResult> Create()
        {
            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View();
        }

        // 3. שמירת הזר והקשרים לפרחים (POST: Create)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // הוספתי כאן את BouquetDesignHtml לתוך ה-Bind כדי שהשרת יסכים לקבל אותו מהטופס
        public async Task<IActionResult> Create([Bind("Id,Name,Price,BouquetDesignHtml")] Bouquet bouquet, int[] selectedFlowers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bouquet);
                await _context.SaveChangesAsync();

                if (selectedFlowers != null && selectedFlowers.Length > 0)
                {
                    foreach (var flowerId in selectedFlowers)
                    {
                        var flowerInBouquet = new FlowerInBouquet
                        {
                            BouquetId = bouquet.Id,
                            FlowerId = flowerId,
                            Quantity = 1
                        };
                        _context.FlowerInBouquet.Add(flowerInBouquet);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Flowers = await _context.Flower.ToListAsync();
            return View(bouquet);
        }

        // 4. עריכת זר (GET: Edit)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet == null) return NotFound();

            return View(bouquet);
        }

        // 5. שמירת עריכה (POST: Edit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // גם כאן הוספתי את BouquetDesignHtml למקרה שתערכי את שם הזר, שהעיצוב לא יימחק
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,BouquetDesignHtml")] Bouquet bouquet)
        {
            if (id != bouquet.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(bouquet);
        }

        // 6. מחיקה ישירה מהגלריה (POST: Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var bouquet = await _context.Bouquet.FindAsync(id);
            if (bouquet != null)
            {
                _context.Bouquet.Remove(bouquet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BouquetExists(int id)
        {
            return _context.Bouquet.Any(e => e.Id == id);
        }
    }
}