using System.Security.Cryptography;

namespace VirusAntivirus.Engine.Hashing;

/// <summary>
/// SHA-256 hash hesaplama servisi.
/// Dosyaları stream ile okuyarak memory-efficient çalışır.
/// </summary>
public static class Sha256Hasher
{
    /// <summary>
    /// Dosyanın SHA-256 hash değerini hesaplar.
    /// </summary>
    /// <param name="filePath">Dosya yolu</param>
    /// <returns>Küçük harfli hex string formatında hash</returns>
    public static async Task<string> ComputeHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920, // 80KB buffer
            useAsync: true);

        var hashBytes = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Dosyanın SHA-256 hash değerini senkron hesaplar.
    /// </summary>
    /// <param name="filePath">Dosya yolu</param>
    /// <returns>Küçük harfli hex string formatında hash</returns>
    public static string ComputeHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920);

        var hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Byte dizisinin SHA-256 hash değerini hesaplar.
    /// </summary>
    /// <param name="data">Hash'lenecek veri</param>
    /// <returns>Küçük harfli hex string formatında hash</returns>
    public static string ComputeHash(byte[] data)
    {
        var hashBytes = SHA256.HashData(data);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
