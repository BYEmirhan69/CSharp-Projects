namespace VirusAntivirus.Engine.Heuristics;

/// <summary>
/// Heuristik analiz bulgularını temsil eder.
/// </summary>
public class HeuristicFinding
{
    /// <summary>
    /// Bulgu tipi
    /// </summary>
    public HeuristicFindingType Type { get; set; }

    /// <summary>
    /// Bulgu açıklaması
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Bu bulgunun risk puanı katkısı (0-100)
    /// </summary>
    public int RiskContribution { get; set; }
}

/// <summary>
/// Heuristik bulgu tipleri
/// </summary>
public enum HeuristicFindingType
{
    /// <summary>
    /// Çift uzantı tespit edildi (ör: .pdf.exe)
    /// </summary>
    DoubleExtension,

    /// <summary>
    /// Tehlikeli script uzantısı
    /// </summary>
    DangerousScriptExtension,

    /// <summary>
    /// Şüpheli konum (Temp, AppData vb.)
    /// </summary>
    SuspiciousLocation,

    /// <summary>
    /// Anormal dosya boyutu
    /// </summary>
    AbnormalFileSize,

    /// <summary>
    /// Yüksek entropy (olası packing/obfuscation)
    /// </summary>
    HighEntropy,

    /// <summary>
    /// Gizli dosya
    /// </summary>
    HiddenFile,

    /// <summary>
    /// Şüpheli dosya adı karakterleri
    /// </summary>
    SuspiciousFileName
}
