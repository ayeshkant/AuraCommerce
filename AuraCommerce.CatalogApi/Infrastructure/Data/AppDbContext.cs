using AuraCommerce.CatalogApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuraCommerce.CatalogApi.Infrastructure.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public AppDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-R8MBUDMG;Database=MyShop;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p=>p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();
            modelBuilder.Entity<Product>().Property(p => p.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Product>().HasCheckConstraint("Ck_Price", "[Price]>=0");
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            List<Product> seedData = new List<Product>
            {
                new Product
                {
                    Name="Laptop",Price=56000,Id=1,SKU="Lap101",CategoryId=1
                },
                new Product
                {
                    Name="Keyboard",Price=2000,Id=2,SKU="Key101",CategoryId=2
                },
                new Product
                {
                    Name="Mouse",Price=900,Id=3,SKU="Mou101",CategoryId=2
                }
            };
            List<Category> categoriesData = new List<Category>
            {
                new Category
                {
                    Id=1,
                    Name="Computers"
                },
                new Category
                {
                    Id=2,
                    Name="Accessories"
                }
            };
            modelBuilder.Entity<Product>().HasQueryFilter(p=>!p.IsDeleted).HasData(seedData);
            modelBuilder.Entity<Category>().HasData(categoriesData);
        }
    }
}
