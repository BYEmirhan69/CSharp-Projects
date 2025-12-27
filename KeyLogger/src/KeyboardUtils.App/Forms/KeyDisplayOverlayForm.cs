using KeyboardUtils.App.Themes;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;
using KeyboardUtils.Services;

namespace KeyboardUtils.App.Forms;

/// <summary>
/// Always-on-top Key Display Overlay
/// Basılan tuşları ekranda gösterir
/// </summary>
public class KeyDisplayOverlayForm : Form
{
    private readonly KeyboardHookService _keyboardService;
    private readonly KeyDisplaySettings _settings;
    
    private Label _keyLabel = null!;
    private readonly Queue<string> _keyHistory = new();
    private System.Windows.Forms.Timer? _fadeTimer;
    private DateTime _lastKeyTime = DateTime.MinValue;
    
    private const int MaxHistoryLength = 5;

    public KeyDisplayOverlayForm(KeyboardHookService keyboardService, KeyDisplaySettings settings)
    {
        _keyboardService = keyboardService;
        _settings = settings;
        
        InitializeForm();
        InitializeUI();
        
        _keyboardService.KeyPressed += OnKeyPressed;
        _keyboardService.Start();
    }

    private void InitializeForm()
    {
        // Form ayarları
        Text = "Key Display";
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ShowInTaskbar = false;
        
        // Boyut ve konum
        Size = new Size(_settings.Width, _settings.Height);
        Location = new Point(_settings.PositionX, _settings.PositionY);
        
        // Şeffaflık
        Opacity = _settings.Opacity;
        
        // Arka plan rengi
        BackColor = ColorTranslator.FromHtml(_settings.BackgroundColor.Replace("#DD", "#"));
        
        // Sürüklenebilirlik için mouse eventleri
        MouseDown += OnFormMouseDown;
        MouseMove += OnFormMouseMove;
    }

    private void InitializeUI()
    {
        // Ana tuş etiketi
        _keyLabel = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", _settings.FontSize, FontStyle.Bold),
            ForeColor = ColorTranslator.FromHtml(_settings.TextColor),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            AutoSize = false
        };
        
        Controls.Add(_keyLabel);
        
        // Kapatma butonu (sağ üst köşe hover)
        var closeButton = new Label
        {
            Text = "✕",
            Font = new Font("Segoe UI", 10),
            ForeColor = DarkTheme.TextMuted,
            Size = new Size(25, 25),
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };
        closeButton.Location = new Point(Width - 30, 5);
        closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        closeButton.Click += (s, e) => Close();
        closeButton.MouseEnter += (s, e) => closeButton.ForeColor = DarkTheme.Error;
        closeButton.MouseLeave += (s, e) => closeButton.ForeColor = DarkTheme.TextMuted;
        
        Controls.Add(closeButton);
        closeButton.BringToFront();
        
        // Fade timer
        _fadeTimer = new System.Windows.Forms.Timer { Interval = 100 };
        _fadeTimer.Tick += OnFadeTimerTick;
        _fadeTimer.Start();
    }

    private void OnKeyPressed(object? sender, KeyEventData e)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnKeyPressed(sender, e));
            return;
        }
        
        // Whitelist kontrolü
        if (_settings.Whitelist.Count > 0 && !_settings.Whitelist.Contains(e.KeyName))
        {
            return;
        }
        
        // Blacklist kontrolü
        if (_settings.Blacklist.Contains(e.KeyName))
        {
            return;
        }
        
        // Modifier'ları göster veya gizle
        string displayText;
        if (_settings.ShowModifiers)
        {
            displayText = e.GetDisplayString();
        }
        else
        {
            // Sadece modifier tuşlarını gösterme
            if (e.KeyName is "ControlKey" or "Menu" or "ShiftKey" or "LWin" or "RWin" or
                "LControlKey" or "RControlKey" or "LMenu" or "RMenu" or "LShiftKey" or "RShiftKey")
            {
                return;
            }
            displayText = e.KeyName;
        }
        
        // Tuşu göster
        _keyHistory.Enqueue(displayText);
        while (_keyHistory.Count > MaxHistoryLength)
        {
            _keyHistory.Dequeue();
        }
        
        UpdateDisplay();
        _lastKeyTime = DateTime.Now;
    }

    private void UpdateDisplay()
    {
        _keyLabel.Text = string.Join("  ", _keyHistory);
        _keyLabel.ForeColor = ColorTranslator.FromHtml(_settings.TextColor);
    }

    private void OnFadeTimerTick(object? sender, EventArgs e)
    {
        // Son tuştan beri geçen süre
        var elapsed = (DateTime.Now - _lastKeyTime).TotalMilliseconds;
        
        if (elapsed > _settings.DisplayDuration && _keyHistory.Count > 0)
        {
            // Yavaşça soldurmak yerine temizle
            if (elapsed > _settings.DisplayDuration + 500)
            {
                _keyHistory.Clear();
                _keyLabel.Text = "";
            }
            else
            {
                // Fade efekti
                int alpha = (int)(255 * (1 - (elapsed - _settings.DisplayDuration) / 500));
                alpha = Math.Max(0, Math.Min(255, alpha));
                _keyLabel.ForeColor = Color.FromArgb(alpha, _keyLabel.ForeColor);
            }
        }
    }

    // Sürükleme için
    private Point _dragStart;
    private bool _isDragging;

    private void OnFormMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            _dragStart = e.Location;
        }
    }

    private void OnFormMouseMove(object? sender, MouseEventArgs e)
    {
        if (_isDragging)
        {
            var newLocation = PointToScreen(e.Location);
            Location = new Point(newLocation.X - _dragStart.X, newLocation.Y - _dragStart.Y);
            
            // Ayarları güncelle
            _settings.PositionX = Location.X;
            _settings.PositionY = Location.Y;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        _isDragging = false;
        base.OnMouseUp(e);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _fadeTimer?.Stop();
        _fadeTimer?.Dispose();
        
        _keyboardService.KeyPressed -= OnKeyPressed;
        _keyboardService.Stop();
        
        base.OnFormClosing(e);
    }

    // Click-through için WS_EX_TRANSPARENT (opsiyonel)
    // protected override CreateParams CreateParams
    // {
    //     get
    //     {
    //         CreateParams cp = base.CreateParams;
    //         cp.ExStyle |= 0x80000; // WS_EX_LAYERED
    //         cp.ExStyle |= 0x20;    // WS_EX_TRANSPARENT
    //         return cp;
    //     }
    // }
}
