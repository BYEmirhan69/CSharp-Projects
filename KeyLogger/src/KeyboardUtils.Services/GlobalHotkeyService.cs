using System.Diagnostics;
using System.Runtime.InteropServices;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Services;

/// <summary>
/// Win32 API kullanarak global hotkey yönetimi
/// </summary>
public class GlobalHotkeyService : IHotkeyService
{
    // Win32 API sabitleri
    private const int WM_HOTKEY = 0x0312;
    private const int MOD_ALT = 0x0001;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int MOD_WIN = 0x0008;
    private const int MOD_NOREPEAT = 0x4000;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly Dictionary<int, HotkeyAction> _registeredHotkeys = new();
    private readonly Dictionary<string, int> _hotkeyIdMap = new();
    private int _nextHotkeyId = 1;
    private IntPtr _windowHandle;
    private bool _disposed;

    public IReadOnlyList<HotkeyAction> RegisteredHotkeys => _registeredHotkeys.Values.ToList().AsReadOnly();
    public bool IsEnabled { get; set; } = true;
    
    public event EventHandler<HotkeyTriggeredEventArgs>? HotkeyTriggered;

    public void SetWindowHandle(IntPtr handle)
    {
        _windowHandle = handle;
    }

    public bool RegisterHotkey(HotkeyAction hotkey)
    {
        if (_windowHandle == IntPtr.Zero || !hotkey.IsEnabled)
            return false;

        try
        {
            // Çakışma kontrolü
            if (_hotkeyIdMap.ContainsKey(hotkey.Id))
            {
                UnregisterHotkey(hotkey.Id);
            }

            int modifiers = MOD_NOREPEAT;
            if (hotkey.Ctrl) modifiers |= MOD_CONTROL;
            if (hotkey.Alt) modifiers |= MOD_ALT;
            if (hotkey.Shift) modifiers |= MOD_SHIFT;
            if (hotkey.Win) modifiers |= MOD_WIN;

            int hotkeyId = _nextHotkeyId++;
            
            if (RegisterHotKey(_windowHandle, hotkeyId, modifiers, hotkey.Key))
            {
                _registeredHotkeys[hotkeyId] = hotkey;
                _hotkeyIdMap[hotkey.Id] = hotkeyId;
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }

    public bool UnregisterHotkey(string hotkeyId)
    {
        if (!_hotkeyIdMap.TryGetValue(hotkeyId, out int id))
            return false;

        try
        {
            UnregisterHotKey(_windowHandle, id);
            _registeredHotkeys.Remove(id);
            _hotkeyIdMap.Remove(hotkeyId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void RegisterAllHotkeys(IEnumerable<HotkeyAction> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            RegisterHotkey(hotkey);
        }
    }

    public void UnregisterAllHotkeys()
    {
        foreach (var id in _registeredHotkeys.Keys.ToList())
        {
            UnregisterHotKey(_windowHandle, id);
        }
        _registeredHotkeys.Clear();
        _hotkeyIdMap.Clear();
    }

    /// <summary>
    /// WndProc'den çağrılmalı
    /// </summary>
    public void ProcessHotkeyMessage(int hotkeyId)
    {
        if (!IsEnabled) return;
        
        if (_registeredHotkeys.TryGetValue(hotkeyId, out var hotkey))
        {
            HotkeyTriggered?.Invoke(this, new HotkeyTriggeredEventArgs(hotkey));
            ExecuteAction(hotkey);
        }
    }

    private void ExecuteAction(HotkeyAction hotkey)
    {
        try
        {
            switch (hotkey.ActionType)
            {
                case ActionType.RunApplication:
                    if (!string.IsNullOrEmpty(hotkey.ActionData))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = hotkey.ActionData,
                            UseShellExecute = true
                        });
                    }
                    break;

                case ActionType.OpenUrl:
                    if (!string.IsNullOrEmpty(hotkey.ActionData))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = hotkey.ActionData,
                            UseShellExecute = true
                        });
                    }
                    break;

                case ActionType.TypeText:
                    if (!string.IsNullOrEmpty(hotkey.ActionData))
                    {
                        System.Windows.Forms.SendKeys.SendWait(hotkey.ActionData);
                    }
                    break;

                case ActionType.RunCommand:
                    if (!string.IsNullOrEmpty(hotkey.ActionData))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c {hotkey.ActionData}",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Hotkey action error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        UnregisterAllHotkeys();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
