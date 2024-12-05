using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy_3.Models;
using Pharmacy_3.Models.Products;

namespace Pharmacy_3.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task<User> GetUserByIdAsync(int id);
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task DeleteUserAsync(int id);
	}
}
