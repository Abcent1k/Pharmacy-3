using Pharmacy_3.Models.Products;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_3.Models
{
	public class InventoryProduct
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }

		public virtual Order Order { get; set; }

		public virtual User User { get; set; }

		public virtual Product Product { get; set; }

		[Required]
		public uint ProductUPC { get; set; }

		[Required]
		public int Quantity { get; set; }

		public InventoryProduct(int quantity)
		{
			Quantity = quantity;
		}

		public InventoryProduct(User user, Product product, int quantity, Order order)
		{
			User = user;
			Quantity = quantity;
			Product = product;
			ProductUPC = product.UPC;
			Order = order;
			OrderId = order.Id;
		}
	}
}
