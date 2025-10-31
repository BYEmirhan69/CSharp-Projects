public class Order
{
	public int OrderId { get; set; }
	public decimal TotalAmount { get; set; }
	public string PaymentMethod { get; set; } // "CreditCard" veya "PayPal"
}
