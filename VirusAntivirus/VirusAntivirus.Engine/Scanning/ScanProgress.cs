namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Tarama ilerleme bilgisi
/// </summary>
public class ScanProgress
{
    /// <summary>
    /// Taranan toplam dosya sayısı
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// Şu ana kadar taranan dosya sayısı
    /// </summary>
    public int ScannedFiles { get; set; }

    /// <summary>
    /// Şu anda taranan dosya yolu
    /// </summary>
    public string CurrentFile { get; set; } = string.Empty;

    /// <summary>
    /// İlerleme yüzdesi (0-100)
    /// </summary>
    public int Percentage => TotalFiles > 0 ? (int)((ScannedFiles * 100.0) / TotalFiles) : 0;

    /// <summary>
    /// Bulunan tehdit sayısı
    /// </summary>
    public int ThreatsFound { get; set; }

    /// <summary>
    /// Tarama durumu mesajı
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;
}
