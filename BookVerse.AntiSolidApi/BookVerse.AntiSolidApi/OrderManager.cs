public class OrderManager
{
    public void ProcessOrder(Order order)
    {
        Console.WriteLine("Sipariþ veritabanýna kaydediliyor...");
        if (order.PaymentMethod == "CreditCard")
            Console.WriteLine("Kredi kartý ile ödeme iþleniyor...");
        else if (order.PaymentMethod == "PayPal")
            Console.WriteLine("PayPal ile ödeme iþleniyor...");

        Console.WriteLine("PDF formatýnda fatura oluþturuluyor...");
        var emailService = new SmtpEmailService();
        emailService.Send("customer@example.com", "Sipariþiniz alýndý");
        Console.WriteLine("Sipariþ baþarýyla iþlendi.");
    }
}
