using KeyboardUtils.App.Themes;
using KeyboardUtils.Core.Events;
using KeyboardUtils.Core.Models;
using KeyboardUtils.Services;

namespace KeyboardUtils.App.Controls;

/// <summary>
/// Keyboard Assist kontrolü - Snippet genişletme
/// </summary>
public class KeyboardAssistControl : UserControl
{
    private readonly SnippetExpansionService _snippetService;
    private readonly JsonSettingsService _settingsService;
    private AppSettings? _settings;
    
    // UI Elements
    private ComboBox _profileCombo = null!;
    private Button _addProfileButton = null!;
    private Button _deleteProfileButton = null!;
    private ListView _snippetListView = null!;
    private Button _addSnippetButton = null!;
    private Button _editSnippetButton = null!;
    private Button _deleteSnippetButton = null!;
    private CheckBox _enabledCheck = null!;
    private Label _statusLabel = null!;
    
    // Snippet düzenleme paneli
    private Panel _editPanel = null!;
    private TextBox _triggerTextBox = null!;
    private TextBox _expansionTextBox = null!;
    private TextBox _descriptionTextBox = null!;
    private CheckBox _caseSensitiveCheck = null!;
    private Button _saveButton = null!;
    private Button _cancelButton = null!;
    
    private Snippet? _currentEditingSnippet;
    private Profile? _currentProfile;

    public KeyboardAssistControl(SnippetExpansionService snippetService, JsonSettingsService settingsService)
    {
        _snippetService = snippetService;
        _settingsService = settingsService;
        
        _snippetService.SnippetExpanded += OnSnippetExpanded;
        
        InitializeUI();
    }

    private void InitializeUI()
    {
        BackColor = DarkTheme.Background;
        Padding = new Padding(10);
        
        // Üst panel - Profil seçimi
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100
        };
        
        var profileLabel = new Label
        {
            Text = "Aktif Profil:",
            Location = new Point(0, 5),
            AutoSize = true
        };
        DarkTheme.StyleLabel(profileLabel);
        topPanel.Controls.Add(profileLabel);
        
        _profileCombo = new ComboBox
        {
            Location = new Point(80, 2),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        DarkTheme.StyleComboBox(_profileCombo);
        _profileCombo.SelectedIndexChanged += OnProfileChanged;
        topPanel.Controls.Add(_profileCombo);
        
        _addProfileButton = new Button { Text = "➕", Size = new Size(35, 27), Location = new Point(290, 1) };
        DarkTheme.StyleButton(_addProfileButton);
        _addProfileButton.Click += OnAddProfileClick;
        topPanel.Controls.Add(_addProfileButton);
        
        _deleteProfileButton = new Button { Text = "🗑️", Size = new Size(35, 27), Location = new Point(330, 1) };
        DarkTheme.StyleButton(_deleteProfileButton);
        _deleteProfileButton.Click += OnDeleteProfileClick;
        topPanel.Controls.Add(_deleteProfileButton);
        
        _enabledCheck = new CheckBox
        {
            Text = "Keyboard Assist Aktif",
            Location = new Point(0, 45),
            AutoSize = true,
            Checked = false
        };
        DarkTheme.StyleCheckBox(_enabledCheck);
        _enabledCheck.CheckedChanged += OnEnabledChanged;
        topPanel.Controls.Add(_enabledCheck);
        
        var infoLabel = new Label
        {
            Text = "💡 Snippet yazdıktan sonra boşluk, enter veya noktalama işareti kullanın.",
            ForeColor = DarkTheme.TextMuted,
            Location = new Point(0, 75),
            AutoSize = true
        };
        topPanel.Controls.Add(infoLabel);
        
        Controls.Add(topPanel);
        
        // Sol panel - Snippet listesi
        var listPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 400,
            Padding = new Padding(0, 10, 10, 0)
        };
        
        var listLabel = new Label
        {
            Text = "Snippet'ler",
            Font = DarkTheme.FontMedium,
            Dock = DockStyle.Top,
            Height = 25
        };
        DarkTheme.StyleLabel(listLabel);
        listPanel.Controls.Add(listLabel);
        
        _snippetListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _snippetListView.Columns.Add("Trigger", 100);
        _snippetListView.Columns.Add("Genişletme", 180);
        _snippetListView.Columns.Add("Kullanım", 70);
        DarkTheme.StyleListView(_snippetListView);
        _snippetListView.SelectedIndexChanged += OnSnippetSelectionChanged;
        listPanel.Controls.Add(_snippetListView);
        
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 45,
            FlowDirection = FlowDirection.LeftToRight
        };
        
        _addSnippetButton = new Button { Text = "➕ Ekle", Size = new Size(100, 35) };
        DarkTheme.StyleButton(_addSnippetButton, true);
        _addSnippetButton.Click += OnAddSnippetClick;
        buttonPanel.Controls.Add(_addSnippetButton);
        
        _editSnippetButton = new Button { Text = "✏️ Düzenle", Size = new Size(100, 35), Enabled = false };
        DarkTheme.StyleButton(_editSnippetButton);
        _editSnippetButton.Click += OnEditSnippetClick;
        buttonPanel.Controls.Add(_editSnippetButton);
        
        _deleteSnippetButton = new Button { Text = "🗑️ Sil", Size = new Size(100, 35), Enabled = false };
        DarkTheme.StyleButton(_deleteSnippetButton);
        _deleteSnippetButton.Click += OnDeleteSnippetClick;
        buttonPanel.Controls.Add(_deleteSnippetButton);
        
        listPanel.Controls.Add(buttonPanel);
        
        Controls.Add(listPanel);
        
        // Sağ panel - Düzenleme
        CreateEditPanel();
        
        // Status
        _statusLabel = new Label
        {
            Text = "",
            ForeColor = DarkTheme.TextSecondary,
            Dock = DockStyle.Bottom,
            Height = 25
        };
        Controls.Add(_statusLabel);
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
            Text = "Snippet Ekle/Düzenle",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(20, 0)
        };
        _editPanel.Controls.Add(titleLabel);
        
        // Trigger
        var triggerLabel = new Label { Text = "Trigger (Kısaltma):", Location = new Point(20, 45), AutoSize = true };
        DarkTheme.StyleLabel(triggerLabel);
        _editPanel.Controls.Add(triggerLabel);
        
        _triggerTextBox = new TextBox
        {
            Location = new Point(20, 70),
            Size = new Size(200, 25),
            PlaceholderText = "örn: btw"
        };
        DarkTheme.StyleTextBox(_triggerTextBox);
        _editPanel.Controls.Add(_triggerTextBox);
        
        // Expansion
        var expansionLabel = new Label { Text = "Genişletme:", Location = new Point(20, 105), AutoSize = true };
        DarkTheme.StyleLabel(expansionLabel);
        _editPanel.Controls.Add(expansionLabel);
        
        _expansionTextBox = new TextBox
        {
            Location = new Point(20, 130),
            Size = new Size(300, 25),
            PlaceholderText = "örn: by the way"
        };
        DarkTheme.StyleTextBox(_expansionTextBox);
        _editPanel.Controls.Add(_expansionTextBox);
        
        // Description
        var descLabel = new Label { Text = "Açıklama (opsiyonel):", Location = new Point(20, 165), AutoSize = true };
        DarkTheme.StyleLabel(descLabel);
        _editPanel.Controls.Add(descLabel);
        
        _descriptionTextBox = new TextBox
        {
            Location = new Point(20, 190),
            Size = new Size(300, 25)
        };
        DarkTheme.StyleTextBox(_descriptionTextBox);
        _editPanel.Controls.Add(_descriptionTextBox);
        
        // Case sensitive
        _caseSensitiveCheck = new CheckBox
        {
            Text = "Büyük/küçük harf duyarlı",
            Location = new Point(20, 230),
            AutoSize = true
        };
        DarkTheme.StyleCheckBox(_caseSensitiveCheck);
        _editPanel.Controls.Add(_caseSensitiveCheck);
        
        // Butonlar
        _saveButton = new Button { Text = "💾 Kaydet", Location = new Point(20, 280), Size = new Size(120, 40) };
        DarkTheme.StyleButton(_saveButton, true);
        _saveButton.Click += OnSaveClick;
        _editPanel.Controls.Add(_saveButton);
        
        _cancelButton = new Button { Text = "❌ İptal", Location = new Point(150, 280), Size = new Size(120, 40) };
        DarkTheme.StyleButton(_cancelButton);
        _cancelButton.Click += OnCancelClick;
        _editPanel.Controls.Add(_cancelButton);
        
        Controls.Add(_editPanel);
    }

    public void LoadSettings(AppSettings settings)
    {
        _settings = settings;
        _enabledCheck.Checked = settings.KeyboardAssistEnabled;
        
        RefreshProfiles();
        
        // Aktif profili seç
        if (!string.IsNullOrEmpty(settings.ActiveProfileId))
        {
            var index = settings.Profiles.FindIndex(p => p.Id == settings.ActiveProfileId);
            if (index >= 0 && index < _profileCombo.Items.Count)
            {
                _profileCombo.SelectedIndex = index;
            }
        }
        else if (_profileCombo.Items.Count > 0)
        {
            _profileCombo.SelectedIndex = 0;
        }
    }

    private void RefreshProfiles()
    {
        _profileCombo.Items.Clear();
        
        if (_settings == null) return;
        
        foreach (var profile in _settings.Profiles)
        {
            _profileCombo.Items.Add(profile.Name);
        }
    }

    private void RefreshSnippets()
    {
        _snippetListView.Items.Clear();
        
        if (_currentProfile == null) return;
        
        foreach (var snippet in _currentProfile.Snippets)
        {
            var item = new ListViewItem(snippet.Trigger);
            item.SubItems.Add(snippet.Expansion.Length > 30 
                ? snippet.Expansion.Substring(0, 27) + "..." 
                : snippet.Expansion);
            item.SubItems.Add(snippet.UsageCount.ToString());
            item.Tag = snippet;
            
            if (!snippet.IsEnabled)
            {
                item.ForeColor = DarkTheme.TextMuted;
            }
            
            _snippetListView.Items.Add(item);
        }
        
        UpdateStatus($"{_currentProfile.Snippets.Count} snippet");
    }

    private void OnProfileChanged(object? sender, EventArgs e)
    {
        if (_settings == null || _profileCombo.SelectedIndex < 0) return;
        
        _currentProfile = _settings.Profiles[_profileCombo.SelectedIndex];
        _settings.ActiveProfileId = _currentProfile.Id;
        
        _snippetService.SetActiveProfile(_currentProfile);
        
        RefreshSnippets();
        _ = SaveSettingsAsync();
    }

    private void OnAddProfileClick(object? sender, EventArgs e)
    {
        var name = Microsoft.VisualBasic.Interaction.InputBox(
            "Profil adı girin:", "Yeni Profil", "Yeni Profil");
        
        if (string.IsNullOrWhiteSpace(name)) return;
        
        var profile = new Profile
        {
            Name = name,
            Description = $"Profil: {name}"
        };
        
        _settings?.Profiles.Add(profile);
        RefreshProfiles();
        _profileCombo.SelectedIndex = _profileCombo.Items.Count - 1;
        
        _ = SaveSettingsAsync();
    }

    private void OnDeleteProfileClick(object? sender, EventArgs e)
    {
        if (_currentProfile == null || _settings == null) return;
        
        if (_settings.Profiles.Count <= 1)
        {
            MessageBox.Show("En az bir profil olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (MessageBox.Show($"'{_currentProfile.Name}' profilini silmek istiyor musunuz?",
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _settings.Profiles.Remove(_currentProfile);
            RefreshProfiles();
            
            if (_profileCombo.Items.Count > 0)
            {
                _profileCombo.SelectedIndex = 0;
            }
            
            _ = SaveSettingsAsync();
        }
    }

    private void OnEnabledChanged(object? sender, EventArgs e)
    {
        if (_settings == null) return;
        
        _settings.KeyboardAssistEnabled = _enabledCheck.Checked;
        _snippetService.IsEnabled = _enabledCheck.Checked;
        
        if (_enabledCheck.Checked && _currentProfile != null)
        {
            _snippetService.SetActiveProfile(_currentProfile);
        }
        
        _ = SaveSettingsAsync();
        
        UpdateStatus(_enabledCheck.Checked ? "Snippet genişletme aktif" : "Snippet genişletme kapalı");
    }

    private void OnSnippetSelectionChanged(object? sender, EventArgs e)
    {
        bool hasSelection = _snippetListView.SelectedItems.Count > 0;
        _editSnippetButton.Enabled = hasSelection;
        _deleteSnippetButton.Enabled = hasSelection;
    }

    private void OnAddSnippetClick(object? sender, EventArgs e)
    {
        if (_currentProfile == null) return;
        
        _currentEditingSnippet = null;
        ClearEditForm();
        _editPanel.Visible = true;
    }

    private void OnEditSnippetClick(object? sender, EventArgs e)
    {
        if (_snippetListView.SelectedItems.Count == 0) return;
        
        _currentEditingSnippet = (Snippet)_snippetListView.SelectedItems[0].Tag;
        
        _triggerTextBox.Text = _currentEditingSnippet.Trigger;
        _expansionTextBox.Text = _currentEditingSnippet.Expansion;
        _descriptionTextBox.Text = _currentEditingSnippet.Description;
        _caseSensitiveCheck.Checked = _currentEditingSnippet.CaseSensitive;
        
        _editPanel.Visible = true;
    }

    private void OnDeleteSnippetClick(object? sender, EventArgs e)
    {
        if (_snippetListView.SelectedItems.Count == 0 || _currentProfile == null) return;
        
        var snippet = (Snippet)_snippetListView.SelectedItems[0].Tag;
        
        if (MessageBox.Show($"'{snippet.Trigger}' snippet'ini silmek istiyor musunuz?",
            "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _currentProfile.Snippets.Remove(snippet);
            RefreshSnippets();
            _ = SaveSettingsAsync();
        }
    }

    private async void OnSaveClick(object? sender, EventArgs e)
    {
        if (_currentProfile == null) return;
        
        if (string.IsNullOrWhiteSpace(_triggerTextBox.Text))
        {
            MessageBox.Show("Trigger boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        if (string.IsNullOrWhiteSpace(_expansionTextBox.Text))
        {
            MessageBox.Show("Genişletme boş olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
        var snippet = _currentEditingSnippet ?? new Snippet();
        
        snippet.Trigger = _triggerTextBox.Text.Trim();
        snippet.Expansion = _expansionTextBox.Text;
        snippet.Description = _descriptionTextBox.Text;
        snippet.CaseSensitive = _caseSensitiveCheck.Checked;
        
        if (_currentEditingSnippet == null)
        {
            _currentProfile.Snippets.Add(snippet);
        }
        
        await SaveSettingsAsync();
        RefreshSnippets();
        _editPanel.Visible = false;
        
        UpdateStatus($"'{snippet.Trigger}' kaydedildi");
    }

    private void OnCancelClick(object? sender, EventArgs e)
    {
        _editPanel.Visible = false;
        ClearEditForm();
    }

    private void ClearEditForm()
    {
        _triggerTextBox.Text = "";
        _expansionTextBox.Text = "";
        _descriptionTextBox.Text = "";
        _caseSensitiveCheck.Checked = false;
    }

    private void OnSnippetExpanded(object? sender, SnippetExpandedEventData e)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnSnippetExpanded(sender, e));
            return;
        }
        
        UpdateStatus($"Genişletildi: {e.Trigger} → {e.Expansion}");
        RefreshSnippets(); // Kullanım sayısını güncelle
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
