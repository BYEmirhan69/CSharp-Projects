namespace VirusAntivirus.Engine.Scanning;

/// <summary>
/// Tarama arayüzü
/// </summary>
public interface IScanner
{
    /// <summary>
    /// Tarama iptal token'ı
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// İlerleme bildirimi için callback
    /// </summary>
    IProgress<ScanProgress>? Progress { get; set; }
}
