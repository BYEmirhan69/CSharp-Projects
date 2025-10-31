public class SmtpEmailService
{
    public void Send(string to, string body)
    {
        Console.WriteLine($"'{to}' adresine e-posta gönderildi: {body}");
    }
}
