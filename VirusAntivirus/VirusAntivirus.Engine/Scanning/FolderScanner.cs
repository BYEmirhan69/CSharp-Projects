using VirusAntivirus.Common;
using VirusAntivirus.Engine.Signatures;

namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Klasör tarama servisi. Recursive tarama ve paralel işleme destekler.
/// </summary>
public class FolderScanner : IScanner
{
    private readonly SignatureDatabase _signatureDb;
    private readonly FileScanner _fileScanner;

    public CancellationToken CancellationToken { get; set; }
    public IProgress<ScanProgress>? Progress { get; set; }

    /// <summary>
    /// Hariç tutulacak klasör/dosya pattern'leri
    /// </summary>
    public List<string> ExcludePatterns { get; set; } = new();

    /// <summary>
    /// Maksimum paralel tarama sayısı
    /// </summary>
    public int MaxParallelism { get; set; } = Config.DefaultMaxParallelism;

    public FolderScanner(SignatureDatabase signatureDb)
    {
        _signatureDb = signatureDb ?? throw new ArgumentNullException(nameof(signatureDb));
        _fileScanner = new FileScanner(signatureDb);
    }

    /// <summary>
    /// Klasörü recursive olarak tarar
    /// </summary>
    /// <param name="folderPath">Klasör yolu</param>
    /// <param name="mode">Tarama modu</param>
    /// <returns>Tarama sonuçları listesi</returns>
    public async Task<List<ScanResult>> ScanFolderAsync(string folderPath, ScanMode mode = ScanMode.Fast)
    {
        var results = new List<ScanResult>();

        if (!Directory.Exists(folderPath))
        {
            Logger.Warning($"Klasör bulunamadı: {folderPath}");
            return results;
        }

        // Tüm dosyaları listele
        var files = GetAllFiles(folderPath);
        var totalFiles = files.Count;

        Logger.Info($"Tarama başladı: {folderPath} ({totalFiles} dosya)");

        var progress = new ScanProgress
        {
            TotalFiles = totalFiles,
            StatusMessage = "Tarama başlatılıyor..."
        };
        Progress?.Report(progress);

        // Paralel tarama için semaphore
        using var semaphore = new SemaphoreSlim(MaxParallelism);
        var scannedCount = 0;
        var threatsFound = 0;
        var lockObj = new object();
        var isCancelled = false;

        _fileScanner.CancellationToken = CancellationToken;

        var tasks = files.Select(async file =>
        {
            // İptal edildiyse beklemeden çık
            if (isCancelled || CancellationToken.IsCancellationRequested)
                return;

            try
            {
                await semaphore.WaitAsync(CancellationToken);
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
                return;
            }

            try
            {
                // Her dosya taramasından önce iptal kontrolü
                if (CancellationToken.IsCancellationRequested)
                {
                    isCancelled = true;
                    return;
                }

                var result = await _fileScanner.ScanFileAsync(file, mode);

                lock (lockObj)
                {
                    if (!isCancelled)
                    {
                        results.Add(result);
                        scannedCount++;

                        if (result.ThreatLevel != ThreatLevel.Clean)
                            threatsFound++;

                        // İlerleme güncelle
                        progress.ScannedFiles = scannedCount;
                        progress.CurrentFile = file;
                        progress.ThreatsFound = threatsFound;
                        progress.StatusMessage = $"Taranan: {Path.GetFileName(file)}";
                    }
                }

                if (!isCancelled)
                {
                    Progress?.Report(new ScanProgress
                    {
                        TotalFiles = progress.TotalFiles,
                        ScannedFiles = progress.ScannedFiles,
                        CurrentFile = progress.CurrentFile,
                        ThreatsFound = progress.ThreatsFound,
                        StatusMessage = progress.StatusMessage
                    });
                }
            }
            catch (OperationCanceledException)
            {
                isCancelled = true;
            }
            finally
            {
                semaphore.Release();
            }
        });

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            Logger.Info("Tarama kullanıcı tarafından iptal edildi.");
            throw;
        }

        // İptal edildiyse exception fırlat
        if (isCancelled || CancellationToken.IsCancellationRequested)
        {
            Logger.Info("Tarama kullanıcı tarafından iptal edildi.");
            throw new OperationCanceledException("Tarama iptal edildi.", CancellationToken);
        }

        Logger.Info($"Tarama tamamlandı: {scannedCount} dosya, {threatsFound} tehdit");

        return results;
    }

    /// <summary>
    /// Klasördeki tüm dosyaları recursive olarak listeler
    /// </summary>
    private List<string> GetAllFiles(string folderPath)
    {
        var files = new List<string>();

        try
        {
            // Dosyaları ekle
            foreach (var file in Directory.GetFiles(folderPath))
            {
                if (!ShouldExclude(file))
                {
                    files.Add(file);
                }
            }

            // Alt klasörleri recursive tara
            foreach (var dir in Directory.GetDirectories(folderPath))
            {
                if (!ShouldExclude(dir))
                {
                    files.AddRange(GetAllFiles(dir));
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.Warning($"Erişim engellendi: {folderPath} - {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Klasör listeleme hatası: {folderPath}", ex);
        }

        return files;
    }

    /// <summary>
    /// Dosya/klasörün hariç tutulup tutulmayacağını kontrol eder
    /// </summary>
    private bool ShouldExclude(string path)
    {
        var name = Path.GetFileName(path);

        foreach (var pattern in ExcludePatterns)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                continue;

            // Basit eşleştirme: isim pattern içeriyorsa hariç tut
            if (name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                return true;

            // Tam eşleşme
            if (name.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
