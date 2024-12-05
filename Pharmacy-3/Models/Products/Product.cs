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
		public uint UPC { get; set; }
		[Required]
		[MaxLength(64)]
		public string Name { get; set; }
		[Required]
		public decimal Price { get; set; }
		public virtual ICollection<InventoryProduct> InventoryProducts { get; set; }
		[Required]
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
