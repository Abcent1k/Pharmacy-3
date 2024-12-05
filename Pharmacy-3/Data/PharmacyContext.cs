using Microsoft.EntityFrameworkCore;
using Pharmacy_3.Models;
using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;

namespace Pharmacy_3.Data
{
	public class PharmacyContext : DbContext
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
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasKey(u => u.UserId);

			modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
			modelBuilder.Entity<Product>().ToTable(t => t.HasCheckConstraint("EDRPOU", "LEN(EDRPOU) = 8"));

			modelBuilder.Entity<Consumables>()
				.Property(c => c.ExpirationDate)
				.HasDefaultValue(DateTime.Now.AddYears(5));
			modelBuilder.Entity<Drugs>()
				.Property(c => c.ExpirationDate)
				.HasDefaultValue(DateTime.Now.AddYears(1));
			modelBuilder.Entity<Drugs>()
				.Property(c => c.NeedRecipe)
				.HasDefaultValue(false);

			modelBuilder.Entity<Drugs>().Property(p => p.DrugType).HasConversion<string>();
			modelBuilder.Entity<Consumables>().Property(p => p.ConsumableType).HasConversion<string>();
			modelBuilder.Entity<Devices>().Property(p => p.DeviceType).HasConversion<string>();

			modelBuilder.Entity<Order>().HasKey(o => new { o.Id, o.UserId });
			modelBuilder.Entity<Order>().Property(o => o.TotalPrice).HasPrecision(18, 2);

			modelBuilder.Entity<InventoryProduct>().HasKey(p => new { p.OrderId, p.UserId, p.ProductUPC });

			// Configure TPH inheritance for Product and its subclasses
			modelBuilder.Entity<Product>()
				.HasDiscriminator<string>("ProductType")
				.HasValue<Consumables>("Consumables")
				.HasValue<Devices>("Devices")
				.HasValue<Drugs>("Drugs");

			// Configure one-to-many relationship between User and Order
			modelBuilder.Entity<User>()
				.HasMany(u => u.Orders)
				.WithOne(o => o.User)
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.NoAction); // Disable cascade delete

			// Configure one-to-many relationship between Order and InventoryProduct
			modelBuilder.Entity<Order>()
				.HasMany(o => o.InventoryProducts)
				.WithOne(ip => ip.Order)
				.HasForeignKey(ip => new { ip.OrderId, ip.UserId })
				.OnDelete(DeleteBehavior.NoAction); // Disable cascade delete

			// Configure one-to-many relationship between Product and InventoryProduct and
			modelBuilder.Entity<InventoryProduct>()
				.HasOne(ip => ip.Product)
				.WithMany(p => p.InventoryProducts)
				.HasForeignKey(ip => ip.ProductUPC);
		}
	}
}
