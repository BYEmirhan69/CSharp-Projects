using System.Collections.Concurrent;

namespace VirusAntivirus.Common;

/// <summary>
/// Basit dosya tabanlı loglama servisi.
/// Thread-safe ve asenkron yazma desteği sağlar.
/// </summary>
public static class Logger
{
    private static readonly object _lock = new();
    private static readonly ConcurrentQueue<string> _logQueue = new();
    private static bool _isInitialized = false;
    private static string _currentLogFile = string.Empty;

    /// <summary>
    /// Log seviyesi
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Minimum log seviyesi (bu ve üstü loglanır)
    /// </summary>
    public static LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    /// <summary>
    /// Logger'ı başlatır ve log dosyasını oluşturur.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized) return;

        lock (_lock)
        {
            if (_isInitialized) return;

            Config.EnsureDirectoriesExist();

            var fileName = $"virusantivirus_{DateTime.Now:yyyyMMdd}.log";
            _currentLogFile = Path.Combine(Config.LogsFolder, fileName);

            _isInitialized = true;
            Info("Logger başlatıldı.");
        }
    }

    /// <summary>
    /// Debug seviyesinde log yazar.
    /// </summary>
    public static void Debug(string message) => Log(LogLevel.Debug, message);

    /// <summary>
    /// Info seviyesinde log yazar.
    /// </summary>
    public static void Info(string message) => Log(LogLevel.Info, message);

    /// <summary>
    /// Warning seviyesinde log yazar.
    /// </summary>
    public static void Warning(string message) => Log(LogLevel.Warning, message);

    /// <summary>
    /// Error seviyesinde log yazar.
    /// </summary>
    public static void Error(string message) => Log(LogLevel.Error, message);

    /// <summary>
    /// Exception ile birlikte error log yazar.
    /// </summary>
    public static void Error(string message, Exception ex)
    {
        Log(LogLevel.Error, $"{message} | Exception: {ex.GetType().Name}: {ex.Message}");
        if (ex.StackTrace != null)
        {
            Log(LogLevel.Error, $"StackTrace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Belirtilen seviyede log yazar.
    /// </summary>
    private static void Log(LogLevel level, string message)
    {
        if (level < MinimumLevel) return;

        if (!_isInitialized)
        {
            Initialize();
        }

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var levelStr = level.ToString().ToUpper().PadRight(7);
        var logLine = $"[{timestamp}] [{levelStr}] {message}";

        WriteToFile(logLine);
    }

    /// <summary>
    /// Log satırını dosyaya yazar.
    /// </summary>
    private static void WriteToFile(string logLine)
    {
        try
        {
            lock (_lock)
            {
                File.AppendAllText(_currentLogFile, logLine + Environment.NewLine);
            }
        }
        catch
        {
            // Log yazma hatalarını sessizce yoksay
        }
    }

    /// <summary>
    /// Tüm bekleyen logları dosyaya yazar.
    /// </summary>
    public static void Flush()
    {
        // Şu anki implementasyonda senkron yazıldığı için ek işlem gerekmez
    }

    /// <summary>
    /// Eski log dosyalarını temizler (varsayılan: 30 günden eski).
    /// </summary>
    public static void CleanOldLogs(int daysToKeep = 30)
    {
        try
        {
            var logDir = new DirectoryInfo(Config.LogsFolder);
            if (!logDir.Exists) return;

            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var file in logDir.GetFiles("virusantivirus_*.log"))
            {
                if (file.CreationTime < cutoffDate)
                {
                    try
                    {
                        file.Delete();
                        Info($"Eski log dosyası silindi: {file.Name}");
                    }
                    catch (Exception ex)
                    {
                        Error($"Log dosyası silinemedi: {file.Name}", ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Error("Log temizleme sırasında hata oluştu", ex);
        }
    }
}
