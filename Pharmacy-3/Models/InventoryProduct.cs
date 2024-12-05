using Pharmacy_3.Models.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_3.Models
{
	public class InventoryProduct
	{
		[Required]
		public int OrderId { get; set; }
		public virtual Order Order { get; set; }
		[Required]
		public int UserId { get; set; }
		public virtual User User { get; private set; }
		public virtual Product Product { get; set; }
		[Required]
		public uint ProductUPC { get; set; }
		[Required]
		public int Quantity { get; set; }

		public InventoryProduct(int quantity)
		{
			Quantity = quantity;
		}

		public InventoryProduct(
			User user, 
			Product product, 
			int quantity, 
			Order order)
		{
			User = user;
			UserId = user.UserId;
			Quantity = quantity;
			Product = product;
			ProductUPC = product.UPC;
			Order = order;
			OrderId = order.Id;
		}
	}
}
