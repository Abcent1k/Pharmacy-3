using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy_3.Models;
using Pharmacy_3.Models.Products;

namespace Pharmacy_3.Interfaces
{
	public interface IOrderService
	{
		Task<IEnumerable<Order>> GetAllOrdersAsync();
		Task<Order> GetOrderByIdAsync(int id);
		Task AddOrderAsync(Order order);
		Task UpdateOrderAsync(Order order);
		Task DeleteOrderAsync(int id);
	}
}
