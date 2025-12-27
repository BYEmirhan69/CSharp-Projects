namespace VirusAntivirus.Engine.Signatures;

/// <summary>
/// Zararlı yazılım imzası modeli.
/// </summary>
public class Signature
{
    /// <summary>
    /// Tehdit adı
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SHA-256 hash değeri (küçük harf)
    /// </summary>
    public string Sha256 { get; set; } = string.Empty;

    /// <summary>
    /// Tehdit seviyesi
    /// </summary>
    public string Severity { get; set; } = "Malware";
}

/// <summary>
/// Tehdit seviyeleri
/// </summary>
public enum ThreatSeverity
{
    /// <summary>
    /// Zararlı yazılım
    /// </summary>
    Malware,

    /// <summary>
    /// Potansiyel olarak istenmeyen program
    /// </summary>
    PUP,

    /// <summary>
    /// Reklam yazılımı
    /// </summary>
    Adware,

    /// <summary>
    /// Bilinmeyen
    /// </summary>
    Unknown
}
