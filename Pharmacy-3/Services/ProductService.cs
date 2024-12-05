using Microsoft.EntityFrameworkCore;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models.Products;
using Pharmacy_3.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy_3.Services
{
	public class ProductService : IProductService
	{
		private readonly PharmacyContext _pharmacyContext;

		public ProductService(PharmacyContext pharmacyContext)
		{
			_pharmacyContext = pharmacyContext;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _pharmacyContext.Products.ToListAsync();
		}

		public async Task<Product> GetProductByIdAsync(uint id)
		{
			return await _pharmacyContext.Products.FindAsync(id);
		}

		public async Task AddProductAsync(Product product)
		{
			var existingProduct = await _pharmacyContext.Products.FindAsync(product.UPC);
			if (existingProduct != null)
			{
				throw new InvalidOperationException($"Product with UPC {product.UPC} already exists.");
			}

			_pharmacyContext.Products.Add(product);
			await _pharmacyContext.SaveChangesAsync();
		}

		public async Task UpdateProductAsync(Product product)
		{
			var existingProduct = await _pharmacyContext.Products.FindAsync(product.UPC);
			if (existingProduct == null)
			{
				throw new InvalidOperationException($"Product with UPC {product.UPC} does not exist.");
			}

			existingProduct.Name = product.Name;
			
			if (product.Price != existingProduct.Price)
			{
				typeof(Product).GetProperty("Price")?.SetValue(existingProduct, product.Price);
			}

			_pharmacyContext.Products.Update(existingProduct);
			await _pharmacyContext.SaveChangesAsync();
		}

		public async Task DeleteProductAsync(uint id)
		{
			var product = await _pharmacyContext.Products.FindAsync(id);
			if (product == null)
			{
				throw new InvalidOperationException($"Product with UPC {id} does not exist.");
			}

			_pharmacyContext.Products.Remove(product);
			await _pharmacyContext.SaveChangesAsync();
		}

		public async Task<bool> ProductExistsAsync(uint id)
		{
			return await _pharmacyContext.Products.AnyAsync(p => p.UPC == id);
		}
	}
}
