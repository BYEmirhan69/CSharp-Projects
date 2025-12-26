using LLMChatbot.WinForms.Common;
using LLMChatbot.WinForms.Core;
using LLMChatbot.WinForms.Services;

namespace LLMChatbot.WinForms.UI;

/// <summary>
/// Ana uygulama formu.
/// Sohbet arayüzünü ve kullanıcı etkileşimlerini yönetir.
/// </summary>
public partial class MainForm : Form
{
    private readonly OpenAiService _openAiService;
    private readonly Conversation _conversation;

    // UI Bileşenleri
    private RichTextBox _chatHistoryBox = null!;
    private TextBox _messageInputBox = null!;
    private Button _sendButton = null!;
    private Label _statusLabel = null!;
    private Panel _inputPanel = null!;

    /// <summary>
    /// MainForm constructor - UI ve servisleri başlatır
    /// </summary>
    public MainForm()
    {
        _openAiService = new OpenAiService();
        _conversation = new Conversation();

        InitializeComponent();
        SetupEventHandlers();
        CheckApiKeyOnStart();
    }

    /// <summary>
    /// Form bileşenlerini oluşturur ve yapılandırır
    /// </summary>
    private void InitializeComponent()
    {
        // Form ayarları
        Text = "LLM Chatbot";
        Size = new Size(800, 600);
        MinimumSize = new Size(500, 400);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(30, 30, 30);

        // Durum etiketi (en üstte)
        _statusLabel = new Label
        {
            Text = "Hazır",
            Dock = DockStyle.Top,
            Height = 30,
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.FromArgb(45, 45, 45),
            ForeColor = Color.FromArgb(0, 200, 150),
            Padding = new Padding(10, 0, 0, 0),
            Font = new Font("Segoe UI", 10, FontStyle.Regular)
        };

        // Sohbet geçmişi alanı
        _chatHistoryBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BackColor = Color.FromArgb(25, 25, 25),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            BorderStyle = BorderStyle.None,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        // Giriş paneli (alt kısım)
        _inputPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 80,
            BackColor = Color.FromArgb(40, 40, 40),
            Padding = new Padding(10)
        };

        // Mesaj giriş alanı
        _messageInputBox = new TextBox
        {
            Multiline = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            Location = new Point(10, 10),
            Size = new Size(_inputPanel.Width - 110, 60),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11, FontStyle.Regular),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Gönder butonu
        _sendButton = new Button
        {
            Text = "Gönder",
            Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
            Location = new Point(_inputPanel.Width - 90, 10),
            Size = new Size(75, 60),
            BackColor = Color.FromArgb(0, 150, 136),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _sendButton.FlatAppearance.BorderSize = 0;

        // Panel'e bileşenleri ekle
        _inputPanel.Controls.Add(_messageInputBox);
        _inputPanel.Controls.Add(_sendButton);

        // Form'a bileşenleri ekle (sıralama önemli)
        Controls.Add(_chatHistoryBox);
        Controls.Add(_inputPanel);
        Controls.Add(_statusLabel);
    }

    /// <summary>
    /// Olay işleyicilerini ayarlar
    /// </summary>
    private void SetupEventHandlers()
    {
        _sendButton.Click += async (s, e) => await SendMessageAsync();
        
        // Enter tuşu ile gönderme (Shift+Enter yeni satır)
        _messageInputBox.KeyDown += async (s, e) =>
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                await SendMessageAsync();
            }
        };

        // Form boyutu değiştiğinde input panel'i güncelle
        _inputPanel.Resize += (s, e) =>
        {
            _messageInputBox.Width = _inputPanel.Width - 110;
            _sendButton.Location = new Point(_inputPanel.Width - 90, 10);
        };
    }

    /// <summary>
    /// Uygulama başlangıcında API anahtarını kontrol eder
    /// </summary>
    private void CheckApiKeyOnStart()
    {
        if (!ConfigHelper.IsApiKeyConfigured())
        {
            UpdateStatus("API Anahtarı Eksik!", Color.OrangeRed);
            AddSystemMessage("⚠️ OpenAI API anahtarı bulunamadı.\n\n" +
                "Lütfen OPENAI_API_KEY ortam değişkenini ayarlayın.");
        }
        else
        {
            AddSystemMessage($"🤖 LLM Chatbot'a hoş geldiniz!\n\n" +
                $"Model: {ConfigHelper.Model}\n" +
                $"Mesajınızı yazın ve Gönder butonuna tıklayın veya Enter tuşuna basın.");
        }
    }

    /// <summary>
    /// Mesaj gönderme işlemini gerçekleştirir (async)
    /// </summary>
    private async Task SendMessageAsync()
    {
        var userMessage = _messageInputBox.Text.Trim();

        // Boş mesaj kontrolü
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            return;
        }

        // UI'ı devre dışı bırak
        SetUIEnabled(false);
        UpdateStatus("Yazıyor...", Color.FromArgb(255, 193, 7));

        // Kullanıcı mesajını ekle
        AddUserMessage(userMessage);
        _messageInputBox.Clear();

        // Konuşmaya ekle
        _conversation.AddUserMessage(userMessage);

        try
        {
            // API'den yanıt al
            var response = await _openAiService.GetResponseAsync(_conversation);

            // Yanıtı konuşmaya ve UI'a ekle
            _conversation.AddAssistantMessage(response);
            AddBotMessage(response);

            UpdateStatus("Hazır", Color.FromArgb(0, 200, 150));
        }
        catch (Exception ex)
        {
            // Hata durumunda kullanıcıyı bilgilendir
            AddErrorMessage($"Hata: {ex.Message}");
            UpdateStatus("Hata!", Color.OrangeRed);
        }
        finally
        {
            // UI'ı tekrar etkinleştir
            SetUIEnabled(true);
            _messageInputBox.Focus();
        }
    }

    /// <summary>
    /// UI bileşenlerini etkinleştirir/devre dışı bırakır
    /// </summary>
    private void SetUIEnabled(bool enabled)
    {
        _sendButton.Enabled = enabled;
        _messageInputBox.Enabled = enabled;
        _sendButton.BackColor = enabled 
            ? Color.FromArgb(0, 150, 136) 
            : Color.FromArgb(100, 100, 100);
    }

    /// <summary>
    /// Durum etiketini günceller
    /// </summary>
    private void UpdateStatus(string status, Color color)
    {
        _statusLabel.Text = status;
        _statusLabel.ForeColor = color;
    }

    /// <summary>
    /// Kullanıcı mesajını sohbet geçmişine ekler (sağ hizalı)
    /// </summary>
    private void AddUserMessage(string message)
    {
        AppendFormattedMessage("Sen", message, Color.FromArgb(100, 180, 255), HorizontalAlignment.Right);
    }

    /// <summary>
    /// Bot mesajını sohbet geçmişine ekler (sol hizalı)
    /// </summary>
    private void AddBotMessage(string message)
    {
        AppendFormattedMessage("Bot", message, Color.FromArgb(0, 200, 150), HorizontalAlignment.Left);
    }

    /// <summary>
    /// Sistem mesajını sohbet geçmişine ekler
    /// </summary>
    private void AddSystemMessage(string message)
    {
        AppendFormattedMessage("Sistem", message, Color.FromArgb(180, 180, 180), HorizontalAlignment.Center);
    }

    /// <summary>
    /// Hata mesajını sohbet geçmişine ekler
    /// </summary>
    private void AddErrorMessage(string message)
    {
        AppendFormattedMessage("Hata", message, Color.OrangeRed, HorizontalAlignment.Center);
    }

    /// <summary>
    /// Formatlanmış mesajı RichTextBox'a ekler
    /// </summary>
    private void AppendFormattedMessage(string sender, string message, Color color, HorizontalAlignment alignment)
    {
        // Zaman damgası
        var timestamp = DateTime.Now.ToString("HH:mm");

        // Başlık satırı
        _chatHistoryBox.SelectionAlignment = alignment;
        _chatHistoryBox.SelectionColor = color;
        _chatHistoryBox.SelectionFont = new Font("Segoe UI", 9, FontStyle.Bold);
        _chatHistoryBox.AppendText($"[{timestamp}] {sender}\n");

        // Mesaj içeriği
        _chatHistoryBox.SelectionAlignment = alignment;
        _chatHistoryBox.SelectionColor = Color.White;
        _chatHistoryBox.SelectionFont = new Font("Segoe UI", 11, FontStyle.Regular);
        _chatHistoryBox.AppendText($"{message}\n\n");

        // Otomatik scroll
        _chatHistoryBox.SelectionStart = _chatHistoryBox.Text.Length;
        _chatHistoryBox.ScrollToCaret();
    }

    /// <summary>
    /// Form kapanırken kaynakları temizler
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _openAiService.Dispose();
        base.OnFormClosing(e);
    }
}
