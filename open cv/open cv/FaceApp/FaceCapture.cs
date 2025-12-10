using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace FaceApp
{
    // Yüz verisi toplama: Kullanıcı isminden klasör açar ve tespit edilen yüzleri gri olarak kaydeder
    public class FaceCapture
    {
        private readonly PictureBox _pictureBox;
        private readonly Label _statusLabel;
        private VideoCapture? _capture;
        private CascadeClassifier? _faceDetector;
        private bool _running;
        private string _currentPersonDir = string.Empty;
        private int _savedCount;

        // Toplanacak örnek sayısı (isteğe göre artırılabilir)
        private const int TargetSamples = 50;

        public FaceCapture(PictureBox pictureBox, Label statusLabel)
        {
            _pictureBox = pictureBox;
            _statusLabel = statusLabel;
            Directory.CreateDirectory(Paths.ModelsDirectory);
        }

        public void Start(string personName)
        {
            try
            {
                var cascadePath = Paths.GetCascadePathForLoading();
                if (!File.Exists(cascadePath))
                {
                    MessageBox.Show("Haar cascade dosyası bulunamadı: " + cascadePath + "\nLütfen dosyayı Assets/haarcascades klasörüne kopyalayın.");
                    return;
                }

                // Mutlak yolu kullan ve cascade'i yükle
                cascadePath = Path.GetFullPath(cascadePath);
                
                // Dosya varlığını ve boyutunu kontrol et
                if (!File.Exists(cascadePath))
                {
                    MessageBox.Show($"Haar cascade dosyası bulunamadı:\n{cascadePath}");
                    Stop();
                    return;
                }
                
                var fileInfo = new FileInfo(cascadePath);
                if (fileInfo.Length < 1000) // Dosya çok küçük, muhtemelen bozuk
                {
                    MessageBox.Show($"Haar cascade dosyası çok küçük (bozuk olabilir):\n{cascadePath}\nBoyut: {fileInfo.Length} byte\n\nLütfen dosyayı yeniden indirin.");
                    Stop();
                    return;
                }
                
                try
                {
                    _faceDetector = new CascadeClassifier(cascadePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Haar cascade dosyası yüklenemedi:\n{ex.Message}\n\nYol: {cascadePath}\n\nDosya bozuk olabilir. Lütfen OpenCV deposundan yeniden indirin:\nhttps://raw.githubusercontent.com/opencv/opencv/4.x/data/haarcascades/haarcascade_frontalface_default.xml");
                    Stop();
                    return;
                }
                _capture = new VideoCapture(0, VideoCapture.API.DShow);
                _capture.Set(CapProp.FrameWidth, 1280);
                _capture.Set(CapProp.FrameHeight, 720);
                if (!_capture.IsOpened)
                {
                    MessageBox.Show("Kameraya erişilemiyor. Farklı bir cihaz indeksini deneyin (0/1).");
                    Stop();
                    return;
                }

                _currentPersonDir = Path.Combine(Paths.FacesRootDirectory, Sanitize(personName));
                Directory.CreateDirectory(_currentPersonDir);
                _savedCount = 0;

                _running = true;
                Application.Idle += OnApplicationIdle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri toplama başlatılamadı: " + ex.Message);
                Stop();
            }
        }

        public void Stop()
        {
            _running = false;
            Application.Idle -= OnApplicationIdle;
            _capture?.Dispose();
            _capture = null;
            _faceDetector?.Dispose();
            _faceDetector = null;
        }

        private void OnApplicationIdle(object? sender, EventArgs e)
        {
            try
            {
                if (!_running || _capture == null) return;
                using var frame = _capture.QueryFrame();
                if (frame == null || frame.IsEmpty) return;

                using var image = frame.ToImage<Bgr, byte>();
                using var gray = image.Convert<Gray, byte>();

                // Yüz tespiti
                if (_faceDetector == null)
                {
                    _statusLabel.Text = "Hata: Cascade classifier yüklenmemiş";
                    return;
                }

                System.Drawing.Rectangle[]? faces = null;
                try
                {
                    faces = _faceDetector.DetectMultiScale(gray, 1.2, 8, new Size(80, 80));
                }
                catch (Emgu.CV.Util.CvException ex)
                {
                    MessageBox.Show($"Cascade hatası: {ex.Message}\n\nCascade classifier boş olabilir veya görüntü boyutu uygun değil.");
                    _statusLabel.Text = "Hata: Yüz tespiti başarısız";
                    Stop();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Yüz tespiti hatası: {ex.Message}");
                    _statusLabel.Text = "Hata: " + ex.Message;
                    Stop();
                    return;
                }
                if (faces != null)
                {
                    foreach (var rect in faces)
                    {
                        CvInvoke.Rectangle(image, rect, new MCvScalar(0, 255, 0), 2);

                        // ROI kırp ve normalize et
                        using var face = new Mat(gray.Mat, rect);
                        using var resized = new Mat();
                        CvInvoke.Resize(face, resized, new Size(200, 200));

                        if (_savedCount < TargetSamples)
                        {
                            string file = Path.Combine(_currentPersonDir, $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png");
                            resized.Save(file);
                            _savedCount++;
                            _statusLabel.Text = $"Durum: Kaydedildi ({_savedCount}/{TargetSamples})";
                        }
                    }
                }

                // Görüntüyü PictureBox'a göster
                _pictureBox.Image?.Dispose();
                _pictureBox.Image = image.ToBitmap();

                if (_savedCount >= TargetSamples)
                {
                    _statusLabel.Text = "Durum: Hedef örnek sayısına ulaşıldı";
                    Stop();
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Hata: " + ex.Message;
                Stop();
            }
        }

        private static string Sanitize(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input.Trim();
        }
    }
}


