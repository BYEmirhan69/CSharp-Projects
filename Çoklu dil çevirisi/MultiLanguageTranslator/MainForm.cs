using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using MultiLanguageTranslator.Services;

namespace MultiLanguageTranslator
{
    /// <summary>
    /// Ana form - Çoklu dil çevirisi uygulaması
    /// Main form - Multi-language translation application
    /// </summary>
    public class MainForm : Form
    {
        // UI Kontrolleri / UI Controls
        private SplitContainer _mainSplitContainer = null!;
        private TableLayoutPanel _leftPanel = null!;
        private TableLayoutPanel _rightPanel = null!;
        private FlowLayoutPanel _topPanel = null!;
        private FlowLayoutPanel _resultPanel = null!;

        private Label _lblSourceText = null!;
        private RichTextBox _txtSourceText = null!;
        private Label _lblSourceLanguage = null!;
        private ComboBox _cmbSourceLanguage = null!;
        private Label _lblTargetLanguages = null!;
        private CheckedListBox _chkTargetLanguages = null!;
        private Button _btnTranslate = null!;
        private Label _lblUILanguage = null!;
        private ComboBox _cmbUILanguage = null!;
        private GroupBox _grpInput = null!;
        private GroupBox _grpOutput = null!;
        private Panel _scrollPanel = null!;

        // Servisler / Services
        private readonly MockTranslationEngine _translationEngine;
        private ResourceManager _resourceManager = null!;

        // Mevcut UI dili / Current UI language
        private CultureInfo _currentCulture;

        public MainForm()
        {
            _translationEngine = new MockTranslationEngine();
            _currentCulture = new CultureInfo("tr-TR"); // Varsayılan Türkçe / Default Turkish

            InitializeResources();
            InitializeComponents();
            ApplyLocalization();
        }

        /// <summary>
        /// Resource Manager'ı başlatır.
        /// Initializes the Resource Manager.
        /// </summary>
        private void InitializeResources()
        {
            _resourceManager = new ResourceManager("MultiLanguageTranslator.Resources.Strings", typeof(MainForm).Assembly);
        }

        /// <summary>
        /// Localized string döndürür.
        /// Returns localized string.
        /// </summary>
        private string GetString(string key)
        {
            try
            {
                string? value = _resourceManager.GetString(key, _currentCulture);
                return value ?? key;
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Tüm UI bileşenlerini başlatır.
        /// Initializes all UI components.
        /// </summary>
        private void InitializeComponents()
        {
            // Form ayarları / Form settings
            this.Text = "Çoklu Dil Çevirisi";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.BackColor = Color.WhiteSmoke;

            // Üst panel oluştur / Create top panel
            CreateTopPanel();

            // Ana SplitContainer oluştur / Create main SplitContainer
            CreateMainSplitContainer();

            // Sol panel oluştur / Create left panel
            CreateLeftPanel();

            // Sağ panel oluştur / Create right panel
            CreateRightPanel();

            // Kontrolleri forma ekle / Add controls to form
            this.Controls.Add(_mainSplitContainer);
            this.Controls.Add(_topPanel);

            // Form resize olayı / Form resize event
            this.Resize += MainForm_Resize;
        }

        /// <summary>
        /// Üst panel oluşturur (UI dil seçimi).
        /// Creates top panel (UI language selection).
        /// </summary>
        private void CreateTopPanel()
        {
            _topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(52, 73, 94),
                FlowDirection = FlowDirection.RightToLeft
            };

            _cmbUILanguage = new ComboBox
            {
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            _cmbUILanguage.Items.AddRange(new object[] { "Türkçe", "English" });
            _cmbUILanguage.SelectedIndex = 0;
            _cmbUILanguage.SelectedIndexChanged += CmbUILanguage_SelectedIndexChanged;

            _lblUILanguage = new Label
            {
                Text = "Arayüz Dili:",
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(5, 6, 5, 0),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            _topPanel.Controls.Add(_cmbUILanguage);
            _topPanel.Controls.Add(_lblUILanguage);
        }

        /// <summary>
        /// Ana SplitContainer oluşturur.
        /// Creates main SplitContainer.
        /// </summary>
        private void CreateMainSplitContainer()
        {
            _mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 6,
                BackColor = Color.FromArgb(189, 195, 199)
            };
        }

        /// <summary>
        /// Sol panel oluşturur (giriş alanı).
        /// Creates left panel (input area).
        /// </summary>
        private void CreateLeftPanel()
        {
            _grpInput = new GroupBox
            {
                Text = "Giriş",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            _leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(5),
                BackColor = Color.White
            };

            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Kaynak metin label
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));  // Kaynak metin
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Kaynak dil label
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F)); // Kaynak dil combo
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Hedef dil label
            _leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));  // Hedef dil checklist + buton

            // Kaynak metin label / Source text label
            _lblSourceText = new Label
            {
                Text = "Çevrilecek Metin:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            _leftPanel.Controls.Add(_lblSourceText, 0, 0);

            // Kaynak metin kutusu / Source text box
            _txtSourceText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250),
                Margin = new Padding(0, 5, 0, 10)
            };
            _leftPanel.Controls.Add(_txtSourceText, 0, 1);

            // Kaynak dil label / Source language label
            _lblSourceLanguage = new Label
            {
                Text = "Kaynak Dil:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            _leftPanel.Controls.Add(_lblSourceLanguage, 0, 2);

            // Kaynak dil combo / Source language combo
            _cmbSourceLanguage = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(0, 5, 0, 10)
            };
            foreach (string lang in _translationEngine.GetSourceLanguages())
            {
                _cmbSourceLanguage.Items.Add(lang);
            }
            _cmbSourceLanguage.SelectedIndex = 0;
            _leftPanel.Controls.Add(_cmbSourceLanguage, 0, 3);

            // Hedef dil label / Target language label
            _lblTargetLanguages = new Label
            {
                Text = "Hedef Diller:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            _leftPanel.Controls.Add(_lblTargetLanguages, 0, 4);

            // Hedef dil ve buton paneli / Target language and button panel
            TableLayoutPanel targetPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            targetPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            // Hedef dil checklist / Target language checklist
            _chkTargetLanguages = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                CheckOnClick = true,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            foreach (string lang in _translationEngine.GetTargetLanguages())
            {
                _chkTargetLanguages.Items.Add(lang);
            }
            targetPanel.Controls.Add(_chkTargetLanguages, 0, 0);

            // Çevir butonu / Translate button
            _btnTranslate = new Button
            {
                Text = "Çevir",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 10, 0, 0)
            };
            _btnTranslate.FlatAppearance.BorderSize = 0;
            _btnTranslate.Click += BtnTranslate_Click;
            targetPanel.Controls.Add(_btnTranslate, 0, 1);

            _leftPanel.Controls.Add(targetPanel, 0, 5);

            _grpInput.Controls.Add(_leftPanel);
            _mainSplitContainer.Panel1.Controls.Add(_grpInput);
        }

        /// <summary>
        /// Sağ panel oluşturur (çıkış alanı).
        /// Creates right panel (output area).
        /// </summary>
        private void CreateRightPanel()
        {
            _grpOutput = new GroupBox
            {
                Text = "Çeviri Sonuçları",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            _scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White
            };

            _resultPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(5),
                BackColor = Color.White
            };

            _scrollPanel.Controls.Add(_resultPanel);
            _grpOutput.Controls.Add(_scrollPanel);
            _mainSplitContainer.Panel2.Controls.Add(_grpOutput);
        }

        /// <summary>
        /// UI dil değiştirildiğinde çağrılır.
        /// Called when UI language is changed.
        /// </summary>
        private void CmbUILanguage_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbUILanguage.SelectedItem?.ToString() == "English")
            {
                _currentCulture = new CultureInfo("en-US");
            }
            else
            {
                _currentCulture = new CultureInfo("tr-TR");
            }

            ApplyLocalization();
        }

        /// <summary>
        /// Tüm UI metinlerini günceller.
        /// Updates all UI texts.
        /// </summary>
        private void ApplyLocalization()
        {
            this.Text = GetString("FormTitle");
            _lblUILanguage.Text = GetString("UILanguage");
            _grpInput.Text = GetString("InputGroup");
            _grpOutput.Text = GetString("OutputGroup");
            _lblSourceText.Text = GetString("SourceText");
            _lblSourceLanguage.Text = GetString("SourceLanguage");
            _lblTargetLanguages.Text = GetString("TargetLanguages");
            _btnTranslate.Text = GetString("TranslateButton");
        }

        /// <summary>
        /// Çevir butonuna tıklandığında çağrılır.
        /// Called when translate button is clicked.
        /// </summary>
        private void BtnTranslate_Click(object? sender, EventArgs e)
        {
            // Metin kontrolü / Text validation
            string sourceText = _txtSourceText.Text.Trim();
            if (string.IsNullOrEmpty(sourceText))
            {
                ShowError(GetString("ErrorEmptyText"));
                return;
            }

            // Hedef dil kontrolü / Target language validation
            List<string> selectedLanguages = new List<string>();
            foreach (var item in _chkTargetLanguages.CheckedItems)
            {
                selectedLanguages.Add(item.ToString()!);
            }

            if (selectedLanguages.Count == 0)
            {
                ShowError(GetString("ErrorNoTargetLanguage"));
                return;
            }

            // Kaynak dil / Source language
            string sourceLanguage = _cmbSourceLanguage.SelectedItem?.ToString() ?? "Türkçe";

            // Kaynak dil hedef dillerde mi kontrol / Check if source is in target
            if (selectedLanguages.Contains(sourceLanguage))
            {
                selectedLanguages.Remove(sourceLanguage);
                if (selectedLanguages.Count == 0)
                {
                    ShowError(GetString("ErrorSameLanguage"));
                    return;
                }
            }

            try
            {
                // Çeviri yap / Perform translation
                Dictionary<string, string> translations = _translationEngine.TranslateToMultiple(
                    sourceText,
                    sourceLanguage,
                    selectedLanguages
                );

                // Sonuçları göster / Display results
                DisplayTranslationResults(translations);
            }
            catch (Exception ex)
            {
                ShowError($"{GetString("ErrorTranslation")}: {ex.Message}");
            }
        }

        /// <summary>
        /// Çeviri sonuçlarını gösterir.
        /// Displays translation results.
        /// </summary>
        private void DisplayTranslationResults(Dictionary<string, string> translations)
        {
            _resultPanel.Controls.Clear();

            foreach (var kvp in translations)
            {
                Panel resultCard = CreateResultCard(kvp.Key, kvp.Value);
                _resultPanel.Controls.Add(resultCard);
            }

            // Panel genişliğini ayarla / Adjust panel width
            AdjustResultPanelWidth();
        }

        /// <summary>
        /// Tek bir çeviri sonucu kartı oluşturur.
        /// Creates a single translation result card.
        /// </summary>
        private Panel CreateResultCard(string language, string translation)
        {
            Panel card = new Panel
            {
                Width = _scrollPanel.ClientSize.Width - 30,
                Height = 120,
                BackColor = Color.FromArgb(236, 240, 241),
                Margin = new Padding(5),
                Padding = new Padding(10)
            };

            // Dil başlığı / Language header
            Label lblLanguage = new Label
            {
                Text = GetLanguageDisplayName(language),
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                BackColor = Color.FromArgb(214, 234, 248),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };

            // Çeviri metni / Translation text
            RichTextBox txtTranslation = new RichTextBox
            {
                Text = translation,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 5, 0, 0)
            };

            card.Controls.Add(txtTranslation);
            card.Controls.Add(lblLanguage);

            return card;
        }

        /// <summary>
        /// Dil kodunu görüntüleme adına çevirir.
        /// Converts language code to display name.
        /// </summary>
        private string GetLanguageDisplayName(string language)
        {
            return language switch
            {
                "İngilizce" => "🇬🇧 İngilizce / English",
                "Almanca" => "🇩🇪 Almanca / Deutsch",
                "Fransızca" => "🇫🇷 Fransızca / Français",
                "İspanyolca" => "🇪🇸 İspanyolca / Español",
                "Türkçe" => "🇹🇷 Türkçe / Turkish",
                _ => language
            };
        }

        /// <summary>
        /// Hata mesajı gösterir.
        /// Displays error message.
        /// </summary>
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                GetString("ErrorTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        /// <summary>
        /// Sonuç paneli genişliğini ayarlar.
        /// Adjusts result panel width.
        /// </summary>
        private void AdjustResultPanelWidth()
        {
            foreach (Control control in _resultPanel.Controls)
            {
                if (control is Panel card)
                {
                    card.Width = _scrollPanel.ClientSize.Width - 30;
                }
            }
        }

        /// <summary>
        /// Form yeniden boyutlandırıldığında çağrılır.
        /// Called when form is resized.
        /// </summary>
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            AdjustResultPanelWidth();
        }
    }
}
