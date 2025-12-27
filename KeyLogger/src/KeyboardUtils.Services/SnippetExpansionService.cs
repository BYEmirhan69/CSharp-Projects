using System.Text;
using KeyboardUtils.Core.Events;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Services;

/// <summary>
/// Snippet genişletme servisi
/// </summary>
public class SnippetExpansionService : IDisposable
{
    private readonly IKeyboardService _keyboardService;
    private readonly StringBuilder _buffer = new();
    private Profile? _activeProfile;
    private bool _isEnabled;
    private bool _disposed;
    
    private const int MaxBufferLength = 50;

    public event EventHandler<SnippetExpandedEventData>? SnippetExpanded;
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value) return;
            
            _isEnabled = value;
            
            if (_isEnabled)
            {
                _keyboardService.Start();
            }
            else
            {
                _keyboardService.Stop();
                _buffer.Clear();
            }
        }
    }

    public SnippetExpansionService(IKeyboardService keyboardService)
    {
        _keyboardService = keyboardService;
        _keyboardService.KeyPressed += OnKeyPressed;
    }

    public void SetActiveProfile(Profile profile)
    {
        _activeProfile = profile;
        _buffer.Clear();
    }

    private void OnKeyPressed(object? sender, KeyEventData e)
    {
        if (!_isEnabled || _activeProfile == null) return;

        // Trigger karakteri (Space, Enter, Tab, Noktalama)
        bool isTriggerKey = e.KeyCode == 0x20 || // Space
                           e.KeyCode == 0x0D || // Enter
                           e.KeyCode == 0x09 || // Tab
                           IsPunctuation(e.KeyCode);

        if (isTriggerKey)
        {
            CheckAndExpand();
            _buffer.Clear();
            return;
        }

        // Backspace
        if (e.KeyCode == 0x08 && _buffer.Length > 0)
        {
            _buffer.Remove(_buffer.Length - 1, 1);
            return;
        }

        // Escape veya modifier tuşları - buffer temizle
        if (e.KeyCode == 0x1B || e.Ctrl || e.Alt)
        {
            _buffer.Clear();
            return;
        }

        // Karakter ekle
        if (e.Character.HasValue)
        {
            char c = e.Character.Value;
            
            // Shift durumuna göre küçük harf
            if (!e.Shift && c >= 'A' && c <= 'Z')
            {
                c = char.ToLower(c);
            }
            
            _buffer.Append(c);
            
            if (_buffer.Length > MaxBufferLength)
            {
                _buffer.Remove(0, _buffer.Length - MaxBufferLength);
            }
        }
    }

    private void CheckAndExpand()
    {
        if (_activeProfile == null || _buffer.Length == 0) return;

        string typed = _buffer.ToString();
        
        foreach (var snippet in _activeProfile.Snippets.Where(s => s.IsEnabled))
        {
            string trigger = snippet.CaseSensitive ? snippet.Trigger : snippet.Trigger.ToLower();
            string compare = snippet.CaseSensitive ? typed : typed.ToLower();
            
            if (compare.EndsWith(trigger))
            {
                ExpandSnippet(snippet, trigger.Length);
                return;
            }
        }
    }

    private void ExpandSnippet(Snippet snippet, int triggerLength)
    {
        try
        {
            // Trigger'ı sil (backspace gönder)
            for (int i = 0; i < triggerLength; i++)
            {
                System.Windows.Forms.SendKeys.SendWait("{BACKSPACE}");
            }
            
            // Expansion'ı yaz
            // SendKeys için özel karakterleri escape'le
            string escapedExpansion = EscapeSendKeys(snippet.Expansion);
            System.Windows.Forms.SendKeys.SendWait(escapedExpansion);
            
            snippet.UsageCount++;
            
            SnippetExpanded?.Invoke(this, new SnippetExpandedEventData
            {
                Trigger = snippet.Trigger,
                Expansion = snippet.Expansion,
                ProfileName = _activeProfile?.Name ?? "",
                Timestamp = DateTime.Now
            });
        }
        catch
        {
            // Hata durumunda sessizce devam et
        }
    }

    private static string EscapeSendKeys(string text)
    {
        // SendKeys için özel karakterleri escape'le
        return text
            .Replace("{", "{{")
            .Replace("}", "}}")
            .Replace("[", "{[}")
            .Replace("]", "{]}")
            .Replace("(", "{(}")
            .Replace(")", "{)}")
            .Replace("+", "{+}")
            .Replace("^", "{^}")
            .Replace("%", "{%}")
            .Replace("~", "{~}");
    }

    private static bool IsPunctuation(int keyCode)
    {
        // Noktalama işaretleri
        return keyCode == 0xBE || // .
               keyCode == 0xBC || // ,
               keyCode == 0xBA || // ;
               keyCode == 0xBF || // /
               keyCode == 0xBB || // =
               keyCode == 0xBD;   // -
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _keyboardService.KeyPressed -= OnKeyPressed;
        IsEnabled = false;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
