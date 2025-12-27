namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Tarama özeti
/// </summary>
public class ScanSummary
{
    /// <summary>
    /// Tarama başlangıç zamanı
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Tarama bitiş zamanı
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Toplam tarama süresi
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Taranan toplam dosya sayısı
    /// </summary>
    public int TotalFilesScanned { get; set; }

    /// <summary>
    /// Temiz dosya sayısı
    /// </summary>
    public int CleanFiles { get; set; }

    /// <summary>
    /// Şüpheli dosya sayısı
    /// </summary>
    public int SuspiciousFiles { get; set; }

    /// <summary>
    /// Zararlı dosya sayısı
    /// </summary>
    public int MalwareFiles { get; set; }

    /// <summary>
    /// Hatalı/erişilemeyen dosya sayısı
    /// </summary>
    public int ErrorFiles { get; set; }

    /// <summary>
    /// Toplam taranan boyut (bytes)
    /// </summary>
    public long TotalBytesScanned { get; set; }

    /// <summary>
    /// Taranan konum (dosya veya klasör yolu)
    /// </summary>
    public string ScannedPath { get; set; } = string.Empty;

    /// <summary>
    /// Tarama modu
    /// </summary>
    public ScanMode ScanMode { get; set; }

    /// <summary>
    /// Toplam tehdit sayısı
    /// </summary>
    public int TotalThreats => SuspiciousFiles + MalwareFiles;

    /// <summary>
    /// Formatlanmış süre
    /// </summary>
    public string FormattedDuration
    {
        get
        {
            if (Duration.TotalMinutes >= 1)
                return $"{Duration.Minutes} dk {Duration.Seconds} sn";
            return $"{Duration.TotalSeconds:F1} saniye";
        }
    }

    /// <summary>
    /// Formatlanmış boyut
    /// </summary>
    public string FormattedSize
    {
        get
        {
            if (TotalBytesScanned >= 1024 * 1024 * 1024)
                return $"{TotalBytesScanned / (1024.0 * 1024 * 1024):F2} GB";
            if (TotalBytesScanned >= 1024 * 1024)
                return $"{TotalBytesScanned / (1024.0 * 1024):F2} MB";
            if (TotalBytesScanned >= 1024)
                return $"{TotalBytesScanned / 1024.0:F2} KB";
            return $"{TotalBytesScanned} bytes";
        }
    }
}

/// <summary>
/// Tarama modları
/// </summary>
public enum ScanMode
{
    /// <summary>
    /// Hızlı tarama - düşük eşikler
    /// </summary>
    Fast,

    /// <summary>
    /// Tam tarama - tüm kontroller
    /// </summary>
    Full
}
