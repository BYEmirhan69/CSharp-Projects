namespace KeyboardUtils.Core.Models;

/// <summary>
/// Yazma pratiği oturumu
/// </summary>
public class TypingSession
{
    /// <summary>Benzersiz tanımlayıcı</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>Oturum tarihi</summary>
    public DateTime Date { get; set; } = DateTime.Now;
    
    /// <summary>Pratik yapılan metin</summary>
    public string TargetText { get; set; } = string.Empty;
    
    /// <summary>Kullanıcının yazdığı metin</summary>
    public string TypedText { get; set; } = string.Empty;
    
    /// <summary>Dakikadaki kelime sayısı (Words Per Minute)</summary>
    public double Wpm { get; set; }
    
    /// <summary>Doğruluk yüzdesi (0-100)</summary>
    public double Accuracy { get; set; }
    
    /// <summary>Süre (saniye)</summary>
    public double DurationSeconds { get; set; }
    
    /// <summary>Toplam tuş basımı</summary>
    public int TotalKeystrokes { get; set; }
    
    /// <summary>Hatalı tuş basımı</summary>
    public int ErrorCount { get; set; }
    
    /// <summary>Zorluk seviyesi</summary>
    public string Difficulty { get; set; } = "Normal";
    
    /// <summary>Oturum tamamlandı mı</summary>
    public bool IsCompleted { get; set; }
}

/// <summary>
/// Typing tutor için örnek metin
/// </summary>
public class PracticeText
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Normal";
    public string Category { get; set; } = "Genel";
}
