using VirusAntivirus.Common;
using VirusAntivirus.Engine.Reporting;
using VirusAntivirus.Engine.Signatures;

namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Ana tarama servisi. UI'ın tek çağırdığı servis.
/// Dosya ve klasör taramalarını, raporlamayı yönetir.
/// </summary>
public class ScanService : IDisposable
{
    private readonly SignatureDatabase _signatureDb;
    private readonly JsonReportWriter _reportWriter;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Tarama modu
    /// </summary>
    public ScanMode ScanMode { get; set; } = ScanMode.Fast;

    /// <summary>
    /// Hariç tutulacak pattern'ler
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new();

    /// <summary>
    /// Maksimum paralel tarama sayısı
    /// </summary>
    public int MaxParallelism { get; set; } = Config.DefaultMaxParallelism;

    /// <summary>
    /// Tarama devam ediyor mu
    /// </summary>
    public bool IsScanning { get; private set; }

    /// <summary>
    /// Son tarama sonuçları
    /// </summary>
    public List<ScanResult> LastResults { get; private set; } = new();

    /// <summary>
    /// Son tarama özeti
    /// </summary>
    public ScanSummary? LastSummary { get; private set; }

    public ScanService()
    {
        _signatureDb = new SignatureDatabase();
        _reportWriter = new JsonReportWriter();
    }

    /// <summary>
    /// Servisi başlatır ve imza veritabanını yükler.
    /// </summary>
    public async Task InitializeAsync()
    {
        Logger.Initialize();
        Config.EnsureDirectoriesExist();
        await _signatureDb.LoadAsync();
        Logger.Info("ScanService başlatıldı.");
    }

    /// <summary>
    /// Tek dosya tarar
    /// </summary>
    /// <param name="filePath">Dosya yolu</param>
    /// <param name="progress">İlerleme callback</param>
    /// <returns>Tarama sonucu ve özeti</returns>
    public async Task<(ScanResult Result, ScanSummary Summary)> ScanFileAsync(
        string filePath,
        IProgress<ScanProgress>? progress = null)
    {
        if (IsScanning)
            throw new InvalidOperationException("Başka bir tarama devam ediyor.");

        IsScanning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        var summary = new ScanSummary
        {
            StartTime = DateTime.Now,
            ScannedPath = filePath,
            ScanMode = ScanMode
        };

        try
        {
            var scanner = new FileScanner(_signatureDb)
            {
                CancellationToken = _cancellationTokenSource.Token,
                Progress = progress
            };

            progress?.Report(new ScanProgress
            {
                TotalFiles = 1,
                ScannedFiles = 0,
                CurrentFile = filePath,
                StatusMessage = "Dosya taranıyor..."
            });

            var result = await scanner.ScanFileAsync(filePath, ScanMode);

            // Özeti güncelle
            summary.EndTime = DateTime.Now;
            summary.TotalFilesScanned = 1;
            summary.TotalBytesScanned = result.FileSize;

            if (!result.IsSuccessful)
                summary.ErrorFiles = 1;
            else if (result.ThreatLevel == ThreatLevel.Malware)
                summary.MalwareFiles = 1;
            else if (result.ThreatLevel == ThreatLevel.Suspicious)
                summary.SuspiciousFiles = 1;
            else
                summary.CleanFiles = 1;

            // Sonuçları sakla
            LastResults = new List<ScanResult> { result };
            LastSummary = summary;

            // Rapor oluştur
            var reportPath = await _reportWriter.WriteReportAsync(summary, LastResults);
            Logger.Info($"Rapor oluşturuldu: {reportPath}");

            progress?.Report(new ScanProgress
            {
                TotalFiles = 1,
                ScannedFiles = 1,
                CurrentFile = filePath,
                ThreatsFound = summary.TotalThreats,
                StatusMessage = "Tarama tamamlandı."
            });

            return (result, summary);
        }
        finally
        {
            IsScanning = false;
        }
    }

    /// <summary>
    /// Klasör tarar
    /// </summary>
    /// <param name="folderPath">Klasör yolu</param>
    /// <param name="progress">İlerleme callback</param>
    /// <returns>Tarama sonuçları ve özeti</returns>
    public async Task<(List<ScanResult> Results, ScanSummary Summary)> ScanFolderAsync(
        string folderPath,
        IProgress<ScanProgress>? progress = null)
    {
        if (IsScanning)
            throw new InvalidOperationException("Başka bir tarama devam ediyor.");

        IsScanning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        var summary = new ScanSummary
        {
            StartTime = DateTime.Now,
            ScannedPath = folderPath,
            ScanMode = ScanMode
        };

        try
        {
            var scanner = new FolderScanner(_signatureDb)
            {
                CancellationToken = _cancellationTokenSource.Token,
                Progress = progress,
                ExcludePatterns = ExcludePatterns,
                MaxParallelism = MaxParallelism
            };

            progress?.Report(new ScanProgress
            {
                StatusMessage = "Dosyalar listeleniyor..."
            });

            var results = await scanner.ScanFolderAsync(folderPath, ScanMode);

            // Özeti güncelle
            summary.EndTime = DateTime.Now;
            summary.TotalFilesScanned = results.Count;

            foreach (var result in results)
            {
                summary.TotalBytesScanned += result.FileSize;

                if (!result.IsSuccessful)
                    summary.ErrorFiles++;
                else if (result.ThreatLevel == ThreatLevel.Malware)
                    summary.MalwareFiles++;
                else if (result.ThreatLevel == ThreatLevel.Suspicious)
                    summary.SuspiciousFiles++;
                else
                    summary.CleanFiles++;
            }

            // Sonuçları sakla
            LastResults = results;
            LastSummary = summary;

            // Rapor oluştur
            var reportPath = await _reportWriter.WriteReportAsync(summary, results);
            Logger.Info($"Rapor oluşturuldu: {reportPath}");

            progress?.Report(new ScanProgress
            {
                TotalFiles = results.Count,
                ScannedFiles = results.Count,
                ThreatsFound = summary.TotalThreats,
                StatusMessage = "Tarama tamamlandı."
            });

            return (results, summary);
        }
        finally
        {
            IsScanning = false;
        }
    }

    /// <summary>
    /// Devam eden taramayı iptal eder
    /// </summary>
    public void CancelScan()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            Logger.Info("Tarama iptal edildi.");
        }
    }

    /// <summary>
    /// Son rapor dosyasının yolunu döndürür
    /// </summary>
    public string? GetLastReportPath()
    {
        return _reportWriter.LastReportPath;
    }

    /// <summary>
    /// Raporlar klasörünü döndürür
    /// </summary>
    public string GetReportsFolder()
    {
        return Config.ReportsFolder;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}
