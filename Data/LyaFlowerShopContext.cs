using Microsoft.EntityFrameworkCore;
using LyaShop.Models;

namespace LyaShop.Data
{
    public class LyaFlowerShopContext : DbContext
    {
        public LyaFlowerShopContext(DbContextOptions<LyaFlowerShopContext> options)
            : base(options)
        {
        }

        public DbSet<Flower> Flower { get; set; }
        public DbSet<Bouquet> Bouquet { get; set; }
        public DbSet<FlowerInBouquet> FlowerInBouquet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // הגדרת מפתח ראשי לטבלה המקשרת
            modelBuilder.Entity<FlowerInBouquet>()
                .HasKey(fb => fb.Id);
        }
    }
}