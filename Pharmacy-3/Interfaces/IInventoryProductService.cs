using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy_3.Models;
using Pharmacy_3.Models.Products;

namespace Pharmacy_3.Interfaces
{
	public interface IInventoryProductService
	{
		Task<IEnumerable<InventoryProduct>> GetAllInventoryProductsAsync();
		Task<InventoryProduct> GetInventoryProductByIdAsync(int id);
		Task AddInventoryProductAsync(InventoryProduct inventoryProduct);
		Task UpdateInventoryProductAsync(InventoryProduct inventoryProduct);
		Task DeleteInventoryProductAsync(int id);
	}
}
