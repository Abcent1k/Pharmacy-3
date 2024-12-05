namespace Pharmacy_3.Interfaces
{
	public interface IExpiration
	{
		DateTime ExpirationDate { get; }
	}

	public interface IProduct
	{
		uint UPC { get; set; }
		string Name { get; }
		decimal Price { get; }
		uint EDRPOU { get; }
		void Show();
		string ToString();
	}

	public interface IProductFormat
	{
		void Show();
		string ToString();
	}
}
