using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_3.Models
{
	public class Order : IOrder
	{
		public delegate void OrderHandler(string message);
		public event OrderHandler? Notify;

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public virtual User User { get; set; }

		[Required]
		public virtual ICollection<InventoryProduct> InventoryProducts { get; private set; }

		[Column(TypeName = "decimal(18, 2)")]
		public decimal? TotalPrice { get; private set; }

		public DateTime? OrderDate { get; set; }

		public Order()
		{
			InventoryProducts = new List<InventoryProduct>();
		}

		public void PlaceOrder()
		{
			if (User == null)
				throw new InvalidOperationException();
			OrderDate = DateTime.Now;
			TotalPrice = CalculateTotalPrice();
			User.Orders.Add(this);
			Notify?.Invoke($"Замовлення на сумму {TotalPrice} від {OrderDate}\n" +
						   $"Замовник: {User.Name} {User.Surname}");
		}

		private decimal CalculateTotalPrice()
		{
			decimal totalPrice = 0;
			foreach (var inv_product in User.Products)
			{
				totalPrice += inv_product.Quantity * inv_product.Product.Price;
			}
			return totalPrice;
		}
	}
}
