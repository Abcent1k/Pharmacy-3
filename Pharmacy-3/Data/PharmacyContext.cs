using Microsoft.EntityFrameworkCore;
using Pharmacy_3.Models;
using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Pharmacy_3.Data
{
	public class PharmacyContext : IdentityDbContext<User>
	{
		internal DbSet<User> Users => Set<User>();
		internal DbSet<Order> Orders => Set<Order>();
		internal DbSet<Product> Products => Set<Product>();
		internal DbSet<Drugs> Drugs => Set<Drugs>();
		internal DbSet<Devices> Devices => Set<Devices>();
		internal DbSet<Consumables> Consumables => Set<Consumables>();
		internal DbSet<InventoryProduct> InventoryProducts => Set<InventoryProduct>();

		public PharmacyContext(DbContextOptions<PharmacyContext> options): base(options) 
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<InventoryProduct>()
				.HasKey(ip => ip.Id);

			modelBuilder.Entity<Order>()
				.Property(o => o.TotalPrice)
				.HasPrecision(18, 2);

			// Налаштування для Product
			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Product>()
				.ToTable(t => t.HasCheckConstraint("EDRPOU", "LEN(EDRPOU) = 8"));

			// Налаштування для Consumables
			modelBuilder.Entity<Consumables>()
				.Property(c => c.ExpirationDate)
				.HasDefaultValue(DateTime.Now.AddYears(5));

			// Налаштування для Drugs
			modelBuilder.Entity<Drugs>()
				.Property(d => d.ExpirationDate)
				.HasDefaultValue(DateTime.Now.AddYears(1));

			modelBuilder.Entity<Drugs>()
				.Property(d => d.NeedRecipe)
				.HasDefaultValue(false);

			modelBuilder.Entity<Drugs>()
				.Property(d => d.DrugType)
				.HasConversion<string>();

			modelBuilder.Entity<Consumables>()
				.Property(c => c.ConsumableType)
				.HasConversion<string>();

			modelBuilder.Entity<Devices>()
				.Property(d => d.DeviceType)
				.HasConversion<string>();

			// Налаштування для TPH спадкування Product
			modelBuilder.Entity<Product>()
				.HasDiscriminator<string>("ProductType")
				.HasValue<Consumables>("Consumables")
				.HasValue<Devices>("Devices")
				.HasValue<Drugs>("Drugs");

			// Зв’язок між User і Order
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.OnDelete(DeleteBehavior.NoAction);

			// Зв’язок між Order і InventoryProduct
			modelBuilder.Entity<InventoryProduct>()
				.HasOne(ip => ip.Order)
				.WithMany(o => o.InventoryProducts)
				.OnDelete(DeleteBehavior.NoAction);

			// Зв’язок між User і InventoryProduct
			modelBuilder.Entity<InventoryProduct>()
				.HasOne(ip => ip.User)
				.WithMany(u => u.Products)
				.OnDelete(DeleteBehavior.NoAction);

			// Зв’язок між Product і InventoryProduct
			modelBuilder.Entity<InventoryProduct>()
				.HasOne(ip => ip.Product)
				.WithMany(p => p.InventoryProducts)
				.HasForeignKey(ip => ip.ProductUPC);
		}

	}
}
