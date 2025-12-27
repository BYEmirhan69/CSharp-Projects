using KeyboardUtils.App.Themes;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;
using KeyboardUtils.Services;

namespace KeyboardUtils.App.Controls;

/// <summary>
/// Hotkey yönetimi kontrolü
/// </summary>
public class HotkeyManagerControl : UserControl
{
    private readonly GlobalHotkeyService _hotkeyService;
    private readonly JsonSettingsService _settingsService;
    private AppSettings? _settings;
    
    // UI Elements
    private ListView _hotkeyListView = null!;
    private Button _addButton = null!;
    private Button _editButton = null!;
    private Button _deleteButton = null!;
    private Button _testButton = null!;
    private CheckBox _enabledCheck = null!;
    private Label _statusLabel = null!;
    
    // Yeni hotkey ekleme paneli
    private Panel _editPanel = null!;
    private TextBox _nameTextBox = null!;
    private TextBox _hotkeyTextBox = null!;
    private ComboBox _actionTypeCombo = null!;
    private TextBox _actionDataTextBox = null!;
    private Button _saveButton = null!;
    private Button _cancelButton = null!;
    private Button _browseButton = null!;
    
    private HotkeyAction? _currentEditingHotkey;
    private bool _isCapturingHotkey;
    private int _capturedKey;
    private bool _capturedCtrl, _capturedAlt, _capturedShift, _capturedWin;

    public HotkeyManagerControl(GlobalHotkeyService hotkeyService, JsonSettingsService settingsService)
    {
        _hotkeyService = hotkeyService;
        _settingsService = settingsService;
        
        InitializeUI();
    }

    private void InitializeUI()
    {
        BackColor = DarkTheme.Background;
        Padding = new Padding(10);
        
        // Sol panel - Liste
        var listPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 450,
            Padding = new Padding(0, 0, 10, 0)
        };
        
        var listLabel = new Label
        {
            Text = "Kayıtlı Hotkey'ler",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        listPanel.Controls.Add(listLabel);
        
        _hotkeyListView = new ListView
        {
            Location = new Point(0, 35),
            Size = new Size(430, 280),
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false
        };
        _hotkeyListView.Columns.Add("Ad", 120);
        _hotkeyListView.Columns.Add("Hotkey", 120);
        _hotkeyListView.Columns.Add("Aksiyon", 80);
        _hotkeyListView.Columns.Add("Durum", 60);
        DarkTheme.StyleListView(_hotkeyListView);
        _hotkeyListView.SelectedIndexChanged += OnHotkeySelectionChanged;
        listPanel.Controls.Add(_hotkeyListView);
        
        // Butonlar
        var buttonPanel = new FlowLayoutPanel
        {
            Location = new Point(0, 325),
            Size = new Size(430, 45),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };
        
        _addButton = new Button { Text = "➕ Ekle", Size = new Size(100, 35) };
        DarkTheme.StyleButton(_addButton, true);
        _addButton.Click += OnAddClick;
        buttonPanel.Controls.Add(_addButton);
        
        _editButton = new Button { Text = "✏️ Düzenle", Size = new Size(100, 35), Enabled = false };
        DarkTheme.StyleButton(_editButton);
        _editButton.Click += OnEditClick;
        buttonPanel.Controls.Add(_editButton);
        
        _deleteButton = new Button { Text = "🗑️ Sil", Size = new Size(100, 35), Enabled = false };
        DarkTheme.StyleButton(_deleteButton);
        _deleteButton.Click += OnDeleteClick;
        buttonPanel.Controls.Add(_deleteButton);
        
        _testButton = new Button { Text = "▶️ Test", Size = new Size(100, 35), Enabled = false };
        DarkTheme.StyleButton(_testButton);
        _testButton.Click += OnTestClick;
        buttonPanel.Controls.Add(_testButton);
        
        listPanel.Controls.Add(buttonPanel);
        
        _enabledCheck = new CheckBox
        {
            Text = "Hotkey Manager Aktif",
            Location = new Point(0, 380),
            AutoSize = true,
            Checked = true
        };
        DarkTheme.StyleCheckBox(_enabledCheck);
        _enabledCheck.CheckedChanged += OnEnabledChanged;
        listPanel.Controls.Add(_enabledCheck);
        
        _statusLabel = new Label
        {
            Text = "",
            ForeColor = DarkTheme.TextSecondary,
            Location = new Point(0, 410),
            AutoSize = true
        };
        listPanel.Controls.Add(_statusLabel);
        
        Controls.Add(listPanel);
        
        // Sağ panel - Düzenleme
        CreateEditPanel();
    }

    private void CreateEditPanel()
    {
        _editPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Padding = new Padding(20)
        };
        
        var titleLabel = new Label
        {
            Text = "Hotkey Ekle/Düzenle",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 0)
        };
        _editPanel.Controls.Add(titleLabel);
        
        // Ad
        var nameLabel = new Label { Text = "Ad:", Location = new Point(20, 45), AutoSize = true };
        _editPanel.Controls.Add(nameLabel);
        
        _nameTextBox = new TextBox { Location = new Point(20, 70), Size = new Size(300, 25) };
        DarkTheme.StyleTextBox(_nameTextBox);
        _editPanel.Controls.Add(_nameTextBox);
        
        // Hotkey
        var hotkeyLabel = new Label { Text = "Hotkey (tıklayıp tuşlara basın):", Location = new Point(20, 105), AutoSize = true };
        _editPanel.Controls.Add(hotkeyLabel);
        
        _hotkeyTextBox = new TextBox
        {
            Location = new Point(20, 130),
            Size = new Size(300, 25),
            ReadOnly = true,
            PlaceholderText = "Tıklayın ve tuş kombinasyonuna basın..."
        };
        DarkTheme.StyleTextBox(_hotkeyTextBox);
        _hotkeyTextBox.MouseDown += OnHotkeyTextBoxClick;
        _hotkeyTextBox.KeyDown += OnHotkeyTextBoxKeyDown;
        _hotkeyTextBox.LostFocus += (s, e) => { _isCapturingHotkey = false; };
        _editPanel.Controls.Add(_hotkeyTextBox);
        
        // Aksiyon tipi
        var actionLabel = new Label { Text = "Aksiyon Tipi:", Location = new Point(20, 165), AutoSize = true };
        _editPanel.Controls.Add(actionLabel);
        
        _actionTypeCombo = new ComboBox
        {
            Location = new Point(20, 190),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _actionTypeCombo.Items.AddRange(new object[] { "Uygulama Çalıştır", "URL Aç", "Metin Yaz", "Komut Çalıştır" });
        _actionTypeCombo.SelectedIndex = 0;
        DarkTheme.StyleComboBox(_actionTypeCombo);
        _actionTypeCombo.SelectedIndexChanged += OnActionTypeChanged;
        _editPanel.Controls.Add(_actionTypeCombo);
        
        // Aksiyon verisi
        var actionDataLabel = new Label { Text = "Aksiyon Verisi:", Location = new Point(20, 225), AutoSize = true };
        _editPanel.Controls.Add(actionDataLabel);
        
        _actionDataTextBox = new TextBox { Location = new Point(20, 250), Size = new Size(250, 25) };
        DarkTheme.StyleTextBox(_actionDataTextBox);
        _editPanel.Controls.Add(_actionDataTextBox);
        
        _browseButton = new Button { Text = "...", Location = new Point(275, 248), Size = new Size(45, 29) };
        DarkTheme.StyleButton(_browseButton);
        _browseButton.Click += OnBrowseClick;
        _editPanel.Controls.Add(_browseButton);
        
        // Kaydet/İptal
        _saveButton = new Button { Text = "💾 Kaydet", Location = new Point(20, 300), Size = new Size(120, 40) };
        DarkTheme.StyleButton(_saveButton, true);
        _saveButton.Click += OnSaveClick;
        _editPanel.Controls.Add(_saveButton);
        
        _cancelButton = new Button { Text = "❌ İptal", Location = new Point(150, 300), Size = new Size(120, 40) };
        DarkTheme.StyleButton(_cancelButton);
        _cancelButton.Click += OnCancelClick;
        _editPanel.Controls.Add(_cancelButton);
        
        Controls.Add(_editPanel);
    }

    public void LoadSettings(AppSettings settings)
    {
        _settings = settings;
        _enabledCheck.Checked = settings.HotkeyManagerEnabled;
        RefreshList();
    }

    private void RefreshList()
    {
        _hotkeyListView.Items.Clear();
        
        if (_settings == null) return;
        
        foreach (var hotkey in _settings.Hotkeys)
        {
            var item = new ListViewItem(hotkey.Name);
            item.SubItems.Add(hotkey.GetHotkeyString());
            item.SubItems.Add(hotkey.ActionType.ToString());
            item.SubItems.Add(hotkey.IsEnabled ? "✓" : "✗");
            item.Tag = hotkey;
            _hotkeyListView.Items.Add(item);
        }
        
        UpdateStatus($"{_settings.Hotkeys.Count} hotkey kayıtlı");
    }

    private void OnHotkeySelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = _hotkeyListView.SelectedItems.Count > 0;
        _editButton.Enabled = hasSelection;
        _deleteButton.Enabled = hasSelection;
        _testButton.Enabled = hasSelection;
    }

    private void OnAddClick(object? sender, EventArgs e)
    {
        _currentEditingHotkey = null;
        ClearEditForm();
        _editPanel.Visible = true;
    }

    private void OnEditClick(object? sender, EventArgs e)
    {
        if (_hotkeyListView.SelectedItems.Count == 0) return;
        
        _currentEditingHotkey = (HotkeyAction)_hotkeyListView.SelectedItems[0].Tag;
        
        _nameTextBox.Text = _currentEditingHotkey.Name;
        _hotkeyTextBox.Text = _currentEditingHotkey.GetHotkeyString();
        _capturedKey = _currentEditingHotkey.Key;
        _capturedCtrl = _currentEditingHotkey.Ctrl;
        _capturedAlt = _currentEditingHotkey.Alt;
        _capturedShift = _currentEditingHotkey.Shift;
        _capturedWin = _currentEditingHotkey.Win;
        _actionTypeCombo.SelectedIndex = (int)_currentEditingHotkey.ActionType;
        _actionDataTextBox.Text = _currentEditingHotkey.ActionData;
        
        _editPanel.Visible = true;
    }

    private void OnDeleteClick(object? sender, EventArgs e)
    {
        if (_hotkeyListView.SelectedItems.Count == 0 || _settings == null) return;
        
        var hotkey = (HotkeyAction)_hotkeyListView.SelectedItems[0].Tag;
        
        if (MessageBox.Show($"'{hotkey.Name}' hotkey'ini silmek istiyor musunuz?", 
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _hotkeyService.UnregisterHotkey(hotkey.Id);
            _settings.Hotkeys.Remove(hotkey);
            _ = SaveSettingsAsync();
            RefreshList();
        }
    }

    private void OnTestClick(object? sender, EventArgs e)
    {
        if (_hotkeyListView.SelectedItems.Count == 0) return;
        
        var hotkey = (HotkeyAction)_hotkeyListView.SelectedItems[0].Tag;
        _hotkeyService.ProcessHotkeyMessage(0); // Test için direkt execute
        
        UpdateStatus($"'{hotkey.Name}' test edildi");
    }

    private void OnEnabledChanged(object? sender, EventArgs e)
    {
        if (_settings == null) return;
        
        _settings.HotkeyManagerEnabled = _enabledCheck.Checked;
        _hotkeyService.IsEnabled = _enabledCheck.Checked;
        
        if (_enabledCheck.Checked)
        {
            _hotkeyService.RegisterAllHotkeys(_settings.Hotkeys);
        }
        else
        {
            _hotkeyService.UnregisterAllHotkeys();
        }
        
        _ = SaveSettingsAsync();
    }

    private void OnHotkeyTextBoxClick(object? sender, MouseEventArgs e)
    {
        _isCapturingHotkey = true;
        _hotkeyTextBox.Text = "Tuşa basın...";
        _hotkeyTextBox.Focus();
    }

    private void OnHotkeyTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (!_isCapturingHotkey) return;
        
        e.SuppressKeyPress = true;
        
        // Modifier tuşları
        _capturedCtrl = e.Control;
        _capturedAlt = e.Alt;
        _capturedShift = e.Shift;
        
        // Ana tuş (modifier olmayan)
        if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu && 
            e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.LWin && e.KeyCode != Keys.RWin)
        {
            _capturedKey = (int)e.KeyCode;
            
            var parts = new List<string>();
            if (_capturedCtrl) parts.Add("Ctrl");
            if (_capturedAlt) parts.Add("Alt");
            if (_capturedShift) parts.Add("Shift");
            parts.Add(e.KeyCode.ToString());
            
            _hotkeyTextBox.Text = string.Join(" + ", parts);
            _isCapturingHotkey = false;
        }
    }

    private void OnActionTypeChanged(object? sender, EventArgs e)
    {
        _browseButton.Visible = _actionTypeCombo.SelectedIndex == 0; // Sadece uygulama için
        
        _actionDataTextBox.PlaceholderText = _actionTypeCombo.SelectedIndex switch
        {
            0 => "Uygulama yolu (örn: notepad.exe)",
            1 => "URL (örn: https://google.com)",
            2 => "Yazılacak metin",
            3 => "Komut (örn: ipconfig /all)",
            _ => ""
        };
    }

    private void OnBrowseClick(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Çalıştırılabilir|*.exe|Tüm dosyalar|*.*",
            Title = "Uygulama Seç"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _actionDataTextBox.Text = dialog.FileName;
        }
    }

    private async void OnSaveClick(object? sender, EventArgs e)
    {
        if (_settings == null) return;
        
        if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
        {
            MessageBox.Show("Lütfen bir ad girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (_capturedKey == 0)
        {
            MessageBox.Show("Lütfen bir hotkey belirleyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var hotkey = _currentEditingHotkey ?? new HotkeyAction();
        
        hotkey.Name = _nameTextBox.Text;
        hotkey.Key = _capturedKey;
        hotkey.Ctrl = _capturedCtrl;
        hotkey.Alt = _capturedAlt;
        hotkey.Shift = _capturedShift;
        hotkey.ActionType = (ActionType)_actionTypeCombo.SelectedIndex;
        hotkey.ActionData = _actionDataTextBox.Text;
        
        if (_currentEditingHotkey == null)
        {
            _settings.Hotkeys.Add(hotkey);
        }
        
        // Hotkey'i yeniden kaydet
        _hotkeyService.UnregisterHotkey(hotkey.Id);
        if (hotkey.IsEnabled && _settings.HotkeyManagerEnabled)
        {
            _hotkeyService.RegisterHotkey(hotkey);
        }
        
        await SaveSettingsAsync();
        RefreshList();
        _editPanel.Visible = false;
        
        UpdateStatus($"'{hotkey.Name}' kaydedildi");
    }

    private void OnCancelClick(object? sender, EventArgs e)
    {
        _editPanel.Visible = false;
        ClearEditForm();
    }

    private void ClearEditForm()
    {
        _nameTextBox.Text = "";
        _hotkeyTextBox.Text = "";
        _capturedKey = 0;
        _capturedCtrl = _capturedAlt = _capturedShift = _capturedWin = false;
        _actionTypeCombo.SelectedIndex = 0;
        _actionDataTextBox.Text = "";
    }

    private async Task SaveSettingsAsync()
    {
        if (_settings == null) return;
        await _settingsService.SaveAsync(_settings);
    }

    private void UpdateStatus(string message)
    {
        _statusLabel.Text = message;
    }
}
