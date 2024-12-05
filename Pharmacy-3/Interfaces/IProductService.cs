using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy_3.Models.Products;

namespace Pharmacy_3.Interfaces
{
	public interface IProductService
	{
		Task<IEnumerable<Product>> GetAllProductsAsync();
		Task<Product> GetProductByIdAsync(uint id);
		Task AddProductAsync(Product product);
		Task UpdateProductAsync(Product product);
		Task DeleteProductAsync(uint id);
		Task<bool> ProductExistsAsync(uint id);
	}
}
