using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace FaceApp
{
    public enum FaceModelType
    {
        LBPH,
        Eigen
    }

    // Kayıtlı yüzlerden model eğitimini yapar ve Models klasörüne kaydeder
    public class FaceTrain
    {
        private readonly Label _statusLabel;

        public FaceTrain(Label statusLabel)
        {
            _statusLabel = statusLabel;
            Directory.CreateDirectory(Paths.ModelsDirectory);
        }

        public bool Train(FaceModelType modelType)
        {
            var faceImages = new List<Image<Gray, byte>>();
            var labels = new List<int>();
            var labelToName = new Dictionary<int, string>();

            if (!Directory.Exists(Paths.FacesRootDirectory))
            {
                MessageBox.Show("Faces klasörü bulunamadı.");
                return false;
            }

            int currentLabel = 0;
            foreach (var personDir in Directory.GetDirectories(Paths.FacesRootDirectory))
            {
                string personName = Path.GetFileName(personDir);
                if (string.IsNullOrWhiteSpace(personName)) continue;
                labelToName[currentLabel] = personName;

                var images = Directory.EnumerateFiles(personDir, "*.png")
                    .Concat(Directory.EnumerateFiles(personDir, "*.jpg"))
                    .Concat(Directory.EnumerateFiles(personDir, "*.jpeg"))
                    .ToList();

                foreach (var imgPath in images)
                {
                    try
                    {
                        using var img = new Image<Gray, byte>(imgPath);
                        faceImages.Add(img.Clone());
                        labels.Add(currentLabel);
                    }
                    catch
                    {
                        // Bozuk görselleri yoksay
                    }
                }

                currentLabel++;
            }

            if (faceImages.Count == 0)
            {
                MessageBox.Show("Eğitim için görüntü bulunamadı. Önce veri toplayın.");
                return false;
            }

            try
            {
                // EmguCV API'si için VectorOfMat ve VectorOfInt kullan
                using var imagesVector = new VectorOfMat();
                using var labelsVector = new VectorOfInt();
                
                foreach (var img in faceImages)
                {
                    imagesVector.Push(img.Mat);
                }
                labelsVector.Push(labels.ToArray());

                if (modelType == FaceModelType.LBPH)
                {
                    using var recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 80);
                    recognizer.Train(imagesVector, labelsVector);
                    recognizer.Write(Paths.LbphModelFile);
                }
                else
                {
                    // EigenFaceRecognizer parametreleri: (numComponents, threshold)
                    using var recognizer = new EigenFaceRecognizer(80, 4000);
                    recognizer.Train(imagesVector, labelsVector);
                    recognizer.Write(Paths.EigenModelFile);
                }

                // Etiket-ad eşlemesini kaydet
                File.WriteAllLines(Path.Combine(Paths.ModelsDirectory, "labels.txt"),
                    labelToName.Select(kv => $"{kv.Key};{kv.Value}"));

                _statusLabel.Text = "Durum: Model eğitildi";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eğitim hatası: " + ex.Message);
                return false;
            }
        }
    }
}


