namespace VirusAntivirus.Engine.Heuristics;

/// <summary>
/// Heuristik analiz sonucu
/// </summary>
public class HeuristicResult
{
    /// <summary>
    /// Toplam risk skoru (0-100)
    /// </summary>
    public int RiskScore { get; set; }

    /// <summary>
    /// Risk seviyesi
    /// </summary>
    public RiskLevel Level
    {
        get
        {
            return RiskScore switch
            {
                >= 70 => RiskLevel.High,
                >= 40 => RiskLevel.Medium,
                >= 20 => RiskLevel.Low,
                _ => RiskLevel.None
            };
        }
    }

    /// <summary>
    /// Tespit edilen bulgular listesi
    /// </summary>
    public List<HeuristicFinding> Findings { get; set; } = new();

    /// <summary>
    /// Şüpheli kabul edilip edilmediği
    /// </summary>
    public bool IsSuspicious => RiskScore >= 70;

    /// <summary>
    /// Bulguların özet açıklaması
    /// </summary>
    public string Summary
    {
        get
        {
            if (Findings.Count == 0)
                return "Şüpheli bulgu yok";

            return string.Join("; ", Findings.Select(f => f.Description));
        }
    }
}

/// <summary>
/// Risk seviyeleri
/// </summary>
public enum RiskLevel
{
    /// <summary>
    /// Risk yok
    /// </summary>
    None,

    /// <summary>
    /// Düşük risk
    /// </summary>
    Low,

    /// <summary>
    /// Orta risk
    /// </summary>
    Medium,

    /// <summary>
    /// Yüksek risk
    /// </summary>
    High
}
