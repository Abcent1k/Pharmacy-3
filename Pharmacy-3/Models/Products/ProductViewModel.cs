using System.ComponentModel.DataAnnotations;

namespace Pharmacy_3.Models.Products
{
	public class ProductViewModel
	{
		public string ProductType { get; set; }

		public uint UPC { get; set; }

		[Required]
		[MaxLength(64)]
		public string Name { get; set; }

		[Required]
		public decimal Price { get; set; }

		[Required]
		public uint EDRPOU { get; set; }

		public DateTime? ExpirationDate { get; set; }
		public ConsumableType? ConsumableType { get; set; }
		public DeviceType? DeviceType { get; set; }
		public DrugType? DrugType { get; set; }
		public bool NeedRecipe { get; set; }
	}
}
