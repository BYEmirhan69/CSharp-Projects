using VirusAntivirus.Common;

namespace VirusAntivirus.Engine.Heuristics;

/// <summary>
/// Heuristik dosya analizi yapan servis.
/// Çeşitli risk sinyallerini kontrol ederek risk skoru üretir.
/// </summary>
public class HeuristicAnalyzer
{
    // Tehlikeli script uzantıları
    private static readonly HashSet<string> DangerousExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".com", ".scr", ".pif", ".bat", ".cmd", ".ps1", ".vbs", ".vbe",
        ".js", ".jse", ".ws", ".wsf", ".wsc", ".wsh", ".msc", ".msi", ".msp",
        ".hta", ".cpl", ".reg", ".dll", ".sys"
    };

    // İkinci uzantı olarak şüpheli olanlar
    private static readonly HashSet<string> DoubleExtensionTriggers = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt",
        ".jpg", ".jpeg", ".png", ".gif", ".mp3", ".mp4", ".avi"
    };

    // Şüpheli konum anahtar kelimeleri
    private static readonly string[] SuspiciousLocationKeywords = new[]
    {
        "temp", "tmp", "appdata", "startup", "start menu", "roaming",
        "downloads", "desktop"
    };

    /// <summary>
    /// Dosyayı heuristik olarak analiz eder.
    /// </summary>
    /// <param name="filePath">Dosya yolu</param>
    /// <param name="fullMode">Full modda daha detaylı analiz yapılır</param>
    /// <returns>Heuristik analiz sonucu</returns>
    public async Task<HeuristicResult> AnalyzeAsync(string filePath, bool fullMode = false)
    {
        var result = new HeuristicResult();

        try
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return result;
            }

            // 1. Çift uzantı kontrolü
            CheckDoubleExtension(filePath, result);

            // 2. Tehlikeli script uzantısı kontrolü
            CheckDangerousExtension(filePath, result);

            // 3. Şüpheli konum kontrolü
            CheckSuspiciousLocation(filePath, result);

            // 4. Dosya boyutu anomali kontrolü
            CheckFileSizeAnomaly(fileInfo, result);

            // 5. Gizli dosya kontrolü
            CheckHiddenFile(fileInfo, result);

            // 6. Şüpheli dosya adı kontrolü
            CheckSuspiciousFileName(fileInfo.Name, result);

            // 7. Full modda entropy analizi
            if (fullMode && fileInfo.Length > 0 && fileInfo.Length < 10 * 1024 * 1024) // 10MB'dan küçükse
            {
                await CheckEntropyAsync(filePath, result);
            }

            // Risk skorunu 100 ile sınırla
            result.RiskScore = Math.Min(result.RiskScore, 100);
        }
        catch (Exception ex)
        {
            Logger.Error($"Heuristik analiz hatası: {filePath}", ex);
        }

        return result;
    }

    /// <summary>
    /// Çift uzantı kontrolü (ör: document.pdf.exe)
    /// </summary>
    private void CheckDoubleExtension(string filePath, HeuristicResult result)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        var secondExtension = Path.GetExtension(nameWithoutExt).ToLowerInvariant();

        if (DangerousExtensions.Contains(extension) && DoubleExtensionTriggers.Contains(secondExtension))
        {
            result.RiskScore += 35;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.DoubleExtension,
                Description = $"Çift uzantı tespit edildi: {secondExtension}{extension}",
                RiskContribution = 35
            });
        }
    }

    /// <summary>
    /// Tehlikeli script uzantısı kontrolü
    /// </summary>
    private void CheckDangerousExtension(string filePath, HeuristicResult result)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        // Script uzantıları (.js, .vbs, .ps1, .bat, .cmd) için ek puan
        var scriptExtensions = new HashSet<string> { ".js", ".vbs", ".ps1", ".bat", ".cmd", ".wsf", ".hta" };
        if (scriptExtensions.Contains(extension))
        {
            result.RiskScore += 15;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.DangerousScriptExtension,
                Description = $"Script dosyası: {extension}",
                RiskContribution = 15
            });
        }
    }

    /// <summary>
    /// Şüpheli konum kontrolü
    /// </summary>
    private void CheckSuspiciousLocation(string filePath, HeuristicResult result)
    {
        var lowerPath = filePath.ToLowerInvariant();

        foreach (var keyword in SuspiciousLocationKeywords)
        {
            if (lowerPath.Contains(keyword))
            {
                result.RiskScore += 10;
                result.Findings.Add(new HeuristicFinding
                {
                    Type = HeuristicFindingType.SuspiciousLocation,
                    Description = $"Şüpheli konum: {keyword} içeriyor",
                    RiskContribution = 10
                });
                break; // Sadece bir kez ekle
            }
        }
    }

    /// <summary>
    /// Dosya boyutu anomali kontrolü
    /// </summary>
    private void CheckFileSizeAnomaly(FileInfo fileInfo, HeuristicResult result)
    {
        var extension = fileInfo.Extension.ToLowerInvariant();

        // Çok büyük dosya
        if (fileInfo.Length > Config.LargeFileSizeThreshold)
        {
            result.RiskScore += 5;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.AbnormalFileSize,
                Description = $"Çok büyük dosya: {fileInfo.Length / (1024 * 1024):N0} MB",
                RiskContribution = 5
            });
        }

        // Çok küçük PE dosyası (şüpheli)
        if (DangerousExtensions.Contains(extension) && fileInfo.Length < Config.SmallFileSizeThreshold && fileInfo.Length > 0)
        {
            result.RiskScore += 15;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.AbnormalFileSize,
                Description = $"Çok küçük çalıştırılabilir dosya: {fileInfo.Length} bytes",
                RiskContribution = 15
            });
        }
    }

    /// <summary>
    /// Gizli dosya kontrolü
    /// </summary>
    private void CheckHiddenFile(FileInfo fileInfo, HeuristicResult result)
    {
        if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        {
            result.RiskScore += 10;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.HiddenFile,
                Description = "Gizli dosya",
                RiskContribution = 10
            });
        }
    }

    /// <summary>
    /// Şüpheli dosya adı kontrolü
    /// </summary>
    private void CheckSuspiciousFileName(string fileName, HeuristicResult result)
    {
        // Unicode karakterler veya RTL override kontrolü
        if (fileName.Any(c => c > 127 || char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.Format))
        {
            result.RiskScore += 20;
            result.Findings.Add(new HeuristicFinding
            {
                Type = HeuristicFindingType.SuspiciousFileName,
                Description = "Dosya adında özel/unicode karakter",
                RiskContribution = 20
            });
        }
    }

    /// <summary>
    /// Entropy (rastgelelik) analizi - yüksek entropy packed/encrypted dosya göstergesi
    /// </summary>
    private async Task CheckEntropyAsync(string filePath, HeuristicResult result)
    {
        try
        {
            var entropy = await CalculateEntropyAsync(filePath);

            if (entropy >= Config.HighEntropyThreshold)
            {
                result.RiskScore += 20;
                result.Findings.Add(new HeuristicFinding
                {
                    Type = HeuristicFindingType.HighEntropy,
                    Description = $"Yüksek entropy: {entropy:F2} (olası packing/encryption)",
                    RiskContribution = 20
                });
            }
        }
        catch (Exception ex)
        {
            Logger.Debug($"Entropy hesaplanamadı: {filePath}, Hata: {ex.Message}");
        }
    }

    /// <summary>
    /// Dosyanın Shannon entropy değerini hesaplar (0-8 arası)
    /// </summary>
    private async Task<double> CalculateEntropyAsync(string filePath)
    {
        var frequency = new long[256];
        long totalBytes = 0;

        const int bufferSize = 81920;
        var buffer = new byte[bufferSize];

        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true))
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    frequency[buffer[i]]++;
                }
                totalBytes += bytesRead;
            }
        }

        if (totalBytes == 0)
            return 0;

        double entropy = 0;
        for (int i = 0; i < 256; i++)
        {
            if (frequency[i] > 0)
            {
                double p = (double)frequency[i] / totalBytes;
                entropy -= p * Math.Log2(p);
            }
        }

        return entropy;
    }
}
