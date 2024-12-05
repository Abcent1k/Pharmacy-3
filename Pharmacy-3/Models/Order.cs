using Pharmacy_3.Models.Products;
using Pharmacy_3.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_3.Models
{

	public class Order: IOrder
	{
		public delegate void OrderHandler(string message);
		public event OrderHandler? Notify;

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[Required]
		public int UserId { get; set; }
		public virtual User User { get; set; }
		[Required]
		public virtual ICollection<InventoryProduct> InventoryProducts { get; private set; }
		public decimal? TotalPrice { get; private set; }
		public DateTime? OrderDate { get; set; }

		public Order()
		{
			InventoryProducts = new List<InventoryProduct>();
		}
		public Order(User user)
		{
			InventoryProducts = new List<InventoryProduct>();
			this.User = user;
			UserId = user.UserId;
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

		void AddProduct(InventoryProduct inv_product)
		{
			var index = (new List<InventoryProduct>(User.Products)).FindIndex(s => s.Product == inv_product.Product);

			if (index >= 0)
			{
				(new List<InventoryProduct>(User.Products))[index] =
					new InventoryProduct(
						User,
						inv_product.Product,
						(new List<InventoryProduct>(User.Products))[index].Quantity + inv_product.Quantity,
						this
						);
			}
			else
			{
				User.Products.Add(inv_product);
			}
		}
		void AddProduct(Product product)
		{
			var index = (new List<InventoryProduct>(User.Products)).FindIndex(s => s.Product == product);

			if (index >= 0)
			{
				(new List<InventoryProduct>(User.Products))[index] =
					new InventoryProduct(
						User,
						product,
						(new List<InventoryProduct>(User.Products))[index].Quantity + 1,
						this);
			}
			else
			{
				User.Products.Add(new InventoryProduct(User, product, 1, this));
			}
		}
		public void RemoveProduct(Product product)
		{
			var index = (new List<InventoryProduct>(User.Products)).FindIndex(s => s.Product == product);
			if (index >= 0)
			{
				if ((new List<InventoryProduct>(User.Products))[index].Quantity > 1)
				{
					(new List<InventoryProduct>(User.Products))[index] = 
						new InventoryProduct(User, product, (new List<InventoryProduct>(User.Products))[index].Quantity - 1, this);
				}
				else
				{
					(new List<InventoryProduct>(User.Products)).RemoveAt(index);
				}
			}
			else
			{
				throw new InvalidOperationException($"{product.Name} are not in this cart");
			}
		}
		public void RemoveProduct(InventoryProduct inv_product)
		{
			var index = (new List<InventoryProduct>(User.Products)).FindIndex(s => s.Product == inv_product.Product);
			if (index >= 0)
			{
				if ((new List<InventoryProduct>(User.Products))[index].Quantity > inv_product.Quantity)
				{
					(new List<InventoryProduct>(User.Products))[index] = 
						new InventoryProduct(
							User, 
							inv_product.Product,
							(new List<InventoryProduct>(User.Products))[index].Quantity - inv_product.Quantity,
							this);
				}
				else
				{
					(new List<InventoryProduct>(User.Products)).RemoveAt(index);
				}
			}
			else
			{
				throw new InvalidOperationException($"{inv_product.Product.Name} are not in this cart");
			}
		}
		public void ShowCart()
		{
			foreach (var item in User.Products)
			{
				Console.WriteLine($"Product Name - {item.Product.Name}; " +
								  $"Quantity - {item.Quantity}; Unit price - {item.Product.Price}");
			}
		}
		public void RemoveAll()
		{
			User.Products.Clear();
		}
	}
}
