using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;

namespace FaceApp
{
    // Gerçek zamanlı yüz tanıma: Eğitilmiş modeli yükler, tespit edilen yüzleri tahmin eder ve isim yazar
    public class FaceRecognition
    {
        private readonly PictureBox _pictureBox;
        private readonly Label _statusLabel;
        private VideoCapture? _capture;
        private CascadeClassifier? _faceDetector;
        private FaceRecognizer? _recognizer;
        private Dictionary<int, string> _labelToName = new();
        private bool _running;

        public FaceRecognition(PictureBox pictureBox, Label statusLabel)
        {
            _pictureBox = pictureBox;
            _statusLabel = statusLabel;
            Directory.CreateDirectory(Paths.ModelsDirectory);
        }

        public bool Start(FaceModelType modelType)
        {
            try
            {
                var cascadePath = Paths.GetCascadePathForLoading();
                if (!File.Exists(cascadePath))
                {
                    MessageBox.Show("Haar cascade dosyası bulunamadı: " + cascadePath);
                    return false;
                }

                // Mutlak yolu kullan ve cascade'i yükle
                cascadePath = Path.GetFullPath(cascadePath);
                
                // Dosya varlığını ve boyutunu kontrol et
                if (!File.Exists(cascadePath))
                {
                    MessageBox.Show($"Haar cascade dosyası bulunamadı:\n{cascadePath}");
                    return false;
                }
                
                var fileInfo = new FileInfo(cascadePath);
                if (fileInfo.Length < 1000) // Dosya çok küçük, muhtemelen bozuk
                {
                    MessageBox.Show($"Haar cascade dosyası çok küçük (bozuk olabilir):\n{cascadePath}\nBoyut: {fileInfo.Length} byte\n\nLütfen dosyayı yeniden indirin.");
                    return false;
                }
                
                try
                {
                    _faceDetector = new CascadeClassifier(cascadePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Haar cascade dosyası yüklenemedi:\n{ex.Message}\n\nYol: {cascadePath}\n\nDosya bozuk olabilir. Lütfen OpenCV deposundan yeniden indirin:\nhttps://raw.githubusercontent.com/opencv/opencv/4.x/data/haarcascades/haarcascade_frontalface_default.xml");
                    return false;
                }

                string modelPath = modelType == FaceModelType.LBPH ? Paths.LbphModelFile : Paths.EigenModelFile;
                if (!File.Exists(modelPath)) return false;

                _recognizer = modelType == FaceModelType.LBPH
                    ? new LBPHFaceRecognizer(1, 8, 8, 8, 80)
                    : new EigenFaceRecognizer(80, 4000);

                _recognizer.Read(modelPath);
                _labelToName = LoadLabels();

                _capture = new VideoCapture(0, VideoCapture.API.DShow);
                _capture.Set(CapProp.FrameWidth, 1280);
                _capture.Set(CapProp.FrameHeight, 720);
                _running = true;
                Application.Idle += OnApplicationIdle;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tanıma başlatma hatası: " + ex.Message);
                return false;
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
            _recognizer?.Dispose();
            _recognizer = null;
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
                        using var face = new Mat(gray.Mat, rect);
                        using var resized = new Mat();
                        CvInvoke.Resize(face, resized, new Size(200, 200));

                        string labelText = "Bilinmiyor";
                        if (_recognizer != null)
                        {
                            var result = _recognizer.Predict(resized.ToImage<Gray, byte>());
                            if (result.Label >= 0 && _labelToName.TryGetValue(result.Label, out var name))
                            {
                                labelText = $"{name} (d={result.Distance:0})";
                            }
                        }

                        CvInvoke.Rectangle(image, rect, new MCvScalar(0, 255, 0), 2);
                        var textPoint = new Point(rect.X, Math.Max(0, rect.Y - 10));
                        CvInvoke.PutText(image, labelText, textPoint, Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.8, new MCvScalar(50, 220, 50), 2);
                    }
                }

                _pictureBox.Image?.Dispose();
                _pictureBox.Image = image.ToBitmap();
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Hata: " + ex.Message;
                Stop();
            }
        }

        private static Dictionary<int, string> LoadLabels()
        {
            var dict = new Dictionary<int, string>();
            string labelsFile = Path.Combine(Paths.ModelsDirectory, "labels.txt");
            if (!File.Exists(labelsFile)) return dict;
            foreach (var line in File.ReadAllLines(labelsFile))
            {
                var parts = line.Split(';');
                if (parts.Length == 2 && int.TryParse(parts[0], out int id))
                {
                    dict[id] = parts[1];
                }
            }
            return dict;
        }
    }
}


