using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Pharmacy_3.Models
{
	public class User: IUser
	{
		public int UserId { get; }
		public virtual ICollection<Order>? Orders { get; }
		public virtual ICollection<InventoryProduct> Products { get; }
		[MaxLength(30)]
		public string Name { get; private set; }
		[MaxLength(30)]
		public string Surname { get; private set; }
		public string? Email { get; set; }

		public User(string name, string surname)
		{
			Name = name;
			Surname = surname;
			Products = new List<InventoryProduct>();
			Orders = new List<Order>();
		}
		public User(string name, string surname, int userId):this(name, surname)
		{
			UserId = userId;
		}
		public User(string name, string surname, int userId, string email):
			this(name, surname, userId)
		{
			Email = email;
		}

	}
}
