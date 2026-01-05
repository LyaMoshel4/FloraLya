using Microsoft.EntityFrameworkCore;
using LyaFlowerShop.Models;

namespace LyaShop.Data // וודאי שזה ה-Namespace הנכון לפי התמונות שלך
{
    public class LyaFlowerShopContext : DbContext
    {
        // חובה: ה-Constructor שמקבל Options ומעביר אותם לבסיס (base)
        public LyaFlowerShopContext(DbContextOptions<LyaFlowerShopContext> options)
            : base(options)
        {
        }

        public DbSet<Flower> Flower { get; set; }
        public DbSet<Bouquet> Bouquet { get; set; }
        public DbSet<FlowerInBouquet> FlowerInBouquet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // הגדרת קשרים בסיסית
            modelBuilder.Entity<FlowerInBouquet>()
                .HasKey(fb => fb.Id);
        }
    }
}