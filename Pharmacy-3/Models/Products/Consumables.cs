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
		[Required]
		public DateTime ExpirationDate { get; set; }
		[Required]
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
