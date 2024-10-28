using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdTracker
{
    class ApplicationContext : DbContext
    {
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Receipt> Receipts { get; set; } = null!;
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<Division> Divisions { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Location> Locations { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<Counterpartie> Counterparties { get; set; } = null!;
        public DbSet<Product_Material> Product_Materials { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=VacDB.db");
        }
    }
}
