using Microsoft.EntityFrameworkCore;

namespace DatabaseManager
{
    public class DatabaseContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи Employee - Division (многие к одному)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Division)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DivisionId)
                .OnDelete(DeleteBehavior.NoAction); // Устанавливаем "ON DELETE NO ACTION"

            // Настройка связи Employee - Post (многие к одному)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Post)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.NoAction); // Устанавливаем "ON DELETE NO ACTION"

            modelBuilder.Entity<Counterpartie>(entity =>
            {
                entity.Property(c => c.Type)
                    .HasConversion<string>() // Хранение значения перечисления как текста
                    .HasDefaultValue(CounterpartieType.Fiz) // Значение по умолчанию
                    .HasColumnType("TEXT");

                // Используем ToTable для добавления CHECK-ограничения  
                entity.ToTable(t => t.HasCheckConstraint("CK_Customer_Type", "Type IN ('Fiz', 'Ur')"));
            });

            // Employee - Location (один ко многим)
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unit - Product (один ко многим)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unit - Material (один ко многим)
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Unit)
                .WithMany(u => u.Materials)
                .HasForeignKey(m => m.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            // Location - Product (один ко многим)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Location)
                .WithMany(l => l.Products)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Location - Material (один ко многим)
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Location)
                .WithMany(l => l.Materials)
                .HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.NoAction);

            // Product - Material (многие ко многим)
            modelBuilder.Entity<Product_Material>()
                .HasKey(pm => new { pm.Id });

            modelBuilder.Entity<Product_Material>()
                .HasOne(pm => pm.Product)
                .WithMany(p => p.ProductMaterials)
                .HasForeignKey(pm => pm.ProductId);

            modelBuilder.Entity<Product_Material>()
                .HasOne(pm => pm.Material)
                .WithMany(m => m.ProductMaterials)
                .HasForeignKey(pm => pm.MaterialId);

            // Counterparty - Contract (один ко многим)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Counterpartie)
                .WithMany(cp => cp.Contracts)
                .HasForeignKey(c => c.CounterpartyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Counterparty - Receipt (один ко многим)
            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.Counterpartie)
                .WithMany(cp => cp.Receipts)
                .HasForeignKey(r => r.CounterpartyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Product - Contract (один ко многим)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Contracts)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // Contract - Sale (один ко многим)
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.ContractId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
