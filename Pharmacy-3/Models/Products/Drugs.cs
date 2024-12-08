using Pharmacy_3.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy_3.Models.Products
{
	public enum DrugType
    {
        Pills,
        Spray,
        Drops,
        Syrop,
        Inhalation_solution,
        Injection_solution,
    }
	public class Drugs : Product, IExpiration, IProductFormat
    {
		[Required(ErrorMessage = "The expiration date is required")]
		public DateTime ExpirationDate { get; set; }
		[Required(ErrorMessage = "The type of drug is required")]
		public DrugType DrugType { get; set; }
		[Required(ErrorMessage = "This field is required")]
		public bool NeedRecipe { get; set; }

        public Drugs(uint uPC,
                     string name,
                     decimal price,
                     uint eDRPOU,
                     DateTime expirationDate,
                     DrugType drugType,
                     bool needRecipe) : base(uPC, name, price, eDRPOU)
        {
			ExpirationDate = expirationDate;
			DrugType = drugType;
			NeedRecipe = needRecipe;
        }
		public Drugs(uint uPC,
			 string name,
			 decimal price,
			 uint eDRPOU,
			 DrugType drugType,
			 bool needRecipe) : base(uPC, name, price, eDRPOU)
		{
			DrugType = drugType;
			NeedRecipe = needRecipe;
		}

		public override void Show()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            return $"{DrugType}\nName: {Name}\nNeed a recipe: {NeedRecipe}\nPrice: {Price}";
        }

        void IProductFormat.Show()
        {
            Console.WriteLine(((IProductFormat)this).ToString());
        }

        string IProductFormat.ToString()
        {
            return $"{DrugType}; Name: {Name}; Need a recipe: {NeedRecipe}; Price: {Price}";
        }
    }
}
