using E_Commerce_System.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_System
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.userEmail)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.productName)
                .IsUnique();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProducts> OrdersProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
