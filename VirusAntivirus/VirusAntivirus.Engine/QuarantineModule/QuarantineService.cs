using System.Text.Json;
using VirusAntivirus.Common;

namespace VirusAntivirus.Engine.QuarantineModule;

/// <summary>
/// Dosya karantina servisi.
/// Tehditli dosyaları güvenli konuma taşır ve metadata kaydeder.
/// </summary>
public class QuarantineService
{
    /// <summary>
    /// Dosyayı karantinaya alır
    /// </summary>
    /// <param name="filePath">Karantinaya alınacak dosya</param>
    /// <param name="sha256Hash">Dosyanın SHA-256 hash değeri</param>
    /// <param name="threatLevel">Tehdit seviyesi</param>
    /// <param name="threatName">Tehdit adı</param>
    /// <returns>Başarılı olup olmadığı</returns>
    public async Task<QuarantineResult> QuarantineFileAsync(
        string filePath,
        string sha256Hash,
        string threatLevel,
        string threatName = "")
    {
        var result = new QuarantineResult
        {
            OriginalPath = filePath,
            Sha256Hash = sha256Hash
        };

        try
        {
            if (!File.Exists(filePath))
            {
                result.Success = false;
                result.ErrorMessage = "Dosya bulunamadı";
                return result;
            }

            // Karantina klasörünü oluştur
            Config.EnsureDirectoriesExist();

            // Karantina dosya adı: <sha256>.quarantine
            var quarantineFileName = $"{sha256Hash}.quarantine";
            var quarantinePath = Path.Combine(Config.QuarantineFolder, quarantineFileName);

            // Metadata dosya adı: <sha256>.meta.json
            var metadataFileName = $"{sha256Hash}.meta.json";
            var metadataPath = Path.Combine(Config.QuarantineFolder, metadataFileName);

            // Dosya zaten karantinada mı kontrol et
            if (File.Exists(quarantinePath))
            {
                result.Success = false;
                result.ErrorMessage = "Dosya zaten karantinada";
                result.QuarantinePath = quarantinePath;
                return result;
            }

            // Dosyayı karantinaya taşı
            File.Move(filePath, quarantinePath);
            result.QuarantinePath = quarantinePath;

            // Metadata oluştur ve kaydet
            var metadata = new QuarantineMetadata
            {
                OriginalPath = filePath,
                OriginalFileName = Path.GetFileName(filePath),
                Sha256Hash = sha256Hash,
                QuarantineTime = DateTime.Now,
                ThreatLevel = threatLevel,
                ThreatName = threatName
            };

            var metadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(metadataPath, metadataJson);
            result.MetadataPath = metadataPath;

            result.Success = true;
            Logger.Info($"Dosya karantinaya alındı: {filePath} -> {quarantinePath}");
        }
        catch (UnauthorizedAccessException ex)
        {
            result.Success = false;
            result.ErrorMessage = "Erişim engellendi";
            Logger.Error($"Karantina erişim hatası: {filePath}", ex);
        }
        catch (IOException ex)
        {
            result.Success = false;
            result.ErrorMessage = $"IO Hatası: {ex.Message}";
            Logger.Error($"Karantina IO hatası: {filePath}", ex);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = $"Hata: {ex.Message}";
            Logger.Error($"Karantina hatası: {filePath}", ex);
        }

        return result;
    }

    /// <summary>
    /// Karantinadaki tüm dosyaları listeler
    /// </summary>
    public async Task<List<QuarantineMetadata>> GetQuarantinedFilesAsync()
    {
        var files = new List<QuarantineMetadata>();

        try
        {
            if (!Directory.Exists(Config.QuarantineFolder))
                return files;

            var metadataFiles = Directory.GetFiles(Config.QuarantineFolder, "*.meta.json");

            foreach (var metaFile in metadataFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(metaFile);
                    var metadata = JsonSerializer.Deserialize<QuarantineMetadata>(json);
                    if (metadata != null)
                    {
                        files.Add(metadata);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning($"Metadata dosyası okunamadı: {metaFile} - {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Karantina listesi alınamadı", ex);
        }

        return files;
    }

    /// <summary>
    /// Dosyayı karantinadan geri yükler
    /// </summary>
    public async Task<bool> RestoreFileAsync(string sha256Hash)
    {
        try
        {
            var quarantinePath = Path.Combine(Config.QuarantineFolder, $"{sha256Hash}.quarantine");
            var metadataPath = Path.Combine(Config.QuarantineFolder, $"{sha256Hash}.meta.json");

            if (!File.Exists(metadataPath))
            {
                Logger.Warning($"Metadata bulunamadı: {sha256Hash}");
                return false;
            }

            var json = await File.ReadAllTextAsync(metadataPath);
            var metadata = JsonSerializer.Deserialize<QuarantineMetadata>(json);

            if (metadata == null || string.IsNullOrEmpty(metadata.OriginalPath))
            {
                Logger.Warning($"Geçersiz metadata: {sha256Hash}");
                return false;
            }

            // Orijinal konuma geri taşı
            File.Move(quarantinePath, metadata.OriginalPath);

            // Metadata dosyasını sil
            File.Delete(metadataPath);

            Logger.Info($"Dosya geri yüklendi: {metadata.OriginalPath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Geri yükleme hatası: {sha256Hash}", ex);
            return false;
        }
    }
}

/// <summary>
/// Karantina işlem sonucu
/// </summary>
public class QuarantineResult
{
    public bool Success { get; set; }
    public string OriginalPath { get; set; } = string.Empty;
    public string QuarantinePath { get; set; } = string.Empty;
    public string MetadataPath { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Karantina metadata modeli
/// </summary>
public class QuarantineMetadata
{
    public string OriginalPath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Sha256Hash { get; set; } = string.Empty;
    public DateTime QuarantineTime { get; set; }
    public string ThreatLevel { get; set; } = string.Empty;
    public string ThreatName { get; set; } = string.Empty;
}
