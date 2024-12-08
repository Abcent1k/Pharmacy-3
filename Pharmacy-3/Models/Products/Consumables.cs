using Pharmacy_3.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_3.Models.Products
{
	public enum ConsumableType
    {
        Patch,
        Syringe,
        Needle,
        Gauze
    }

	public class Consumables : Product, IExpiration
    {
		[Required(ErrorMessage = "The expiration date is required")]
		public DateTime ExpirationDate { get; set; }
		[Required(ErrorMessage = "The type of consumable is required")]
		public ConsumableType ConsumableType { get; set; }

        public Consumables(uint uPC,
                     string name,
                     decimal price,
                     uint eDRPOU,
                     DateTime expirationDate,
                     ConsumableType consumableType) : base(uPC, name, price, eDRPOU)
        {
			ExpirationDate = expirationDate;
			ConsumableType = consumableType;
        }
		public Consumables(uint uPC,
			 string name,
			 decimal price,
			 uint eDRPOU,
			 ConsumableType consumableType) : base(uPC, name, price, eDRPOU)
		{
			ConsumableType = consumableType;
		}

		public override void Show()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            return $"{ConsumableType}\nName: {Name}\nPrice: {Price}";
        }
    }
}
