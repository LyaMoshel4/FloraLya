using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;

namespace LyaShop.Controllers
{
    public class FlowersController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public FlowersController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // 1. INDEX - תצוגת רשימת הפרחים
        public async Task<IActionResult> Index()
        {
            var flowers = await _context.Flower.ToListAsync();
            return View(flowers);
        }

        // 2. CREATE - יצירת פרח חדש
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flower flower)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flower);
        }

        // 3. EDIT - עריכת פרח קיים
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var flower = await _context.Flower.FindAsync(id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flower flower)
        {
            if (id != flower.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Flower.Any(e => e.Id == flower.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flower);
        }

        // 4. DELETE - הצגת דף אישור מחיקה
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var flower = await _context.Flower.FirstOrDefaultAsync(m => m.Id == id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        // אישור מחיקה סופי
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flower = await _context.Flower.FindAsync(id);
            if (flower != null)
            {
                _context.Flower.Remove(flower);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}