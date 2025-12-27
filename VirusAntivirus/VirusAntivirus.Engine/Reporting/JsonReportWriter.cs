using System.Text.Json;
using VirusAntivirus.Common;
using VirusAntivirus.Engine.Scanning;

namespace VirusAntivirus.Engine.Reporting;

/// <summary>
/// JSON formatında tarama raporu yazan servis.
/// </summary>
public class JsonReportWriter
{
    /// <summary>
    /// Son oluşturulan rapor dosyasının yolu
    /// </summary>
    public string? LastReportPath { get; private set; }

    /// <summary>
    /// Tarama raporunu JSON olarak yazar.
    /// </summary>
    /// <param name="summary">Tarama özeti</param>
    /// <param name="results">Tarama sonuçları</param>
    /// <returns>Oluşturulan rapor dosyasının yolu</returns>
    public async Task<string> WriteReportAsync(ScanSummary summary, List<ScanResult> results)
    {
        // Raporlar klasörünü oluştur
        Config.EnsureDirectoriesExist();

        // Rapor dosya adı: scan_report_YYYYMMDD_HHMMSS.json
        var fileName = $"scan_report_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var reportPath = Path.Combine(Config.ReportsFolder, fileName);

        // Rapor modelini oluştur
        var report = new ScanReport
        {
            GeneratedAt = DateTime.Now,
            Summary = new ReportSummary
            {
                ScannedPath = summary.ScannedPath,
                ScanMode = summary.ScanMode.ToString(),
                StartTime = summary.StartTime,
                EndTime = summary.EndTime,
                Duration = summary.FormattedDuration,
                TotalFilesScanned = summary.TotalFilesScanned,
                TotalBytesScanned = summary.FormattedSize,
                CleanFiles = summary.CleanFiles,
                SuspiciousFiles = summary.SuspiciousFiles,
                MalwareFiles = summary.MalwareFiles,
                ErrorFiles = summary.ErrorFiles,
                TotalThreats = summary.TotalThreats
            },
            Results = results.Select(r => new ReportScanResult
            {
                FilePath = r.FilePath,
                FileName = r.FileName,
                FileSize = r.FileSize,
                Sha256Hash = r.Sha256Hash,
                ThreatLevel = r.ThreatLevel.ToString(),
                ThreatName = r.ThreatName,
                RiskScore = r.RiskScore,
                HeuristicFindings = r.HeuristicFindings.Select(f => new ReportHeuristicFinding
                {
                    Type = f.Type.ToString(),
                    Description = f.Description,
                    RiskContribution = f.RiskContribution
                }).ToList(),
                ScanTime = r.ScanTime,
                IsSuccessful = r.IsSuccessful,
                ErrorMessage = r.ErrorMessage
            }).ToList()
        };

        // JSON olarak yaz
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(report, options);
        await File.WriteAllTextAsync(reportPath, json);

        LastReportPath = reportPath;
        Logger.Info($"Rapor yazıldı: {reportPath}");

        return reportPath;
    }

    /// <summary>
    /// Belirtilen rapor dosyasını okur.
    /// </summary>
    public async Task<ScanReport?> ReadReportAsync(string reportPath)
    {
        try
        {
            if (!File.Exists(reportPath))
                return null;

            var json = await File.ReadAllTextAsync(reportPath);
            return JsonSerializer.Deserialize<ScanReport>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Logger.Error($"Rapor okunamadı: {reportPath}", ex);
            return null;
        }
    }

    /// <summary>
    /// Tüm rapor dosyalarını listeler.
    /// </summary>
    public List<string> GetAllReports()
    {
        try
        {
            if (!Directory.Exists(Config.ReportsFolder))
                return new List<string>();

            return Directory.GetFiles(Config.ReportsFolder, "scan_report_*.json")
                .OrderByDescending(f => f)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error("Raporlar listelenemedi", ex);
            return new List<string>();
        }
    }
}
