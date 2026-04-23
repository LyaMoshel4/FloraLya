using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LyaShop.Data;
using LyaShop.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace LyaShop.Controllers
{
    public class FlowersController : Controller
    {
        private readonly LyaFlowerShopContext _context;

        public FlowersController(LyaFlowerShopContext context)
        {
            _context = context;
        }

        // 1. INDEX - תצוגת רשימת הפרחים לניהול המלאי
        public async Task<IActionResult> Index()
        {
            var flowers = await _context.Flower.ToListAsync();
            return View(flowers);
        }

        // 2. CREATE (GET) - טופס יצירת פרח חדש
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST) - קליטת הטופס ושמירת התמונה והצבע
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Flower flower, IFormFile imageFile)
        {
            ModelState.Remove("imageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImagePath");

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                flower.ImageUrl = "/images/" + fileName;
                flower.ImagePath = "/images/" + fileName;
            }
            else
            {
                flower.ImageUrl = "https://via.placeholder.com/150";
            }

            _context.Add(flower);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. EDIT (GET) - עריכת פרח קיים
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var flower = await _context.Flower.FindAsync(id);
            if (flower == null) return NotFound();

            return View(flower);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Flower flower, IFormFile? imageFile)
        {
            if (id != flower.Id) return NotFound();

            ModelState.Remove("imageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("ImagePath");

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        flower.ImageUrl = "/images/" + fileName;
                    }

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