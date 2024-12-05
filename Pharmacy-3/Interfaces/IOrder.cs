namespace Pharmacy_3.Interfaces
{
	public interface IOrder
	{
		decimal? TotalPrice { get; }
		DateTime? OrderDate { get; }
		void PlaceOrder();
	}
}
