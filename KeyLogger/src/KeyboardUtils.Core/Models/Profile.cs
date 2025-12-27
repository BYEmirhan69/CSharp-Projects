namespace KeyboardUtils.Core.Models;

/// <summary>
/// Snippet profili
/// </summary>
public class Profile
{
    /// <summary>Benzersiz tanımlayıcı</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>Profil adı</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Profil açıklaması</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Profile ait snippet'ler</summary>
    public List<Snippet> Snippets { get; set; } = new();
    
    /// <summary>Oluşturulma tarihi</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>Profil rengi (UI için)</summary>
    public string Color { get; set; } = "#FF6C5CE7";
}
