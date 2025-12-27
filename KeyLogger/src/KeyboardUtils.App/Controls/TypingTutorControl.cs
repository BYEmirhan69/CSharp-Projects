using System.Diagnostics;
using KeyboardUtils.App.Themes;
using KeyboardUtils.Core.Models;
using KeyboardUtils.Services;

namespace KeyboardUtils.App.Controls;

/// <summary>
/// Typing Tutor kontrolü - Yazma pratiği
/// </summary>
public class TypingTutorControl : UserControl
{
    private readonly TypingStatisticsService _typingService;
    private readonly JsonSettingsService _settingsService;
    private AppSettings? _settings;
    
    // UI Elements
    private ComboBox _textSelector = null!;
    private RichTextBox _targetTextBox = null!;
    private RichTextBox _inputTextBox = null!;
    private Button _startButton = null!;
    private Button _resetButton = null!;
    private Label _wpmLabel = null!;
    private Label _accuracyLabel = null!;
    private Label _timeLabel = null!;
    private Label _statusLabel = null!;
    private ListView _historyListView = null!;
    private Button _exportCsvButton = null!;
    private Button _exportJsonButton = null!;
    
    // State
    private TypingSession? _currentSession;
    private readonly Stopwatch _stopwatch = new();
    private System.Windows.Forms.Timer? _updateTimer;
    private List<PracticeText> _practiceTexts = new();

    public TypingTutorControl(TypingStatisticsService typingService, JsonSettingsService settingsService)
    {
        _typingService = typingService;
        _settingsService = settingsService;
        
        InitializeUI();
        LoadPracticeTexts();
    }

    private void InitializeUI()
    {
        BackColor = DarkTheme.Background;
        Padding = new Padding(10);
        
        // Üst panel - Metin seçimi ve istatistikler
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80
        };
        
        var selectLabel = new Label
        {
            Text = "Pratik Metni:",
            Location = new Point(0, 5),
            AutoSize = true
        };
        DarkTheme.StyleLabel(selectLabel);
        topPanel.Controls.Add(selectLabel);
        
        _textSelector = new ComboBox
        {
            Location = new Point(90, 2),
            Size = new Size(300, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        DarkTheme.StyleComboBox(_textSelector);
        _textSelector.SelectedIndexChanged += OnTextSelected;
        topPanel.Controls.Add(_textSelector);
        
        // İstatistikler
        _wpmLabel = new Label
        {
            Text = "WPM: --",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.Primary,
            Location = new Point(0, 45),
            AutoSize = true
        };
        topPanel.Controls.Add(_wpmLabel);
        
        _accuracyLabel = new Label
        {
            Text = "Doğruluk: --%",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.Success,
            Location = new Point(120, 45),
            AutoSize = true
        };
        topPanel.Controls.Add(_accuracyLabel);
        
        _timeLabel = new Label
        {
            Text = "Süre: 00:00",
            Font = DarkTheme.FontLarge,
            ForeColor = DarkTheme.Secondary,
            Location = new Point(280, 45),
            AutoSize = true
        };
        topPanel.Controls.Add(_timeLabel);
        
        Controls.Add(topPanel);
        
        // Orta panel - Hedef ve giriş metinleri
        var middlePanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 10, 0, 10)
        };
        
        // Hedef metin
        var targetLabel = new Label
        {
            Text = "Hedef Metin:",
            Dock = DockStyle.Top,
            Height = 25
        };
        DarkTheme.StyleLabel(targetLabel);
        middlePanel.Controls.Add(targetLabel);
        
        _targetTextBox = new RichTextBox
        {
            Dock = DockStyle.Top,
            Height = 100,
            ReadOnly = true,
            Font = new Font("Cascadia Code", 12)
        };
        DarkTheme.StyleRichTextBox(_targetTextBox);
        middlePanel.Controls.Add(_targetTextBox);
        
        // Giriş metni
        var inputLabel = new Label
        {
            Text = "Yazmaya Başlayın:",
            Dock = DockStyle.Top,
            Height = 35,
            Padding = new Padding(0, 10, 0, 0)
        };
        DarkTheme.StyleLabel(inputLabel);
        middlePanel.Controls.Add(inputLabel);
        
        _inputTextBox = new RichTextBox
        {
            Dock = DockStyle.Top,
            Height = 100,
            Font = new Font("Cascadia Code", 12)
        };
        DarkTheme.StyleRichTextBox(_inputTextBox);
        _inputTextBox.TextChanged += OnInputTextChanged;
        _inputTextBox.KeyDown += OnInputKeyDown;
        middlePanel.Controls.Add(_inputTextBox);
        
        // Butonlar
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 55,
            Padding = new Padding(0, 10, 0, 0),
            FlowDirection = FlowDirection.LeftToRight
        };
        
        _startButton = new Button { Text = "▶️ Başla", Size = new Size(120, 40) };
        DarkTheme.StyleButton(_startButton, true);
        _startButton.Click += OnStartClick;
        buttonPanel.Controls.Add(_startButton);
        
        _resetButton = new Button { Text = "🔄 Sıfırla", Size = new Size(120, 40), Enabled = false };
        DarkTheme.StyleButton(_resetButton);
        _resetButton.Click += OnResetClick;
        buttonPanel.Controls.Add(_resetButton);
        
        middlePanel.Controls.Add(buttonPanel);
        
        // Sıralama düzelt
        buttonPanel.BringToFront();
        _inputTextBox.BringToFront();
        inputLabel.BringToFront();
        _targetTextBox.BringToFront();
        targetLabel.BringToFront();
        
        Controls.Add(middlePanel);
        
        // Alt panel - Geçmiş
        var bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 180
        };
        
        var historyLabel = new Label
        {
            Text = "Geçmiş",
            Font = DarkTheme.FontMedium,
            Dock = DockStyle.Top,
            Height = 25
        };
        DarkTheme.StyleLabel(historyLabel);
        bottomPanel.Controls.Add(historyLabel);
        
        _historyListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };
        _historyListView.Columns.Add("Tarih", 130);
        _historyListView.Columns.Add("WPM", 70);
        _historyListView.Columns.Add("Doğruluk", 80);
        _historyListView.Columns.Add("Süre", 80);
        _historyListView.Columns.Add("Zorluk", 80);
        DarkTheme.StyleListView(_historyListView);
        bottomPanel.Controls.Add(_historyListView);
        
        // Export butonları
        var exportPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            FlowDirection = FlowDirection.RightToLeft
        };
        
        _exportJsonButton = new Button { Text = "📄 JSON Export", Size = new Size(120, 32) };
        DarkTheme.StyleButton(_exportJsonButton);
        _exportJsonButton.Click += OnExportJsonClick;
        exportPanel.Controls.Add(_exportJsonButton);
        
        _exportCsvButton = new Button { Text = "📊 CSV Export", Size = new Size(120, 32) };
        DarkTheme.StyleButton(_exportCsvButton);
        _exportCsvButton.Click += OnExportCsvClick;
        exportPanel.Controls.Add(_exportCsvButton);
        
        bottomPanel.Controls.Add(exportPanel);
        
        Controls.Add(bottomPanel);
        
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

    private void LoadPracticeTexts()
    {
        _practiceTexts = _typingService.GetDefaultPracticeTexts().ToList();
        
        _textSelector.Items.Clear();
        foreach (var text in _practiceTexts)
        {
            _textSelector.Items.Add($"[{text.Difficulty}] {text.Title}");
        }
        
        if (_textSelector.Items.Count > 0)
        {
            _textSelector.SelectedIndex = 0;
        }
    }

    public void LoadSettings(AppSettings settings)
    {
        _settings = settings;
        
        // Özel metinleri ekle
        foreach (var text in settings.CustomPracticeTexts)
        {
            _practiceTexts.Add(text);
            _textSelector.Items.Add($"[Özel] {text.Title}");
        }
        
        RefreshHistory();
    }

    private void RefreshHistory()
    {
        _historyListView.Items.Clear();
        
        if (_settings == null) return;
        
        foreach (var session in _settings.TypingSessions.OrderByDescending(s => s.Date).Take(20))
        {
            var item = new ListViewItem(session.Date.ToString("dd.MM.yyyy HH:mm"));
            item.SubItems.Add($"{session.Wpm:F1}");
            item.SubItems.Add($"%{session.Accuracy:F1}");
            item.SubItems.Add($"{TimeSpan.FromSeconds(session.DurationSeconds):mm\\:ss}");
            item.SubItems.Add(session.Difficulty);
            _historyListView.Items.Add(item);
        }
    }

    private void OnTextSelected(object? sender, EventArgs e)
    {
        if (_textSelector.SelectedIndex >= 0 && _textSelector.SelectedIndex < _practiceTexts.Count)
        {
            var text = _practiceTexts[_textSelector.SelectedIndex];
            _targetTextBox.Text = text.Content;
            OnResetClick(null, EventArgs.Empty);
        }
    }

    private void OnStartClick(object? sender, EventArgs e)
    {
        if (_currentSession != null && _stopwatch.IsRunning)
        {
            // Durdur ve kaydet
            StopSession();
            return;
        }
        
        // Başlat
        StartSession();
    }

    private void StartSession()
    {
        if (string.IsNullOrEmpty(_targetTextBox.Text)) return;
        
        _currentSession = _typingService.CreateSession(_targetTextBox.Text);
        
        if (_textSelector.SelectedIndex >= 0 && _textSelector.SelectedIndex < _practiceTexts.Count)
        {
            _currentSession.Difficulty = _practiceTexts[_textSelector.SelectedIndex].Difficulty;
        }
        
        _inputTextBox.Text = "";
        _inputTextBox.Enabled = true;
        _inputTextBox.Focus();
        
        _stopwatch.Restart();
        
        _updateTimer = new System.Windows.Forms.Timer { Interval = 100 };
        _updateTimer.Tick += OnTimerTick;
        _updateTimer.Start();
        
        _startButton.Text = "⏹️ Bitir";
        _resetButton.Enabled = true;
        _textSelector.Enabled = false;
        
        UpdateStatus("Yazmaya başlayın!");
    }

    private void StopSession()
    {
        if (_currentSession == null) return;
        
        _stopwatch.Stop();
        _updateTimer?.Stop();
        
        _currentSession = _typingService.CompleteSession(
            _currentSession,
            _inputTextBox.Text,
            _stopwatch.Elapsed.TotalSeconds
        );
        
        // İstatistikleri güncelle
        UpdateStats();
        
        // Geçmişe ekle
        if (_settings != null)
        {
            _settings.TypingSessions.Add(_currentSession);
            _ = SaveSettingsAsync();
            RefreshHistory();
        }
        
        _startButton.Text = "▶️ Başla";
        _textSelector.Enabled = true;
        _inputTextBox.Enabled = false;
        
        var result = $"Tamamlandı! WPM: {_currentSession.Wpm:F1}, Doğruluk: %{_currentSession.Accuracy:F1}";
        UpdateStatus(result);
        
        _currentSession = null;
    }

    private void OnResetClick(object? sender, EventArgs e)
    {
        _stopwatch.Reset();
        _updateTimer?.Stop();
        _currentSession = null;
        
        _inputTextBox.Text = "";
        _inputTextBox.Enabled = true;
        
        _wpmLabel.Text = "WPM: --";
        _accuracyLabel.Text = "Doğruluk: --%";
        _timeLabel.Text = "Süre: 00:00";
        
        _startButton.Text = "▶️ Başla";
        _resetButton.Enabled = false;
        _textSelector.Enabled = true;
        
        UpdateStatus("Hazır");
    }

    private void OnInputTextChanged(object? sender, EventArgs e)
    {
        if (_currentSession == null || !_stopwatch.IsRunning) return;
        
        UpdateStats();
        HighlightErrors();
        
        // Tamamlandı mı kontrol et
        if (_inputTextBox.Text.Length >= _targetTextBox.Text.Length)
        {
            StopSession();
        }
    }

    private void OnInputKeyDown(object? sender, KeyEventArgs e)
    {
        // Başlamamışsa ilk tuşta başlat
        if (_currentSession == null && !_stopwatch.IsRunning && e.KeyCode != Keys.Tab)
        {
            StartSession();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _timeLabel.Text = $"Süre: {_stopwatch.Elapsed:mm\\:ss}";
        
        if (_stopwatch.Elapsed.TotalSeconds > 0)
        {
            var wpm = _typingService.CalculateWpm(_inputTextBox.Text, _stopwatch.Elapsed.TotalSeconds);
            _wpmLabel.Text = $"WPM: {wpm:F0}";
        }
    }

    private void UpdateStats()
    {
        if (_stopwatch.Elapsed.TotalSeconds > 0)
        {
            var wpm = _typingService.CalculateWpm(_inputTextBox.Text, _stopwatch.Elapsed.TotalSeconds);
            var accuracy = _typingService.CalculateAccuracy(_targetTextBox.Text, _inputTextBox.Text);
            
            _wpmLabel.Text = $"WPM: {wpm:F0}";
            _accuracyLabel.Text = $"Doğruluk: %{accuracy:F0}";
            
            // Renklendirme
            _accuracyLabel.ForeColor = accuracy switch
            {
                >= 95 => DarkTheme.Success,
                >= 80 => DarkTheme.Warning,
                _ => DarkTheme.Error
            };
        }
    }

    private void HighlightErrors()
    {
        // Basit hata vurgulama - performans için sınırlı
        // Gerçek uygulamada daha sofistike olabilir
    }

    private async void OnExportCsvClick(object? sender, EventArgs e)
    {
        if (_settings == null || _settings.TypingSessions.Count == 0)
        {
            MessageBox.Show("Export edilecek veri yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        using var dialog = new SaveFileDialog
        {
            Filter = "CSV Dosyası|*.csv",
            Title = "Geçmişi CSV Olarak Kaydet",
            FileName = $"typing_history_{DateTime.Now:yyyyMMdd}.csv"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            await _typingService.ExportHistoryToCsvAsync(_settings.TypingSessions, dialog.FileName);
            UpdateStatus($"CSV kaydedildi: {dialog.FileName}");
        }
    }

    private async void OnExportJsonClick(object? sender, EventArgs e)
    {
        if (_settings == null || _settings.TypingSessions.Count == 0)
        {
            MessageBox.Show("Export edilecek veri yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        using var dialog = new SaveFileDialog
        {
            Filter = "JSON Dosyası|*.json",
            Title = "Geçmişi JSON Olarak Kaydet",
            FileName = $"typing_history_{DateTime.Now:yyyyMMdd}.json"
        };
        
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            await _typingService.ExportHistoryToJsonAsync(_settings.TypingSessions, dialog.FileName);
            UpdateStatus($"JSON kaydedildi: {dialog.FileName}");
        }
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
