namespace LLMChatbot.WinForms.Core;

/// <summary>
/// Sohbet geçmişini yöneten sınıf.
/// Multi-turn konuşmalar için mesaj listesini RAM'de tutar.
/// </summary>
public class Conversation
{
    /// <summary>
    /// Varsayılan sistem mesajı
    /// </summary>
    private const string DefaultSystemMessage = 
        "Sen kısa, net ve teknik cevaplar veren yardımcı bir asistansın.";

    /// <summary>
    /// Sohbet geçmişindeki mesajlar
    /// </summary>
    private readonly List<ChatMessage> _messages;

    /// <summary>
    /// Yeni bir Conversation başlatır ve varsayılan sistem mesajını ekler
    /// </summary>
    public Conversation()
    {
        _messages = new List<ChatMessage>
        {
            ChatMessage.System(DefaultSystemMessage)
        };
    }

    /// <summary>
    /// Özel bir sistem mesajı ile Conversation başlatır
    /// </summary>
    /// <param name="systemMessage">Özel sistem mesajı</param>
    public Conversation(string systemMessage)
    {
        _messages = new List<ChatMessage>
        {
            ChatMessage.System(systemMessage)
        };
    }

    /// <summary>
    /// Sohbete yeni bir mesaj ekler
    /// </summary>
    /// <param name="message">Eklenecek mesaj</param>
    public void AddMessage(ChatMessage message)
    {
        _messages.Add(message);
    }

    /// <summary>
    /// Kullanıcı mesajı ekler
    /// </summary>
    /// <param name="content">Mesaj içeriği</param>
    public void AddUserMessage(string content)
    {
        _messages.Add(ChatMessage.User(content));
    }

    /// <summary>
    /// Asistan mesajı ekler
    /// </summary>
    /// <param name="content">Mesaj içeriği</param>
    public void AddAssistantMessage(string content)
    {
        _messages.Add(ChatMessage.Assistant(content));
    }

    /// <summary>
    /// Tüm sohbet geçmişini döndürür (salt okunur)
    /// </summary>
    /// <returns>Mesaj listesi</returns>
    public IReadOnlyList<ChatMessage> GetHistory()
    {
        return _messages.AsReadOnly();
    }

    /// <summary>
    /// API isteği için mesaj listesini döndürür
    /// </summary>
    /// <returns>API formatında mesaj listesi</returns>
    public List<object> GetMessagesForApi()
    {
        return _messages.Select(m => new { role = m.Role, content = m.Content })
                        .Cast<object>()
                        .ToList();
    }

    /// <summary>
    /// Sohbet geçmişini temizler ve yeni bir konuşma başlatır
    /// </summary>
    public void Clear()
    {
        _messages.Clear();
        _messages.Add(ChatMessage.System(DefaultSystemMessage));
    }

    /// <summary>
    /// Sohbetteki mesaj sayısı (sistem mesajı dahil)
    /// </summary>
    public int MessageCount => _messages.Count;
}
