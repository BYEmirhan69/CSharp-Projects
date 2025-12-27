using System.Diagnostics;
using System.Runtime.InteropServices;
using KeyboardUtils.Core.Interfaces;

namespace KeyboardUtils.Services;

/// <summary>
/// Low-level keyboard hook servisi
/// Sadece Key Display Overlay veya Keyboard Assist aktifken çalışır
/// </summary>
public class KeyboardHookService : IKeyboardService
{
    // Win32 API sabitleri
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    private LowLevelKeyboardProc? _proc;
    private IntPtr _hookId = IntPtr.Zero;
    private bool _disposed;

    public bool IsRunning => _hookId != IntPtr.Zero;
    
    public event EventHandler<KeyEventData>? KeyPressed;
    public event EventHandler<KeyEventData>? KeyReleased;

    public void Start()
    {
        if (IsRunning) return;
        
        _proc = HookCallback;
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        
        if (curModule != null)
        {
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public void Stop()
    {
        if (!IsRunning) return;
        
        UnhookWindowsHookEx(_hookId);
        _hookId = IntPtr.Zero;
        _proc = null;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            var keyCode = hookStruct.vkCode;
            var keyName = ((System.Windows.Forms.Keys)keyCode).ToString();
            
            var keyData = new KeyEventData
            {
                KeyCode = keyCode,
                KeyName = keyName,
                Ctrl = (GetAsyncKeyState(0x11) & 0x8000) != 0,  // VK_CONTROL
                Alt = (GetAsyncKeyState(0x12) & 0x8000) != 0,   // VK_MENU
                Shift = (GetAsyncKeyState(0x10) & 0x8000) != 0, // VK_SHIFT
                Character = GetCharFromKey(keyCode),
                Timestamp = DateTime.Now
            };

            int msg = wParam.ToInt32();
            
            if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN)
            {
                KeyPressed?.Invoke(this, keyData);
            }
            else if (msg == WM_KEYUP || msg == WM_SYSKEYUP)
            {
                KeyReleased?.Invoke(this, keyData);
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private static char? GetCharFromKey(int vkCode)
    {
        // Basit karakter çevirimi
        if (vkCode >= 0x41 && vkCode <= 0x5A) // A-Z
        {
            return (char)vkCode;
        }
        if (vkCode >= 0x30 && vkCode <= 0x39) // 0-9
        {
            return (char)vkCode;
        }
        if (vkCode == 0x20) // Space
        {
            return ' ';
        }
        
        return null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        Stop();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
