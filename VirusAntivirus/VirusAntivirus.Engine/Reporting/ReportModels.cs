using VirusAntivirus.Engine.Heuristics;

namespace VirusAntivirus.Engine.Reporting;

/// <summary>
/// JSON rapor için model sınıfları
/// </summary>

/// <summary>
/// Ana rapor modeli
/// </summary>
public class ScanReport
{
    public string ReportVersion { get; set; } = "1.0";
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public ReportSummary Summary { get; set; } = new();
    public List<ReportScanResult> Results { get; set; } = new();
}

/// <summary>
/// Rapor özeti
/// </summary>
public class ReportSummary
{
    public string ScannedPath { get; set; } = string.Empty;
    public string ScanMode { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Duration { get; set; } = string.Empty;
    public int TotalFilesScanned { get; set; }
    public string TotalBytesScanned { get; set; } = string.Empty;
    public int CleanFiles { get; set; }
    public int SuspiciousFiles { get; set; }
    public int MalwareFiles { get; set; }
    public int ErrorFiles { get; set; }
    public int TotalThreats { get; set; }
}

/// <summary>
/// Tek dosya tarama sonucu rapor modeli
/// </summary>
public class ReportScanResult
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string Sha256Hash { get; set; } = string.Empty;
    public string ThreatLevel { get; set; } = string.Empty;
    public string ThreatName { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public List<ReportHeuristicFinding> HeuristicFindings { get; set; } = new();
    public DateTime ScanTime { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Heuristik bulgu rapor modeli
/// </summary>
public class ReportHeuristicFinding
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RiskContribution { get; set; }
}
