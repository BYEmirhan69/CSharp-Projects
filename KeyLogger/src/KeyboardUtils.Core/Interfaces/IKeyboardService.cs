namespace KeyboardUtils.Core.Interfaces;

/// <summary>
/// Keyboard hook servisi arayüzü
/// </summary>
public interface IKeyboardService : IDisposable
{
    /// <summary>Hook'u başlat</summary>
    void Start();
    
    /// <summary>Hook'u durdur</summary>
    void Stop();
    
    /// <summary>Hook aktif mi</summary>
    bool IsRunning { get; }
    
    /// <summary>Tuş basıldığında</summary>
    event EventHandler<KeyEventData>? KeyPressed;
    
    /// <summary>Tuş bırakıldığında</summary>
    event EventHandler<KeyEventData>? KeyReleased;
}

/// <summary>
/// Tuş event verisi
/// </summary>
public class KeyEventData
{
    /// <summary>Tuş kodu</summary>
    public int KeyCode { get; set; }
    
    /// <summary>Tuş adı</summary>
    public string KeyName { get; set; } = string.Empty;
    
    /// <summary>Ctrl basılı mı</summary>
    public bool Ctrl { get; set; }
    
    /// <summary>Alt basılı mı</summary>
    public bool Alt { get; set; }
    
    /// <summary>Shift basılı mı</summary>
    public bool Shift { get; set; }
    
    /// <summary>Karakter değeri (varsa)</summary>
    public char? Character { get; set; }
    
    /// <summary>Zaman damgası</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    /// <summary>Modifier tuşlarıyla birlikte string gösterimi</summary>
    public string GetDisplayString()
    {
        var parts = new List<string>();
        if (Ctrl) parts.Add("Ctrl");
        if (Alt) parts.Add("Alt");
        if (Shift) parts.Add("Shift");
        parts.Add(KeyName);
        return string.Join(" + ", parts);
    }
}
