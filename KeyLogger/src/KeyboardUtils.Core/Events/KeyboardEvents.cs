namespace KeyboardUtils.Core.Events;

/// <summary>
/// Keyboard ile ilgili event delegate'leri
/// </summary>
public static class KeyboardEvents
{
    /// <summary>Tuş basım delegate</summary>
    public delegate void KeyPressHandler(object sender, KeyPressEventData e);
    
    /// <summary>Snippet genişletme delegate</summary>
    public delegate void SnippetExpandedHandler(object sender, SnippetExpandedEventData e);
}

/// <summary>
/// Tuş basım event verisi
/// </summary>
public class KeyPressEventData : EventArgs
{
    public int KeyCode { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public bool IsModifier { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

/// <summary>
/// Snippet genişletme event verisi
/// </summary>
public class SnippetExpandedEventData : EventArgs
{
    public string Trigger { get; set; } = string.Empty;
    public string Expansion { get; set; } = string.Empty;
    public string ProfileName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
