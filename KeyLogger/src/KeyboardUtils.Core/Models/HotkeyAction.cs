namespace KeyboardUtils.Core.Models;

/// <summary>
/// Hotkey aksiyonu için eylem tipi
/// </summary>
public enum ActionType
{
    /// <summary>Uygulama çalıştır</summary>
    RunApplication,
    /// <summary>URL aç</summary>
    OpenUrl,
    /// <summary>Metin yaz</summary>
    TypeText,
    /// <summary>Komut çalıştır</summary>
    RunCommand
}

/// <summary>
/// Global hotkey ve ilişkili aksiyon tanımı
/// </summary>
public class HotkeyAction
{
    /// <summary>Benzersiz tanımlayıcı</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>Hotkey açıklaması</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Ana tuş (Keys enum değeri)</summary>
    public int Key { get; set; }
    
    /// <summary>Ctrl tuşu gerekli mi</summary>
    public bool Ctrl { get; set; }
    
    /// <summary>Alt tuşu gerekli mi</summary>
    public bool Alt { get; set; }
    
    /// <summary>Shift tuşu gerekli mi</summary>
    public bool Shift { get; set; }
    
    /// <summary>Windows tuşu gerekli mi</summary>
    public bool Win { get; set; }
    
    /// <summary>Aksiyon tipi</summary>
    public ActionType ActionType { get; set; } = ActionType.RunApplication;
    
    /// <summary>Aksiyon verisi (dosya yolu, URL, metin veya komut)</summary>
    public string ActionData { get; set; } = string.Empty;
    
    /// <summary>Hotkey aktif mi</summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>Oluşturulma tarihi</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>Hotkey kombinasyonunun string gösterimi</summary>
    public string GetHotkeyString()
    {
        var parts = new List<string>();
        if (Ctrl) parts.Add("Ctrl");
        if (Alt) parts.Add("Alt");
        if (Shift) parts.Add("Shift");
        if (Win) parts.Add("Win");
        parts.Add(((System.Windows.Forms.Keys)Key).ToString());
        return string.Join(" + ", parts);
    }
}
