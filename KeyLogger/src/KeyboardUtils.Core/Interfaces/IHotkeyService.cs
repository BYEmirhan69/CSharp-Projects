using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Core.Interfaces;

/// <summary>
/// Global hotkey servisi arayüzü
/// </summary>
public interface IHotkeyService : IDisposable
{
    /// <summary>Hotkey kaydet</summary>
    bool RegisterHotkey(HotkeyAction hotkey);
    
    /// <summary>Hotkey kaydını sil</summary>
    bool UnregisterHotkey(string hotkeyId);
    
    /// <summary>Tüm hotkey'leri kaydet</summary>
    void RegisterAllHotkeys(IEnumerable<HotkeyAction> hotkeys);
    
    /// <summary>Tüm hotkey'lerin kaydını sil</summary>
    void UnregisterAllHotkeys();
    
    /// <summary>Kayıtlı hotkey'ler</summary>
    IReadOnlyList<HotkeyAction> RegisteredHotkeys { get; }
    
    /// <summary>Hotkey tetiklendiğinde</summary>
    event EventHandler<HotkeyTriggeredEventArgs>? HotkeyTriggered;
    
    /// <summary>Servis aktif mi</summary>
    bool IsEnabled { get; set; }
}

/// <summary>
/// Hotkey tetiklenme event args
/// </summary>
public class HotkeyTriggeredEventArgs : EventArgs
{
    public HotkeyAction Hotkey { get; }
    public DateTime TriggeredAt { get; }
    
    public HotkeyTriggeredEventArgs(HotkeyAction hotkey)
    {
        Hotkey = hotkey;
        TriggeredAt = DateTime.Now;
    }
}
