using Pharmacy_3.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_3.Models.Products
{
    public abstract class Product : IProduct
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Required(ErrorMessage = "The UPC is required")]
		public uint UPC { get; set; }
		[Required(ErrorMessage = "The name is required")]
		[MaxLength(64, ErrorMessage = "The name cannot exceed 64 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "The price is required")]
		[Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")]
		public decimal Price { get; set; }
		public virtual ICollection<InventoryProduct> InventoryProducts { get; set; }
		[Required(ErrorMessage = "EDRPOU is required")]
		[Range(10000000, 99999999, ErrorMessage = "EDRPOU must be in a range 10000000-99999999")]
		public uint EDRPOU { get; private set; }

        public Product(uint uPC, string name, decimal price, uint eDRPOU)
        {
            UPC = uPC;
            Name = name;
            Price = price;
			EDRPOU = eDRPOU;
			InventoryProducts = new List<InventoryProduct>();
		}

		public abstract void Show();
        public abstract override string ToString();
    }
}
