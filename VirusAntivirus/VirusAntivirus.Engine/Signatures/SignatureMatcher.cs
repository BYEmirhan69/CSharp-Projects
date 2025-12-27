namespace VirusAntivirus.Engine.Signatures;

/// <summary>
/// Hash eşleştirme sonucu
/// </summary>
public class SignatureMatchResult
{
    /// <summary>
    /// Eşleşme bulundu mu
    /// </summary>
    public bool IsMatch { get; set; }

    /// <summary>
    /// Eşleşen imza (varsa)
    /// </summary>
    public Signature? MatchedSignature { get; set; }

    /// <summary>
    /// Tehdit adı
    /// </summary>
    public string ThreatName => MatchedSignature?.Name ?? string.Empty;

    /// <summary>
    /// Tehdit seviyesi
    /// </summary>
    public string Severity => MatchedSignature?.Severity ?? string.Empty;
}

/// <summary>
/// SHA-256 hash tabanlı imza eşleştirme servisi.
/// </summary>
public class SignatureMatcher
{
    private readonly SignatureDatabase _database;

    public SignatureMatcher(SignatureDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    /// <summary>
    /// Verilen hash değerini imza veritabanıyla karşılaştırır.
    /// </summary>
    /// <param name="sha256Hash">Dosyanın SHA-256 hash değeri</param>
    /// <returns>Eşleştirme sonucu</returns>
    public SignatureMatchResult Match(string sha256Hash)
    {
        if (string.IsNullOrWhiteSpace(sha256Hash))
        {
            return new SignatureMatchResult { IsMatch = false };
        }

        var signature = _database.FindByHash(sha256Hash);

        return new SignatureMatchResult
        {
            IsMatch = signature != null,
            MatchedSignature = signature
        };
    }
}
