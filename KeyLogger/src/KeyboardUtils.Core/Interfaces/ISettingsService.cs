using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Core.Interfaces;

/// <summary>
/// JSON tabanlı ayar servisi arayüzü
/// </summary>
public interface ISettingsService
{
    /// <summary>Ayarları yükle</summary>
    Task<AppSettings> LoadAsync();
    
    /// <summary>Ayarları kaydet</summary>
    Task SaveAsync(AppSettings settings);
    
    /// <summary>Ayarları varsayılana sıfırla</summary>
    Task<AppSettings> ResetToDefaultAsync();
    
    /// <summary>Ayar dosyası yolu</summary>
    string SettingsFilePath { get; }
    
    /// <summary>Ayarlar değiştiğinde tetiklenir</summary>
    event EventHandler<AppSettings>? SettingsChanged;
}
