using VirusAntivirus.Engine.Heuristics;

namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Tek dosya tarama sonucu
/// </summary>
public class ScanResult
{
    /// <summary>
    /// Dosya yolu
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Dosya adı
    /// </summary>
    public string FileName => Path.GetFileName(FilePath);

    /// <summary>
    /// Dosya boyutu (bytes)
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// SHA-256 hash değeri
    /// </summary>
    public string Sha256Hash { get; set; } = string.Empty;

    /// <summary>
    /// Tehdit seviyesi
    /// </summary>
    public ThreatLevel ThreatLevel { get; set; } = ThreatLevel.Clean;

    /// <summary>
    /// Tehdit adı (imza eşleşmesi varsa)
    /// </summary>
    public string ThreatName { get; set; } = string.Empty;

    /// <summary>
    /// Heuristik risk skoru (0-100)
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Heuristik bulgular
    /// </summary>
    public List<HeuristicFinding> HeuristicFindings { get; set; } = new();

    /// <summary>
    /// Tarama zamanı
    /// </summary>
    public DateTime ScanTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Hata mesajı (tarama başarısız ise)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Tarama başarılı mı
    /// </summary>
    public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Karantinaya alındı mı
    /// </summary>
    public bool IsQuarantined { get; set; }

    /// <summary>
    /// Sonuç özeti
    /// </summary>
    public string Summary
    {
        get
        {
            if (!IsSuccessful)
                return $"Hata: {ErrorMessage}";

            return ThreatLevel switch
            {
                ThreatLevel.Malware => $"Tehdit: {ThreatName}",
                ThreatLevel.Suspicious => $"Şüpheli (Risk: {RiskScore})",
                _ => "Temiz"
            };
        }
    }
}

/// <summary>
/// Tehdit seviyeleri
/// </summary>
public enum ThreatLevel
{
    /// <summary>
    /// Temiz, tehdit yok
    /// </summary>
    Clean,

    /// <summary>
    /// Şüpheli (heuristik tespit)
    /// </summary>
    Suspicious,

    /// <summary>
    /// Zararlı yazılım (imza eşleşmesi)
    /// </summary>
    Malware
}
