namespace KeyboardUtils.Core.Models;

/// <summary>
/// Tüm uygulama ayarlarını içeren root model
/// </summary>
public class AppSettings
{
    /// <summary>Ayar dosyası sürümü</summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>Son kaydetme tarihi</summary>
    public DateTime LastSaved { get; set; } = DateTime.Now;
    
    // === Hotkey Manager ===
    /// <summary>Tanımlı hotkey'ler</summary>
    public List<HotkeyAction> Hotkeys { get; set; } = new();
    
    /// <summary>Hotkey Manager aktif mi</summary>
    public bool HotkeyManagerEnabled { get; set; } = true;
    
    // === Typing Tutor ===
    /// <summary>Yazma pratiği geçmişi</summary>
    public List<TypingSession> TypingSessions { get; set; } = new();
    
    /// <summary>Özel pratik metinleri</summary>
    public List<PracticeText> CustomPracticeTexts { get; set; } = new();
    
    // === Key Display Overlay ===
    /// <summary>Key display ayarları</summary>
    public KeyDisplaySettings KeyDisplaySettings { get; set; } = new();
    
    // === Keyboard Assist ===
    /// <summary>Snippet profilleri</summary>
    public List<Profile> Profiles { get; set; } = new();
    
    /// <summary>Aktif profil ID</summary>
    public string? ActiveProfileId { get; set; }
    
    /// <summary>Keyboard Assist aktif mi</summary>
    public bool KeyboardAssistEnabled { get; set; } = false;
    
    // === Genel Ayarlar ===
    /// <summary>Uygulama başlangıçta küçültülmüş mü</summary>
    public bool StartMinimized { get; set; } = false;
    
    /// <summary>Windows ile başlat</summary>
    public bool StartWithWindows { get; set; } = false;
    
    /// <summary>Tema (Dark/Light)</summary>
    public string Theme { get; set; } = "Dark";
}
