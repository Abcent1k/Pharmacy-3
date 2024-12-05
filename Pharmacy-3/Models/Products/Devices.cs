using System.ComponentModel.DataAnnotations;

namespace Pharmacy_3.Models.Products
{
	public enum DeviceType
    {
        Inhaler,
        Pulse_oximetr,
        Blood_pressure_monitor,
    }
	public class Devices : Product
    {
		[Required]
		public DeviceType DeviceType { get; set; }
        public Devices(uint uPC,
                     string name,
                     decimal price,
                     uint eDRPOU,
                     DeviceType deviceType) : base(uPC, name, price, eDRPOU)
        {
			DeviceType = deviceType;
        }

        public override void Show()
        {
            Console.WriteLine(ToString());
        }
        public override string ToString()
        {
            return $"{DeviceType}\nName: {Name}\nPrice: {Price}";
        }
    }
}
