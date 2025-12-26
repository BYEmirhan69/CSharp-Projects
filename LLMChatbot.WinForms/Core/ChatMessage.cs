namespace LLMChatbot.WinForms.Core;

/// <summary>
/// Sohbetteki tek bir mesajı temsil eden sınıf.
/// OpenAI API formatına uygun role ve content alanları içerir.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Mesajın rolü: "system", "user" veya "assistant"
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Mesaj içeriği
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Yeni bir ChatMessage oluşturur
    /// </summary>
    /// <param name="role">Mesajın rolü</param>
    /// <param name="content">Mesaj içeriği</param>
    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    /// <summary>
    /// Kullanıcı mesajı oluşturur
    /// </summary>
    public static ChatMessage User(string content) => new("user", content);

    /// <summary>
    /// Asistan (bot) mesajı oluşturur
    /// </summary>
    public static ChatMessage Assistant(string content) => new("assistant", content);

    /// <summary>
    /// Sistem mesajı oluşturur
    /// </summary>
    public static ChatMessage System(string content) => new("system", content);
}
