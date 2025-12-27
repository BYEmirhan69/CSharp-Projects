namespace VirusAntivirus.Common;

/// <summary>
/// Uygulama genelinde kullanılan yolları ve varsayılan ayarları sağlar.
/// </summary>
public static class Config
{
    /// <summary>
    /// Uygulama kök klasörü (%LOCALAPPDATA%\VirusAntivirus)
    /// </summary>
    public static string AppDataFolder
    {
        get
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "VirusAntivirus");
        }
    }

    /// <summary>
    /// Karantina klasörü yolu
    /// </summary>
    public static string QuarantineFolder => Path.Combine(AppDataFolder, "Quarantine");

    /// <summary>
    /// Log dosyaları klasörü
    /// </summary>
    public static string LogsFolder => Path.Combine(AppDataFolder, "Logs");

    /// <summary>
    /// Raporlar klasörü (uygulama çalışma dizininde)
    /// </summary>
    public static string ReportsFolder => Path.Combine(AppContext.BaseDirectory, "Reports");

    /// <summary>
    /// İmza veritabanı dosya yolu
    /// </summary>
    public static string SignaturesFilePath => Path.Combine(AppContext.BaseDirectory, "signatures.json");

    /// <summary>
    /// Varsayılan exclude pattern listesi
    /// </summary>
    public static readonly string[] DefaultExcludePatterns = new[]
    {
        "bin",
        "obj",
        ".git",
        ".vs",
        "node_modules",
        "packages"
    };

    /// <summary>
    /// Varsayılan maksimum paralel tarama sayısı
    /// </summary>
    public const int DefaultMaxParallelism = 4;

    /// <summary>
    /// Minimum paralel tarama sayısı
    /// </summary>
    public const int MinParallelism = 1;

    /// <summary>
    /// Maksimum paralel tarama sayısı
    /// </summary>
    public const int MaxParallelism = 8;

    /// <summary>
    /// Fast modda heuristik risk eşiği
    /// </summary>
    public const int FastModeRiskThreshold = 80;

    /// <summary>
    /// Full modda heuristik risk eşiği
    /// </summary>
    public const int FullModeRiskThreshold = 70;

    /// <summary>
    /// Büyük dosya eşiği (bytes) - 100MB
    /// </summary>
    public const long LargeFileSizeThreshold = 100 * 1024 * 1024;

    /// <summary>
    /// Küçük PE dosya eşiği (bytes) - 10KB
    /// </summary>
    public const long SmallFileSizeThreshold = 10 * 1024;

    /// <summary>
    /// Yüksek entropy eşiği (0-8 arası)
    /// </summary>
    public const double HighEntropyThreshold = 7.5;

    /// <summary>
    /// Gerekli klasörlerin varlığını sağlar.
    /// </summary>
    public static void EnsureDirectoriesExist()
    {
        Directory.CreateDirectory(AppDataFolder);
        Directory.CreateDirectory(QuarantineFolder);
        Directory.CreateDirectory(LogsFolder);
        Directory.CreateDirectory(ReportsFolder);
    }
}
