using System.Text.Json;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Services;

/// <summary>
/// JSON tabanlı ayar yönetim servisi
/// </summary>
public class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private AppSettings? _cachedSettings;
    private readonly object _lockObj = new();

    public string SettingsFilePath { get; }

    public event EventHandler<AppSettings>? SettingsChanged;

    public JsonSettingsService()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "KeyboardUtils");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        SettingsFilePath = Path.Combine(appFolder, "settings.json");
    }

    public async Task<AppSettings> LoadAsync()
    {
        try
        {
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            if (!File.Exists(SettingsFilePath))
            {
                var defaultSettings = CreateDefaultSettings();
                await SaveAsync(defaultSettings);
                return defaultSettings;
            }

            var json = await File.ReadAllTextAsync(SettingsFilePath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
            
            lock (_lockObj)
            {
                _cachedSettings = settings ?? CreateDefaultSettings();
            }
            
            return _cachedSettings;
        }
        catch (Exception)
        {
            return CreateDefaultSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        try
        {
            settings.LastSaved = DateTime.Now;
            
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            await File.WriteAllTextAsync(SettingsFilePath, json);
            
            lock (_lockObj)
            {
                _cachedSettings = settings;
            }
            
            SettingsChanged?.Invoke(this, settings);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ayarlar kaydedilemedi: {ex.Message}", ex);
        }
    }

    public async Task<AppSettings> ResetToDefaultAsync()
    {
        var defaultSettings = CreateDefaultSettings();
        await SaveAsync(defaultSettings);
        return defaultSettings;
    }

    private static AppSettings CreateDefaultSettings()
    {
        var settings = new AppSettings
        {
            Version = "1.0.0",
            HotkeyManagerEnabled = true,
            KeyboardAssistEnabled = false,
            Theme = "Dark",
            KeyDisplaySettings = new KeyDisplaySettings
            {
                PositionX = 100,
                PositionY = 100,
                Width = 300,
                Height = 80,
                Opacity = 0.9,
                FontSize = 24,
                DisplayDuration = 2000,
                ShowModifiers = true,
                IsEnabledByDefault = false
            }
        };

        // Varsayılan profil ekle
        var defaultProfile = new Profile
        {
            Name = "Varsayılan",
            Description = "Varsayılan snippet profili",
            Color = "#FF6C5CE7",
            Snippets = new List<Snippet>
            {
                new() { Trigger = "btw", Expansion = "by the way", Description = "Kısaltma" },
                new() { Trigger = "omw", Expansion = "on my way", Description = "Kısaltma" },
                new() { Trigger = "brb", Expansion = "be right back", Description = "Kısaltma" },
                new() { Trigger = "ty", Expansion = "thank you", Description = "Kısaltma" },
                new() { Trigger = "np", Expansion = "no problem", Description = "Kısaltma" }
            }
        };

        settings.Profiles.Add(defaultProfile);
        settings.ActiveProfileId = defaultProfile.Id;

        return settings;
    }
}
