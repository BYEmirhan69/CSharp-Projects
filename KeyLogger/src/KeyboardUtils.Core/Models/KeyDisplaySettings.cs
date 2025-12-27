namespace KeyboardUtils.Core.Models;

/// <summary>
/// Key Display Overlay ayarları
/// </summary>
public class KeyDisplaySettings
{
    /// <summary>Overlay pencere X konumu</summary>
    public int PositionX { get; set; } = 100;
    
    /// <summary>Overlay pencere Y konumu</summary>
    public int PositionY { get; set; } = 100;
    
    /// <summary>Overlay genişliği</summary>
    public int Width { get; set; } = 300;
    
    /// <summary>Overlay yüksekliği</summary>
    public int Height { get; set; } = 80;
    
    /// <summary>Şeffaflık (0.0 - 1.0)</summary>
    public double Opacity { get; set; } = 0.9;
    
    /// <summary>Arka plan rengi (ARGB)</summary>
    public string BackgroundColor { get; set; } = "#DD1E1E1E";
    
    /// <summary>Metin rengi (ARGB)</summary>
    public string TextColor { get; set; } = "#FFFFFFFF";
    
    /// <summary>Font boyutu</summary>
    public int FontSize { get; set; } = 24;
    
    /// <summary>Tuş gösterim süresi (ms)</summary>
    public int DisplayDuration { get; set; } = 2000;
    
    /// <summary>Modifier tuşları göster (Ctrl, Alt, Shift)</summary>
    public bool ShowModifiers { get; set; } = true;
    
    /// <summary>Sadece bu tuşları göster (boşsa tümünü göster)</summary>
    public List<string> Whitelist { get; set; } = new();
    
    /// <summary>Bu tuşları gösterme</summary>
    public List<string> Blacklist { get; set; } = new();
    
    /// <summary>Overlay varsayılan olarak aktif mi</summary>
    public bool IsEnabledByDefault { get; set; } = false;
}
