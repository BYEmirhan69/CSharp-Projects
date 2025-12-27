using System.Runtime.InteropServices;
using KeyboardUtils.App.Controls;
using KeyboardUtils.App.Forms;
using KeyboardUtils.App.Themes;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;
using KeyboardUtils.Services;

namespace KeyboardUtils.App;

/// <summary>
/// Ana uygulama formu
/// </summary>
public class MainForm : Form
{
    private const int WM_HOTKEY = 0x0312;
    
    // Services
    private readonly JsonSettingsService _settingsService;
    private readonly GlobalHotkeyService _hotkeyService;
    private readonly TypingStatisticsService _typingService;
    private readonly KeyboardHookService _keyboardService;
    private readonly SnippetExpansionService _snippetService;
    
    private AppSettings _settings = null!;
    
    // UI Elements
    private TabControl _tabControl = null!;
    private TabPage _hotkeyTab = null!;
    private TabPage _typingTab = null!;
    private TabPage _keyDisplayTab = null!;
    private TabPage _assistTab = null!;
    
    private HotkeyManagerControl _hotkeyControl = null!;
    private TypingTutorControl _typingControl = null!;
    private KeyboardAssistControl _assistControl = null!;
    
    private Button _keyDisplayButton = null!;
    private KeyDisplayOverlayForm? _overlayForm;
    
    private Label _statusLabel = null!;

    public MainForm()
    {
        // Servisleri başlat
        _settingsService = new JsonSettingsService();
        _hotkeyService = new GlobalHotkeyService();
        _typingService = new TypingStatisticsService();
        _keyboardService = new KeyboardHookService();
        _snippetService = new SnippetExpansionService(_keyboardService);
        
        InitializeForm();
        InitializeUI();
        
        // Ayarları yükle
        _ = LoadSettingsAsync();
    }

    private void InitializeForm()
    {
        Text = "Keyboard Utilities";
        Size = new Size(900, 650);
        MinimumSize = new Size(800, 550);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = DarkTheme.Background;
        ForeColor = DarkTheme.TextPrimary;
        Font = DarkTheme.FontRegular;
        
        // Form icon (opsiyonel)
        // Icon = new Icon("keyboard.ico");
    }

    private void InitializeUI()
    {
        // Header
        var headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = DarkTheme.BackgroundDark,
            Padding = new Padding(20, 0, 20, 0)
        };
        
        var titleLabel = new Label
        {
            Text = "⌨️ Keyboard Utilities",
            Font = DarkTheme.FontTitle,
            ForeColor = DarkTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 18)
        };
        
        headerPanel.Controls.Add(titleLabel);
        Controls.Add(headerPanel);
        
        // TabControl
        _tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(20, 10),
            Font = DarkTheme.FontMedium
        };
        
        CreateTabs();
        
        Controls.Add(_tabControl);
        
        // Status Bar
        var statusPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 30,
            BackColor = DarkTheme.BackgroundDark,
            Padding = new Padding(10, 0, 10, 0)
        };
        
        _statusLabel = new Label
        {
            Text = "Hazır",
            ForeColor = DarkTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(10, 7)
        };
        
        statusPanel.Controls.Add(_statusLabel);
        Controls.Add(statusPanel);
        
        // Panel sıralaması
        headerPanel.BringToFront();
        statusPanel.BringToFront();
        _tabControl.BringToFront();
    }

    private void CreateTabs()
    {
        // Hotkey Manager Tab
        _hotkeyTab = new TabPage
        {
            Text = "🔑 Hotkey Manager",
            BackColor = DarkTheme.Background,
            Padding = new Padding(10)
        };
        _hotkeyControl = new HotkeyManagerControl(_hotkeyService, _settingsService);
        _hotkeyControl.Dock = DockStyle.Fill;
        _hotkeyTab.Controls.Add(_hotkeyControl);
        _tabControl.TabPages.Add(_hotkeyTab);
        
        // Typing Tutor Tab
        _typingTab = new TabPage
        {
            Text = "📝 Typing Tutor",
            BackColor = DarkTheme.Background,
            Padding = new Padding(10)
        };
        _typingControl = new TypingTutorControl(_typingService, _settingsService);
        _typingControl.Dock = DockStyle.Fill;
        _typingTab.Controls.Add(_typingControl);
        _tabControl.TabPages.Add(_typingTab);
        
        // Key Display Tab
        _keyDisplayTab = new TabPage
        {
            Text = "👁️ Key Display",
            BackColor = DarkTheme.Background,
            Padding = new Padding(10)
        };
        CreateKeyDisplaySettings();
        _tabControl.TabPages.Add(_keyDisplayTab);
        
        // Keyboard Assist Tab
        _assistTab = new TabPage
        {
            Text = "✨ Keyboard Assist",
            BackColor = DarkTheme.Background,
            Padding = new Padding(10)
        };
        _assistControl = new KeyboardAssistControl(_snippetService, _settingsService);
        _assistControl.Dock = DockStyle.Fill;
        _assistTab.Controls.Add(_assistControl);
        _tabControl.TabPages.Add(_assistTab);
    }

    private void CreateKeyDisplaySettings()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20)
        };
        
        var titleLabel = new Label
        {
            Text = "Key Display Overlay",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 20)
        };
        panel.Controls.Add(titleLabel);
        
        var descLabel = new Label
        {
            Text = "Basılan tuşları ekranda gösteren always-on-top overlay penceresi.\nSadece siz açtığınızda aktiftir.",
            ForeColor = DarkTheme.TextSecondary,
            AutoSize = true,
            Location = new Point(20, 50)
        };
        panel.Controls.Add(descLabel);
        
        _keyDisplayButton = new Button
        {
            Text = "▶️ Overlay'i Aç",
            Size = new Size(200, 45),
            Location = new Point(20, 100)
        };
        DarkTheme.StyleButton(_keyDisplayButton, true);
        _keyDisplayButton.Click += OnKeyDisplayButtonClick;
        panel.Controls.Add(_keyDisplayButton);
        
        // Ayarlar grup kutusu
        var settingsGroup = new GroupBox
        {
            Text = "Overlay Ayarları",
            Location = new Point(20, 170),
            Size = new Size(400, 250),
            ForeColor = DarkTheme.TextSecondary
        };
        
        // Şeffaflık
        var opacityLabel = new Label { Text = "Şeffaflık:", Location = new Point(20, 30), AutoSize = true };
        settingsGroup.Controls.Add(opacityLabel);
        
        var opacityTrack = new TrackBar
        {
            Location = new Point(100, 25),
            Size = new Size(200, 45),
            Minimum = 10,
            Maximum = 100,
            Value = 90,
            TickFrequency = 10
        };
        opacityTrack.ValueChanged += (s, e) => UpdateStatus($"Şeffaflık: %{opacityTrack.Value}");
        settingsGroup.Controls.Add(opacityTrack);
        
        // Font boyutu
        var fontLabel = new Label { Text = "Font Boyutu:", Location = new Point(20, 80), AutoSize = true };
        settingsGroup.Controls.Add(fontLabel);
        
        var fontCombo = new ComboBox
        {
            Location = new Point(100, 77),
            Size = new Size(100, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        fontCombo.Items.AddRange(new object[] { "16", "20", "24", "28", "32", "36" });
        fontCombo.SelectedIndex = 2;
        DarkTheme.StyleComboBox(fontCombo);
        settingsGroup.Controls.Add(fontCombo);
        
        // Modifier göster
        var modifierCheck = new CheckBox
        {
            Text = "Ctrl, Alt, Shift tuşlarını göster",
            Location = new Point(20, 120),
            AutoSize = true,
            Checked = true
        };
        DarkTheme.StyleCheckBox(modifierCheck);
        settingsGroup.Controls.Add(modifierCheck);
        
        // Süre
        var durationLabel = new Label { Text = "Gösterim Süresi:", Location = new Point(20, 160), AutoSize = true };
        settingsGroup.Controls.Add(durationLabel);
        
        var durationCombo = new ComboBox
        {
            Location = new Point(130, 157),
            Size = new Size(100, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        durationCombo.Items.AddRange(new object[] { "1 saniye", "2 saniye", "3 saniye", "5 saniye" });
        durationCombo.SelectedIndex = 1;
        DarkTheme.StyleComboBox(durationCombo);
        settingsGroup.Controls.Add(durationCombo);
        
        panel.Controls.Add(settingsGroup);
        
        _keyDisplayTab.Controls.Add(panel);
    }

    private void OnKeyDisplayButtonClick(object? sender, EventArgs e)
    {
        if (_overlayForm == null || _overlayForm.IsDisposed)
        {
            _overlayForm = new KeyDisplayOverlayForm(_keyboardService, _settings.KeyDisplaySettings);
            _overlayForm.FormClosed += (s, e) =>
            {
                _keyDisplayButton.Text = "▶️ Overlay'i Aç";
                UpdateStatus("Overlay kapatıldı");
            };
            _overlayForm.Show();
            _keyDisplayButton.Text = "⏹️ Overlay'i Kapat";
            UpdateStatus("Overlay açıldı");
        }
        else
        {
            _overlayForm.Close();
            _overlayForm = null;
            _keyDisplayButton.Text = "▶️ Overlay'i Aç";
        }
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            _settings = await _settingsService.LoadAsync();
            _hotkeyService.SetWindowHandle(Handle);
            
            // Hotkey'leri kaydet
            if (_settings.HotkeyManagerEnabled)
            {
                _hotkeyService.RegisterAllHotkeys(_settings.Hotkeys);
            }
            
            // Control'lere ayarları aktar
            _hotkeyControl.LoadSettings(_settings);
            _typingControl.LoadSettings(_settings);
            _assistControl.LoadSettings(_settings);
            
            // Keyboard Assist aktifse başlat
            if (_settings.KeyboardAssistEnabled && _settings.ActiveProfileId != null)
            {
                var profile = _settings.Profiles.FirstOrDefault(p => p.Id == _settings.ActiveProfileId);
                if (profile != null)
                {
                    _snippetService.SetActiveProfile(profile);
                    _snippetService.IsEnabled = true;
                }
            }
            
            UpdateStatus("Ayarlar yüklendi");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Hata: {ex.Message}");
        }
    }

    private void UpdateStatus(string message)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateStatus(message));
            return;
        }
        
        _statusLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY)
        {
            int hotkeyId = m.WParam.ToInt32();
            _hotkeyService.ProcessHotkeyMessage(hotkeyId);
        }
        
        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Temizlik
        _hotkeyService.Dispose();
        _keyboardService.Dispose();
        _snippetService.Dispose();
        _overlayForm?.Close();
        
        base.OnFormClosing(e);
    }
}
