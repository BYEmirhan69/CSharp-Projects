using System;
using System.Text;
using System.Windows.Forms;

namespace SpeechToText
{
    /// <summary>
    /// Basit WinForms arayüzü: Başlat/Durdur butonları ve tanınan metin için çok satırlı kutu.
    /// </summary>
    public sealed class MainForm : Form
    {
        private readonly SpeechRecognizerService _recognizerService;

        private readonly Button _buttonStart;
        private readonly Button _buttonStop;
        private readonly TextBox _textOutput;
        private readonly Label _statusLabel;
        private readonly ComboBox _comboRecognizer;
        private readonly ComboBox _comboMicrophone;

        public MainForm()
        {
            Text = "Speech to Text (tr-TR)";
            Width = 800;
            Height = 500;
            StartPosition = FormStartPosition.CenterScreen;

            _buttonStart = new Button
            {
                Text = "Başlat",
                Left = 20,
                Top = 20,
                Width = 120,
                Height = 35
            };

            _buttonStop = new Button
            {
                Text = "Durdur",
                Left = 160,
                Top = 20,
                Width = 120,
                Height = 35,
                Enabled = false
            };

            _statusLabel = new Label
            {
                Text = "Durum: Beklemede",
                Left = 300,
                Top = 26,
                AutoSize = true
            };

            _textOutput = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Left = 20,
                Top = 70,
                Width = 740,
                Height = 370,
                ReadOnly = true
            };

            _comboRecognizer = new ComboBox
            {
                Left = 500,
                Top = 24,
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _comboMicrophone = new ComboBox
            {
                Left = 500,
                Top = 50,
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Controls.Add(_buttonStart);
            Controls.Add(_buttonStop);
            Controls.Add(_statusLabel);
            Controls.Add(_textOutput);
            Controls.Add(_comboRecognizer);
            Controls.Add(_comboMicrophone);

            _recognizerService = new SpeechRecognizerService();

            // Olaylara abone ol.
            _recognizerService.TextRecognized += RecognizerService_TextRecognized;
            _recognizerService.RecognitionError += RecognizerService_RecognitionError;
            _recognizerService.ListeningStateChanged += RecognizerService_ListeningStateChanged;

            // Yüklü tanıyıcıları listele ve uygun olanı varsayılan seç.
            LoadRecognizers();
            LoadMicrophones();

            _buttonStart.Click += (s, e) =>
            {
                try
                {
                    var selected = _comboRecognizer.SelectedItem as RecognizerListItem;
                    if (selected != null)
                    {
                        _recognizerService.SetPreferredRecognizer(selected.Id);
                    }
                    var mic = _comboMicrophone.SelectedItem as MicrophoneListItem;
                    _recognizerService.SetPreferredMicrophone(mic?.DeviceIndex);
                    _recognizerService.Start();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };

            _buttonStop.Click += (s, e) =>
            {
                try
                {
                    _recognizerService.Stop();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            };

            FormClosed += (s, e) => _recognizerService.Dispose();
        }

        private void LoadRecognizers()
        {
            _comboRecognizer.Items.Clear();
            try
            {
                var installed = System.Speech.Recognition.SpeechRecognitionEngine.InstalledRecognizers();
                foreach (var r in installed)
                {
                    _comboRecognizer.Items.Add(new RecognizerListItem
                    {
                        Id = r.Id,
                        Display = $"{r.Culture.Name} - {r.Name}"
                    });
                }

                // Önce tr-TR, yoksa en-GB, yoksa ilk öğe
                int index = -1;
                for (int i = 0; i < _comboRecognizer.Items.Count; i++)
                {
                    var item = (RecognizerListItem)_comboRecognizer.Items[i];
                    if (item.Display.StartsWith("tr-TR", StringComparison.OrdinalIgnoreCase)) { index = i; break; }
                }
                if (index == -1)
                {
                    for (int i = 0; i < _comboRecognizer.Items.Count; i++)
                    {
                        var item = (RecognizerListItem)_comboRecognizer.Items[i];
                        if (item.Display.StartsWith("en-GB", StringComparison.OrdinalIgnoreCase)) { index = i; break; }
                    }
                }
                if (_comboRecognizer.Items.Count > 0)
                {
                    _comboRecognizer.SelectedIndex = index >= 0 ? index : 0;
                }
            }
            catch (Exception ex)
            {
                // Listeleme hatası UI'yı engellemesin
                _comboRecognizer.Items.Add(new RecognizerListItem { Id = string.Empty, Display = ex.Message });
                _comboRecognizer.SelectedIndex = 0;
            }
        }

        private void LoadMicrophones()
        {
            _comboMicrophone.Items.Clear();
            try
            {
                for (int i = 0; i < NAudio.Wave.WaveInEvent.DeviceCount; i++)
                {
                    var caps = NAudio.Wave.WaveInEvent.GetCapabilities(i);
                    _comboMicrophone.Items.Add(new MicrophoneListItem
                    {
                        DeviceIndex = i,
                        Display = caps.ProductName
                    });
                }
                if (_comboMicrophone.Items.Count > 0)
                {
                    _comboMicrophone.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _comboMicrophone.Items.Add(new MicrophoneListItem { DeviceIndex = null, Display = ex.Message });
                _comboMicrophone.SelectedIndex = 0;
            }
        }

        private void RecognizerService_ListeningStateChanged(object sender, bool isListening)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, bool>(RecognizerService_ListeningStateChanged), sender, isListening);
                return;
            }

            _buttonStart.Enabled = !isListening;
            _buttonStop.Enabled = isListening;
            _statusLabel.Text = isListening ? "Durum: Dinleniyor..." : "Durum: Beklemede";
        }

        private void RecognizerService_TextRecognized(object sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, string>(RecognizerService_TextRecognized), sender, text);
                return;
            }

            // Yeni tanınan metni sonuna ekle.
            var builder = new StringBuilder(_textOutput.Text);
            if (builder.Length > 0) builder.Append(Environment.NewLine);
            builder.Append(text);
            _textOutput.Text = builder.ToString();
            _textOutput.SelectionStart = _textOutput.Text.Length;
            _textOutput.ScrollToCaret();
        }

        private void RecognizerService_RecognitionError(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, Exception>(RecognizerService_RecognitionError), sender, ex);
                return;
            }
            ShowError(ex);
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(this,
                ex.Message,
                "Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    internal sealed class RecognizerListItem
    {
        public string Id { get; set; }
        public string Display { get; set; }
        public override string ToString() => Display;
    }

    internal sealed class MicrophoneListItem
    {
        public int? DeviceIndex { get; set; }
        public string Display { get; set; }
        public override string ToString() => Display;
    }
}


