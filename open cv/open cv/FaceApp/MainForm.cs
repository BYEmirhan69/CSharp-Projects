using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace FaceApp
{
    public class MainForm : Form
    {
        private PictureBox _pictureBox;
        private TextBox _nameTextBox;
        private Button _btnStartCapture;
        private Button _btnStopCapture;
        private Button _btnTrain;
        private Button _btnStartRecognition;
        private Button _btnStopRecognition;
        private Label _statusLabel;

        private FaceCapture? _faceCapture;
        private FaceTrain? _faceTrain;
        private FaceRecognition? _faceRecognition;

        public MainForm()
        {
            Text = "EmguCV Yüz Tanıma";
            Width = 900;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 480,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var nameLabel = new Label
            {
                Text = "İsim:",
                AutoSize = true,
                Top = 490,
                Left = 12
            };

            _nameTextBox = new TextBox
            {
                Top = 486,
                Left = 60,
                Width = 200
            };

            _btnStartCapture = new Button { Text = "Veri Topla Başlat", Top = 520, Left = 12, Width = 150 };
            _btnStopCapture = new Button { Text = "Veri Topla Durdur", Top = 520, Left = 170, Width = 150, Enabled = false };
            _btnTrain = new Button { Text = "Model Eğit", Top = 520, Left = 330, Width = 120 };
            _btnStartRecognition = new Button { Text = "Tanımayı Başlat", Top = 520, Left = 460, Width = 150 };
            _btnStopRecognition = new Button { Text = "Tanımayı Durdur", Top = 520, Left = 620, Width = 150, Enabled = false };

            _statusLabel = new Label { Text = "Durum: Hazır", AutoSize = true, Top = 560, Left = 12 };

            Controls.Add(_pictureBox);
            Controls.Add(nameLabel);
            Controls.Add(_nameTextBox);
            Controls.Add(_btnStartCapture);
            Controls.Add(_btnStopCapture);
            Controls.Add(_btnTrain);
            Controls.Add(_btnStartRecognition);
            Controls.Add(_btnStopRecognition);
            Controls.Add(_statusLabel);

            Directory.CreateDirectory(Paths.FacesRootDirectory);
            Directory.CreateDirectory(Paths.CascadeDirectory);

            _btnStartCapture.Click += (s, e) => StartCapture();
            _btnStopCapture.Click += (s, e) => StopCapture();
            _btnTrain.Click += (s, e) => TrainModel();
            _btnStartRecognition.Click += (s, e) => StartRecognition();
            _btnStopRecognition.Click += (s, e) => StopRecognition();
            this.FormClosing += (s, e) =>
            {
                try
                {
                    StopCapture();
                    StopRecognition();
                }
                catch { }
            };
        }

        private void StartCapture()
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                MessageBox.Show("Lütfen isim giriniz.");
                return;
            }

            StopRecognition();

            _faceCapture = new FaceCapture(_pictureBox, _statusLabel);
            _faceCapture.Start(_nameTextBox.Text.Trim());
            _btnStartCapture.Enabled = false;
            _btnStopCapture.Enabled = true;
            _statusLabel.Text = "Durum: Veri toplama başladı";
        }

        private void StopCapture()
        {
            _faceCapture?.Stop();
            _btnStartCapture.Enabled = true;
            _btnStopCapture.Enabled = false;
            _statusLabel.Text = "Durum: Veri toplama durdu";
        }

        private void TrainModel()
        {
            StopCapture();
            StopRecognition();

            _faceTrain = new FaceTrain(_statusLabel);
            // Varsayılan olarak LBPH kullan, istenirse Eigen için parametre ile değiştirilebilir
            bool trained = _faceTrain.Train(modelType: FaceModelType.LBPH);
            _statusLabel.Text = trained ? "Durum: Eğitim tamamlandı" : "Durum: Eğitim başarısız";
        }

        private void StartRecognition()
        {
            StopCapture();
            _faceRecognition = new FaceRecognition(_pictureBox, _statusLabel);
            bool ok = _faceRecognition.Start(modelType: FaceModelType.LBPH);
            if (ok)
            {
                _btnStartRecognition.Enabled = false;
                _btnStopRecognition.Enabled = true;
                _statusLabel.Text = "Durum: Tanıma başladı";
            }
            else
            {
                _statusLabel.Text = "Durum: Model bulunamadı. Önce eğitim yapın.";
            }
        }

        private void StopRecognition()
        {
            _faceRecognition?.Stop();
            _btnStartRecognition.Enabled = true;
            _btnStopRecognition.Enabled = false;
        }
    }

    public static class Paths
    {
        public static readonly string FacesRootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Faces");
        public static readonly string ModelsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
        public static readonly string CascadeDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Assets", "haarcascades"));
        // Çalışma anında kopyalanmamışsa, proje dizinindeki dosyayı da arar
        public static string GetHaarCascadePath()
        {
            string runtimePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "haarcascades", "haarcascade_frontalface_default.xml");
            if (File.Exists(runtimePath)) return runtimePath;

            // Geliştirme sırasında bin/Debug/net8.0-windows konumundan 3 kez yukarı çık
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                string devPath = Path.Combine(projectRoot, "FaceApp", "Assets", "haarcascades", "haarcascade_frontalface_default.xml");
                if (File.Exists(devPath)) return devPath;
                // Alternatif: çözüm kökünde ise
                devPath = Path.Combine(projectRoot, "Assets", "haarcascades", "haarcascade_frontalface_default.xml");
                if (File.Exists(devPath)) return devPath;
            }
            catch { }

            return runtimePath; // Uyarı mesajı bu yolu gösterecek
        }

        // OpenCV/EmguCV bazen Unicode karakter içeren yollarda XML yükleyemiyor.
        // Bu nedenle cascade dosyasını her zaman ASCII karakterli TEMP klasörüne kopyalayıp oradan yükleriz.
        public static string GetCascadePathForLoading()
        {
            string source = GetHaarCascadePath();
            
            if (!File.Exists(source))
            {
                return source; // Dosya yoksa kaynak yolu döndür, hata başka yerde yakalanacak
            }

            try
            {
                // Her zaman temp'e kopyala (güvenli yol garantisi için)
                // TempPath genellikle ASCII karakterli (örn: C:\Users\...\AppData\Local\Temp)
                string tempDir = Path.Combine(Path.GetTempPath(), "faceapp_haarcascade");
                
                // Klasörü oluştur (yoksa)
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                
                string tempFile = Path.Combine(tempDir, "haarcascade_frontalface_default.xml");
                
                // Dosyayı kopyala (overwrite = true) - her zaman güncel olması için
                File.Copy(source, tempFile, overwrite: true);
                
                // Kopyalanan dosyanın var olduğunu ve yeterli boyutta olduğunu doğrula
                if (File.Exists(tempFile))
                {
                    var fi = new FileInfo(tempFile);
                    if (fi.Length > 1000) // En az 1KB olmalı (bozuk dosya kontrolü)
                    {
                        // Temp dosyası başarılı - ASCII karakterli yol
                        return tempFile;
                    }
                    else
                    {
                        // Dosya çok küçük, bozuk olabilir
                        MessageBox.Show($"Cascade dosyası temp'e kopyalandı ancak boyutu yetersiz: {fi.Length} byte\nKaynak: {source}", "Uyarı");
                    }
                }
            }
            catch (Exception ex)
            {
                // Kopyalama başarısız - kullanıcıya bildir
                MessageBox.Show($"Cascade dosyası temp'e kopyalanamadı:\n{ex.Message}\n\nKaynak yol kullanılacak: {source}", "Uyarı");
            }

            // Son çare: Kaynak yolu döndür (Unicode karakterli olabilir, sorun olabilir)
            return source;
        }
        public static readonly string LbphModelFile = Path.Combine(ModelsDirectory, "model_lbph.yml");
        public static readonly string EigenModelFile = Path.Combine(ModelsDirectory, "model_eigen.yml");
    }
}


