using System.Text.Json;
using VirusAntivirus.Common;

namespace VirusAntivirus.Engine.Signatures;

/// <summary>
/// İmza veritabanı. JSON dosyasından imzaları yükler ve yönetir.
/// </summary>
public class SignatureDatabase
{
    private readonly Dictionary<string, Signature> _signatures = new(StringComparer.OrdinalIgnoreCase);
    private bool _isLoaded = false;

    /// <summary>
    /// Yüklü imza sayısı
    /// </summary>
    public int Count => _signatures.Count;

    /// <summary>
    /// Veritabanının yüklenip yüklenmediği
    /// </summary>
    public bool IsLoaded => _isLoaded;

    /// <summary>
    /// İmza veritabanını JSON dosyasından yükler.
    /// </summary>
    /// <param name="filePath">JSON dosya yolu (varsayılan: signatures.json)</param>
    public async Task LoadAsync(string? filePath = null)
    {
        filePath ??= Config.SignaturesFilePath;

        try
        {
            if (!File.Exists(filePath))
            {
                Logger.Warning($"İmza dosyası bulunamadı: {filePath}");
                _isLoaded = true;
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var signatures = JsonSerializer.Deserialize<List<Signature>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (signatures != null)
            {
                _signatures.Clear();
                foreach (var sig in signatures)
                {
                    if (!string.IsNullOrWhiteSpace(sig.Sha256))
                    {
                        _signatures[sig.Sha256.ToLowerInvariant()] = sig;
                    }
                }
            }

            _isLoaded = true;
            Logger.Info($"İmza veritabanı yüklendi: {_signatures.Count} imza");
        }
        catch (Exception ex)
        {
            Logger.Error("İmza veritabanı yüklenirken hata oluştu", ex);
            _isLoaded = true; // Hata olsa bile devam et
        }
    }

    /// <summary>
    /// Verilen hash için eşleşen imzayı döndürür.
    /// </summary>
    /// <param name="sha256Hash">SHA-256 hash değeri</param>
    /// <returns>Eşleşen imza veya null</returns>
    public Signature? FindByHash(string sha256Hash)
    {
        if (string.IsNullOrWhiteSpace(sha256Hash))
            return null;

        _signatures.TryGetValue(sha256Hash.ToLowerInvariant(), out var signature);
        return signature;
    }

    /// <summary>
    /// Hash'in veritabanında olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="sha256Hash">SHA-256 hash değeri</param>
    /// <returns>Eşleşme varsa true</returns>
    public bool Contains(string sha256Hash)
    {
        if (string.IsNullOrWhiteSpace(sha256Hash))
            return false;

        return _signatures.ContainsKey(sha256Hash.ToLowerInvariant());
    }

    /// <summary>
    /// Tüm imzaları döndürür.
    /// </summary>
    public IEnumerable<Signature> GetAll() => _signatures.Values;

    /// <summary>
    /// Yeni imza ekler veya günceller.
    /// </summary>
    public void AddOrUpdate(Signature signature)
    {
        if (signature != null && !string.IsNullOrWhiteSpace(signature.Sha256))
        {
            _signatures[signature.Sha256.ToLowerInvariant()] = signature;
        }
    }
}
