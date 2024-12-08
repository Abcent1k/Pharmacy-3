using Pharmacy_3.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Pharmacy_3.Models
{
	public class User : IdentityUser, IUser
	{
		public virtual ICollection<Order>? Orders { get; private set; }
		public virtual ICollection<InventoryProduct> Products { get; private set; }

		[Required(ErrorMessage = "The name is required")]
		[MaxLength(30, ErrorMessage = "The name cannot exceed 30 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "The surname is required")]
		[MaxLength(30, ErrorMessage = "The surname cannot exceed 64 characters")]
		public string Surname { get; set; }

		[EmailAddress(ErrorMessage = "Incorrect email format")]
		public override string? Email { get; set; }

		// Constructors
		public User(string name, string surname) : base()
		{
			Name = name;
			Surname = surname;
			Products = new List<InventoryProduct>();
			Orders = new List<Order>();
		}

		// Parameterless constructor required by Identity framework
		public User() : base()
		{
			Products = new List<InventoryProduct>();
			Orders = new List<Order>();
		}
	}
}
