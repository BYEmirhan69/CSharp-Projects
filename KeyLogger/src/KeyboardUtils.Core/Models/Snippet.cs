namespace KeyboardUtils.Core.Models;

/// <summary>
/// Snippet (kısaltma) tanımı
/// </summary>
public class Snippet
{
    /// <summary>Benzersiz tanımlayıcı</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>Tetikleyici metin (örn: "btw")</summary>
    public string Trigger { get; set; } = string.Empty;
    
    /// <summary>Genişletilmiş metin (örn: "by the way")</summary>
    public string Expansion { get; set; } = string.Empty;
    
    /// <summary>Açıklama</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Büyük/küçük harf duyarlı mı</summary>
    public bool CaseSensitive { get; set; } = false;
    
    /// <summary>Snippet aktif mi</summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>Oluşturulma tarihi</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>Kullanım sayısı</summary>
    public int UsageCount { get; set; } = 0;
}
