// ===== Form1.cs =====
// Ana pencere arayüzü
// Tüm UI bileşenleri C# kodu ile oluşturulmuştur (Designer kullanılmamıştır)

using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace YoloWinForms
{
    /// <summary>
    /// Ana uygulama formu
    /// YOLOv8 ile nesne tespiti arayüzü
    /// </summary>
    public class Form1 : Form
    {
        // UI Bileşenleri
        private Panel _topPanel = null!;
        private Button _btnSelectImage = null!;
        private Button _btnDetect = null!;
        private Button _btnSave = null!;
        private Label _lblConfidence = null!;
        private NumericUpDown _numConfidence = null!;
        private PictureBox _pictureBox = null!;
        private Label _lblStatus = null!;

        // YOLOv8 Model
        private YoloV8Onnx? _yoloModel;
        
        // Görseller
        private string? _currentImagePath;
        private Bitmap? _originalBitmap;
        private Bitmap? _detectionBitmap;
        private List<Detection>? _lastDetections;

        // Model dosya yolu
        private readonly string _modelPath;

        /// <summary>
        /// Form yapıcısı - UI bileşenlerini oluşturur
        /// </summary>
        public Form1()
        {
            // Model yolunu belirle (uygulama dizininde)
            _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yolov8n.onnx");

            // Form ayarları
            InitializeForm();
            
            // UI bileşenlerini oluştur
            InitializeComponents();
            
            // Model yükleme durumunu kontrol et
            CheckModelStatus();
        }

        /// <summary>
        /// Form temel ayarlarını yapar
        /// </summary>
        private void InitializeForm()
        {
            this.Text = "YOLOv8 Nesne Tespiti - WinForms";
            this.Size = new System.Drawing.Size(1200, 800);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// UI bileşenlerini oluşturur ve yerleştirir
        /// </summary>
        private void InitializeComponents()
        {
            // ===== ÜST PANEL =====
            _topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                Padding = new Padding(10)
            };
            this.Controls.Add(_topPanel);

            // Görsel Seç Butonu
            _btnSelectImage = new Button
            {
                Text = "📁 Görsel Seç",
                Location = new System.Drawing.Point(10, 12),
                Size = new System.Drawing.Size(120, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            _btnSelectImage.FlatAppearance.BorderSize = 0;
            _btnSelectImage.Click += BtnSelectImage_Click;
            _topPanel.Controls.Add(_btnSelectImage);

            // Tespit Et Butonu
            _btnDetect = new Button
            {
                Text = "🔍 Tespit Et",
                Location = new System.Drawing.Point(140, 12),
                Size = new System.Drawing.Size(120, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(40, 167, 69),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnDetect.FlatAppearance.BorderSize = 0;
            _btnDetect.Click += BtnDetect_Click;
            _topPanel.Controls.Add(_btnDetect);

            // Kaydet Butonu
            _btnSave = new Button
            {
                Text = "💾 Kaydet",
                Location = new System.Drawing.Point(270, 12),
                Size = new System.Drawing.Size(120, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(255, 193, 7),
                ForeColor = System.Drawing.Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += BtnSave_Click;
            _topPanel.Controls.Add(_btnSave);

            // Confidence Label
            _lblConfidence = new Label
            {
                Text = "Güven Eşiği (%):",
                Location = new System.Drawing.Point(420, 18),
                Size = new System.Drawing.Size(110, 24),
                ForeColor = System.Drawing.Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleRight
            };
            _topPanel.Controls.Add(_lblConfidence);

            // Confidence NumericUpDown
            _numConfidence = new NumericUpDown
            {
                Location = new System.Drawing.Point(540, 14),
                Size = new System.Drawing.Size(70, 30),
                Minimum = 1,
                Maximum = 99,
                Value = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = System.Drawing.Color.FromArgb(60, 60, 60),
                ForeColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _topPanel.Controls.Add(_numConfidence);

            // Durum Label
            _lblStatus = new Label
            {
                Text = "Hazır",
                Location = new System.Drawing.Point(640, 18),
                Size = new System.Drawing.Size(500, 24),
                ForeColor = System.Drawing.Color.LightGray,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleLeft
            };
            _topPanel.Controls.Add(_lblStatus);

            // ===== ANA ALAN - PICTUREBOX =====
            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = System.Drawing.Color.FromArgb(25, 25, 25),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(_pictureBox);

            // PictureBox'ı panelin altına yerleştir
            _pictureBox.BringToFront();
            _topPanel.BringToFront();
        }

        /// <summary>
        /// Model dosyasının varlığını kontrol eder
        /// </summary>
        private void CheckModelStatus()
        {
            if (!File.Exists(_modelPath))
            {
                _lblStatus.Text = $"⚠️ Model bulunamadı: {_modelPath}";
                _lblStatus.ForeColor = System.Drawing.Color.Orange;
                _btnDetect.Enabled = false;
            }
            else
            {
                _lblStatus.Text = "✅ Model hazır - Görsel seçin";
                _lblStatus.ForeColor = System.Drawing.Color.LightGreen;
            }
        }

        /// <summary>
        /// Görsel seçme butonu tıklama olayı
        /// </summary>
        private void BtnSelectImage_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = "Görsel Seç",
                Filter = "Görsel Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tüm Dosyalar|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Önceki görselleri temizle
                    ClearImages();

                    // Yeni görseli yükle
                    _currentImagePath = openFileDialog.FileName;
                    _originalBitmap = new Bitmap(_currentImagePath);
                    _pictureBox.Image = _originalBitmap;

                    // Butonları güncelle
                    _btnDetect.Enabled = File.Exists(_modelPath);
                    _btnSave.Enabled = false;

                    _lblStatus.Text = $"📷 Görsel yüklendi: {Path.GetFileName(_currentImagePath)} ({_originalBitmap.Width}x{_originalBitmap.Height})";
                    _lblStatus.ForeColor = System.Drawing.Color.LightGreen;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Görsel yüklenirken hata oluştu:\n{ex.Message}", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "❌ Görsel yüklenemedi";
                    _lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        /// <summary>
        /// Tespit butonu tıklama olayı
        /// </summary>
        private async void BtnDetect_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentImagePath) || !File.Exists(_currentImagePath))
            {
                MessageBox.Show("Lütfen önce bir görsel seçin.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(_modelPath))
            {
                MessageBox.Show($"Model dosyası bulunamadı:\n{_modelPath}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // UI'ı devre dışı bırak
            SetUIEnabled(false);
            _lblStatus.Text = "🔄 Tespit yapılıyor...";
            _lblStatus.ForeColor = System.Drawing.Color.Yellow;

            try
            {
                // Confidence değerini al (0-1 arası)
                float confidence = (float)_numConfidence.Value / 100f;
                string imagePath = _currentImagePath!;

                // Tespiti arka planda yap
                var (detections, resultImage) = await Task.Run(() =>
                {
                    // Modeli yükle (ilk seferde)
                    if (_yoloModel == null)
                    {
                        _yoloModel = new YoloV8Onnx(_modelPath);
                    }

                    // Tespit yap
                    var dets = _yoloModel.Detect(imagePath, confidence);

                    // Sonuç görselini oluştur
                    var imgResult = YoloV8Onnx.DrawDetections(imagePath, dets);

                    return (dets, imgResult);
                });

                // UI thread'de güncelleme yap
                if (this.InvokeRequired)
                {
                    this.Invoke(() => UpdateUIAfterDetection(detections, resultImage));
                }
                else
                {
                    UpdateUIAfterDetection(detections, resultImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tespit sırasında hata oluştu:\n{ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus.Text = $"❌ Hata: {ex.Message}";
                _lblStatus.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                SetUIEnabled(true);
            }
        }

        /// <summary>
        /// Tespit sonrası UI güncellemelerini yapar
        /// </summary>
        private void UpdateUIAfterDetection(List<Detection> detections, SixLabors.ImageSharp.Image<Rgb24> resultImage)
        {
            // Sonuçları sakla
            _lastDetections = detections;

            // ImageSharp görselini Bitmap'e dönüştür
            _detectionBitmap?.Dispose();
            _detectionBitmap = ConvertToBitmap(resultImage);
            resultImage.Dispose();

            // Önceki resmi temizle ve yenisini ata
            var oldImage = _pictureBox.Image;
            _pictureBox.Image = _detectionBitmap;
            
            // PictureBox'ı yenile
            _pictureBox.Refresh();
            
            // Kaydet butonunu aktif et
            _btnSave.Enabled = true;

            // Durum güncelle
            if (detections.Count > 0)
            {
                var classCounts = detections.GroupBy(d => d.ClassId)
                    .Select(g => $"{YoloV8Onnx.GetClassName(g.Key)}: {g.Count()}")
                    .ToList();
                
                _lblStatus.Text = $"✅ {detections.Count} nesne tespit edildi - {string.Join(", ", classCounts)}";
                _lblStatus.ForeColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                _lblStatus.Text = "ℹ️ Hiç nesne tespit edilemedi. Güven eşiğini düşürmeyi deneyin.";
                _lblStatus.ForeColor = System.Drawing.Color.Orange;
            }
        }

        /// <summary>
        /// Kaydet butonu tıklama olayı
        /// </summary>
        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_detectionBitmap == null)
            {
                MessageBox.Show("Kaydedilecek görsel yok. Önce tespit yapın.", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Sonucu Kaydet",
                Filter = "PNG Dosyası|*.png|JPEG Dosyası|*.jpg|BMP Dosyası|*.bmp",
                FilterIndex = 1,
                FileName = $"detection_result_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Dosya uzantısına göre format belirle
                    ImageFormat format = saveFileDialog.FilterIndex switch
                    {
                        1 => ImageFormat.Png,
                        2 => ImageFormat.Jpeg,
                        3 => ImageFormat.Bmp,
                        _ => ImageFormat.Png
                    };

                    _detectionBitmap.Save(saveFileDialog.FileName, format);

                    _lblStatus.Text = $"💾 Kaydedildi: {Path.GetFileName(saveFileDialog.FileName)}";
                    _lblStatus.ForeColor = System.Drawing.Color.LightGreen;

                    MessageBox.Show($"Görsel başarıyla kaydedildi:\n{saveFileDialog.FileName}", "Başarılı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kaydetme sırasında hata oluştu:\n{ex.Message}", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _lblStatus.Text = "❌ Kaydetme hatası";
                    _lblStatus.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        /// <summary>
        /// ImageSharp görselini System.Drawing.Bitmap'e dönüştürür
        /// </summary>
        private Bitmap ConvertToBitmap(SixLabors.ImageSharp.Image<Rgb24> image)
        {
            var memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            // MemoryStream'i dispose etmiyoruz çünkü Bitmap onu kullanmaya devam ediyor
            // Bitmap dispose edildiğinde stream de serbest kalacak
            return new Bitmap(memoryStream);
        }

        /// <summary>
        /// UI bileşenlerini etkinleştirir/devre dışı bırakır
        /// </summary>
        private void SetUIEnabled(bool enabled)
        {
            _btnSelectImage.Enabled = enabled;
            _btnDetect.Enabled = enabled && _originalBitmap != null && File.Exists(_modelPath);
            _btnSave.Enabled = enabled && _detectionBitmap != null;
            _numConfidence.Enabled = enabled;
        }

        /// <summary>
        /// Yüklü görselleri temizler
        /// </summary>
        private void ClearImages()
        {
            _pictureBox.Image = null;
            
            _originalBitmap?.Dispose();
            _originalBitmap = null;
            
            _detectionBitmap?.Dispose();
            _detectionBitmap = null;
            
            _lastDetections = null;
        }

        /// <summary>
        /// Form kapatılırken kaynakları serbest bırakır
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Görselleri temizle
            ClearImages();

            // Modeli kapat
            _yoloModel?.Dispose();
            _yoloModel = null;
        }
    }
}
