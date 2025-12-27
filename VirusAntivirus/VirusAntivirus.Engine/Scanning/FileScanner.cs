using VirusAntivirus.Common;
using VirusAntivirus.Engine.Hashing;
using VirusAntivirus.Engine.Heuristics;
using VirusAntivirus.Engine.Signatures;

namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Tek dosya tarama servisi
/// </summary>
public class FileScanner : IScanner
{
    private readonly SignatureDatabase _signatureDb;
    private readonly SignatureMatcher _signatureMatcher;
    private readonly HeuristicAnalyzer _heuristicAnalyzer;

    public CancellationToken CancellationToken { get; set; }
    public IProgress<ScanProgress>? Progress { get; set; }

    public FileScanner(SignatureDatabase signatureDb)
    {
        _signatureDb = signatureDb ?? throw new ArgumentNullException(nameof(signatureDb));
        _signatureMatcher = new SignatureMatcher(signatureDb);
        _heuristicAnalyzer = new HeuristicAnalyzer();
    }

    /// <summary>
    /// Tek dosyayı tarar
    /// </summary>
    /// <param name="filePath">Dosya yolu</param>
    /// <param name="mode">Tarama modu</param>
    /// <returns>Tarama sonucu</returns>
    public async Task<ScanResult> ScanFileAsync(string filePath, ScanMode mode = ScanMode.Fast)
    {
        var result = new ScanResult
        {
            FilePath = filePath,
            ScanTime = DateTime.Now
        };

        try
        {
            // İptal kontrolü
            CancellationToken.ThrowIfCancellationRequested();

            // Dosya varlık kontrolü
            if (!File.Exists(filePath))
            {
                result.ErrorMessage = "Dosya bulunamadı";
                return result;
            }

            var fileInfo = new FileInfo(filePath);
            result.FileSize = fileInfo.Length;

            // SHA-256 hash hesapla
            Logger.Debug($"Hash hesaplanıyor: {filePath}");
            result.Sha256Hash = await Sha256Hasher.ComputeHashAsync(filePath);

            // İmza eşleştirme
            var matchResult = _signatureMatcher.Match(result.Sha256Hash);
            if (matchResult.IsMatch)
            {
                result.ThreatLevel = ThreatLevel.Malware;
                result.ThreatName = matchResult.ThreatName;
                Logger.Info($"Tehdit tespit edildi: {filePath} -> {matchResult.ThreatName}");
                return result;
            }

            // Heuristik analiz
            var fullMode = mode == ScanMode.Full;
            var heuristicResult = await _heuristicAnalyzer.AnalyzeAsync(filePath, fullMode);
            result.RiskScore = heuristicResult.RiskScore;
            result.HeuristicFindings = heuristicResult.Findings;

            // Risk eşiği kontrolü
            var riskThreshold = mode == ScanMode.Fast
                ? Config.FastModeRiskThreshold
                : Config.FullModeRiskThreshold;

            if (heuristicResult.RiskScore >= riskThreshold)
            {
                result.ThreatLevel = ThreatLevel.Suspicious;
                result.ThreatName = "Heuristik Şüphe";
                Logger.Info($"Şüpheli dosya: {filePath} (Risk: {heuristicResult.RiskScore})");
            }
            else
            {
                result.ThreatLevel = ThreatLevel.Clean;
            }
        }
        catch (OperationCanceledException)
        {
            result.ErrorMessage = "Tarama iptal edildi";
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            result.ErrorMessage = "Erişim engellendi";
            Logger.Warning($"Erişim engellendi: {filePath}");
        }
        catch (IOException ex)
        {
            result.ErrorMessage = $"IO Hatası: {ex.Message}";
            Logger.Error($"IO hatası: {filePath}", ex);
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Hata: {ex.Message}";
            Logger.Error($"Tarama hatası: {filePath}", ex);
        }

        return result;
    }
}
