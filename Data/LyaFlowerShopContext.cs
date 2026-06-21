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
        public DbSet<Customer> Customers { get; set; }

       
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlowerInBouquet>()
                .HasKey(fb => fb.Id);
        }
    }
}