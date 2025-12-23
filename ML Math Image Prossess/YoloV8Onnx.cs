// ===== YoloV8Onnx.cs =====
// YOLOv8 ONNX model işlemleri için sınıf
// Nesne tespiti, NMS ve bounding box hesaplamaları bu sınıfta yapılır

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

// Alias tanımlamaları - namespace çakışmalarını önlemek için
using ISImage = SixLabors.ImageSharp.Image;
using ISRectangleF = SixLabors.ImageSharp.RectangleF;
using ISPointF = SixLabors.ImageSharp.PointF;
using ISFont = SixLabors.Fonts.Font;
using ISFontStyle = SixLabors.Fonts.FontStyle;
using ISSystemFonts = SixLabors.Fonts.SystemFonts;

namespace YoloWinForms
{
    /// <summary>
    /// Tespit edilen nesne bilgilerini tutan yapı
    /// </summary>
    public class Detection
    {
        /// <summary>Sınıf ID'si (0-79 arası COCO sınıfları)</summary>
        public int ClassId { get; set; }
        
        /// <summary>Güven skoru (0.0 - 1.0 arası)</summary>
        public float Confidence { get; set; }
        
        /// <summary>Bounding box sol üst köşe X koordinatı</summary>
        public float X { get; set; }
        
        /// <summary>Bounding box sol üst köşe Y koordinatı</summary>
        public float Y { get; set; }
        
        /// <summary>Bounding box genişliği</summary>
        public float Width { get; set; }
        
        /// <summary>Bounding box yüksekliği</summary>
        public float Height { get; set; }

        /// <summary>
        /// Bounding box'ın dikdörtgen temsilini döndürür
        /// </summary>
        public SixLabors.ImageSharp.RectangleF BoundingBox => new SixLabors.ImageSharp.RectangleF(X, Y, Width, Height);
    }

    /// <summary>
    /// YOLOv8 ONNX modeli ile nesne tespiti yapan sınıf
    /// </summary>
    public class YoloV8Onnx : IDisposable
    {
        // ONNX çalışma zamanı oturumu
        private readonly InferenceSession _session;
        
        // Model giriş boyutu (640x640)
        private const int InputWidth = 640;
        private const int InputHeight = 640;
        
        // YOLOv8n için sınıf sayısı (COCO dataset)
        private const int NumClasses = 80;
        
        // NMS için IoU eşik değeri
        private const float IouThreshold = 0.45f;

        // Letterbox padding bilgileri
        private float _scale;
        private float _padX;
        private float _padY;

        // COCO dataset sınıf isimleri
        private static readonly string[] CocoClassNames = new string[]
        {
            "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat", "traffic light",
            "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow",
            "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee",
            "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard",
            "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
            "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "couch",
            "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse", "remote", "keyboard",
            "cell phone", "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase",
            "scissors", "teddy bear", "hair drier", "toothbrush"
        };

        /// <summary>
        /// Sınıf ID'sine göre sınıf ismini döndürür
        /// </summary>
        public static string GetClassName(int classId)
        {
            if (classId >= 0 && classId < CocoClassNames.Length)
                return CocoClassNames[classId];
            return $"class_{classId}";
        }

        /// <summary>
        /// YOLOv8 ONNX modelini yükler
        /// </summary>
        /// <param name="modelPath">ONNX model dosyasının yolu</param>
        public YoloV8Onnx(string modelPath)
        {
            // Model dosyasının varlığını kontrol et
            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException($"ONNX model dosyası bulunamadı: {modelPath}");
            }

            // ONNX oturumunu oluştur
            var sessionOptions = new SessionOptions();
            sessionOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
            
            _session = new InferenceSession(modelPath, sessionOptions);
        }

        /// <summary>
        /// Görsel üzerinde nesne tespiti yapar
        /// </summary>
        /// <param name="imagePath">Görsel dosyasının yolu</param>
        /// <param name="confidenceThreshold">Güven eşik değeri (0.0 - 1.0)</param>
        /// <returns>Tespit edilen nesnelerin listesi</returns>
        public List<Detection> Detect(string imagePath, float confidenceThreshold)
        {
            // Görseli yükle
            using var image = ISImage.Load<Rgb24>(imagePath);
            return Detect(image, confidenceThreshold);
        }

        /// <summary>
        /// Görsel üzerinde nesne tespiti yapar
        /// </summary>
        /// <param name="image">ImageSharp görsel nesnesi</param>
        /// <param name="confidenceThreshold">Güven eşik değeri (0.0 - 1.0)</param>
        /// <returns>Tespit edilen nesnelerin listesi</returns>
        public List<Detection> Detect(Image<Rgb24> image, float confidenceThreshold)
        {
            // Orijinal boyutları sakla
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Letterbox resize uygula ve tensor oluştur
            var inputTensor = PreprocessImage(image);

            // Model giriş adını al
            var inputName = _session.InputMetadata.Keys.First();

            // Girişleri hazırla
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor)
            };

            // Çıkarım yap
            using var results = _session.Run(inputs);

            // Çıkışı işle
            var output = results.First().AsTensor<float>();
            var outputShape = output.Dimensions.ToArray();

            // Tespitleri parse et
            var detections = ParseOutput(output, outputShape, confidenceThreshold, originalWidth, originalHeight);

            // NMS uygula
            var nmsDetections = ApplyNMS(detections);

            return nmsDetections;
        }

        /// <summary>
        /// Görseli model girişi için hazırlar (Letterbox resize + normalizasyon)
        /// </summary>
        private DenseTensor<float> PreprocessImage(Image<Rgb24> image)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Letterbox resize için ölçek ve padding hesapla
            float scale = Math.Min((float)InputWidth / originalWidth, (float)InputHeight / originalHeight);
            int newWidth = (int)(originalWidth * scale);
            int newHeight = (int)(originalHeight * scale);

            // Padding hesapla (merkeze yerleştirmek için)
            int padX = (InputWidth - newWidth) / 2;
            int padY = (InputHeight - newHeight) / 2;

            // Padding bilgilerini sakla (sonradan koordinat dönüşümü için)
            _scale = scale;
            _padX = padX;
            _padY = padY;

            // Yeni görsel oluştur (letterbox)
            using var resizedImage = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(newWidth, newHeight),
                    Mode = ResizeMode.Stretch
                });
            });

            // 640x640 siyah arka plan oluştur
            using var paddedImage = new Image<Rgb24>(InputWidth, InputHeight, new Rgb24(114, 114, 114));

            // Resize edilmiş görseli merkeze yerleştir
            paddedImage.Mutate(ctx =>
            {
                ctx.DrawImage(resizedImage, new SixLabors.ImageSharp.Point(padX, padY), 1f);
            });

            // Tensor oluştur [1, 3, 640, 640] - NCHW format
            var tensor = new DenseTensor<float>(new[] { 1, 3, InputHeight, InputWidth });

            // Piksel değerlerini normalize et ve tensöre aktar
            paddedImage.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < InputHeight; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (int x = 0; x < InputWidth; x++)
                    {
                        var pixel = row[x];
                        // RGB değerlerini 0-1 arasına normalize et
                        tensor[0, 0, y, x] = pixel.R / 255f; // R
                        tensor[0, 1, y, x] = pixel.G / 255f; // G
                        tensor[0, 2, y, x] = pixel.B / 255f; // B
                    }
                }
            });

            return tensor;
        }

        /// <summary>
        /// Model çıkışını parse eder ve tespitleri çıkarır
        /// YOLOv8 çıkış formatları desteklenir: [1, 84, 8400] veya [1, 8400, 84]
        /// </summary>
        private List<Detection> ParseOutput(Tensor<float> output, int[] shape, float confidenceThreshold,
            int originalWidth, int originalHeight)
        {
            var detections = new List<Detection>();

            // Çıkış shape'ini belirle
            // YOLOv8: [1, 84, 8400] veya [1, 8400, 84]
            // 84 = 4 (x, y, w, h) + 80 (class scores)
            bool isTransposed = shape[1] == 84; // [1, 84, 8400] formatı
            int numDetections = isTransposed ? shape[2] : shape[1];
            int numOutputs = isTransposed ? shape[1] : shape[2];

            for (int i = 0; i < numDetections; i++)
            {
                // Bounding box koordinatları (center x, center y, width, height)
                float cx, cy, w, h;
                
                if (isTransposed)
                {
                    // [1, 84, 8400] formatı
                    cx = output[0, 0, i];
                    cy = output[0, 1, i];
                    w = output[0, 2, i];
                    h = output[0, 3, i];
                }
                else
                {
                    // [1, 8400, 84] formatı
                    cx = output[0, i, 0];
                    cy = output[0, i, 1];
                    w = output[0, i, 2];
                    h = output[0, i, 3];
                }

                // En yüksek sınıf skorunu bul
                float maxScore = 0;
                int maxClassId = 0;

                for (int c = 0; c < NumClasses; c++)
                {
                    float score;
                    if (isTransposed)
                    {
                        score = output[0, 4 + c, i];
                    }
                    else
                    {
                        score = output[0, i, 4 + c];
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxClassId = c;
                    }
                }

                // Güven eşiğini kontrol et
                if (maxScore < confidenceThreshold)
                    continue;

                // Koordinatları orijinal görsel boyutuna dönüştür
                // Önce letterbox padding'i kaldır
                float x1 = (cx - w / 2 - _padX) / _scale;
                float y1 = (cy - h / 2 - _padY) / _scale;
                float x2 = (cx + w / 2 - _padX) / _scale;
                float y2 = (cy + h / 2 - _padY) / _scale;

                // Sınırları kontrol et
                x1 = Math.Max(0, Math.Min(originalWidth, x1));
                y1 = Math.Max(0, Math.Min(originalHeight, y1));
                x2 = Math.Max(0, Math.Min(originalWidth, x2));
                y2 = Math.Max(0, Math.Min(originalHeight, y2));

                // Geçerli bir bounding box mu kontrol et
                if (x2 - x1 < 1 || y2 - y1 < 1)
                    continue;

                detections.Add(new Detection
                {
                    ClassId = maxClassId,
                    Confidence = maxScore,
                    X = x1,
                    Y = y1,
                    Width = x2 - x1,
                    Height = y2 - y1
                });
            }

            return detections;
        }

        /// <summary>
        /// Non-Maximum Suppression (NMS) uygular
        /// Örtüşen bounding box'ları filtreler
        /// </summary>
        private List<Detection> ApplyNMS(List<Detection> detections)
        {
            if (detections.Count == 0)
                return detections;

            // Güven skoruna göre sırala (yüksekten düşüğe)
            var sorted = detections.OrderByDescending(d => d.Confidence).ToList();
            var result = new List<Detection>();
            var used = new bool[sorted.Count];

            for (int i = 0; i < sorted.Count; i++)
            {
                if (used[i])
                    continue;

                result.Add(sorted[i]);

                for (int j = i + 1; j < sorted.Count; j++)
                {
                    if (used[j])
                        continue;

                    // Aynı sınıf için IoU hesapla
                    if (sorted[i].ClassId == sorted[j].ClassId)
                    {
                        float iou = CalculateIoU(sorted[i].BoundingBox, sorted[j].BoundingBox);
                        if (iou > IouThreshold)
                        {
                            used[j] = true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// İki dikdörtgen arasındaki IoU (Intersection over Union) değerini hesaplar
        /// </summary>
        private float CalculateIoU(SixLabors.ImageSharp.RectangleF box1, SixLabors.ImageSharp.RectangleF box2)
        {
            float x1 = Math.Max(box1.Left, box2.Left);
            float y1 = Math.Max(box1.Top, box2.Top);
            float x2 = Math.Min(box1.Right, box2.Right);
            float y2 = Math.Min(box1.Bottom, box2.Bottom);

            float intersectionArea = Math.Max(0, x2 - x1) * Math.Max(0, y2 - y1);
            float box1Area = box1.Width * box1.Height;
            float box2Area = box2.Width * box2.Height;
            float unionArea = box1Area + box2Area - intersectionArea;

            return unionArea > 0 ? intersectionArea / unionArea : 0;
        }

        /// <summary>
        /// Tespitleri görsel üzerine çizer
        /// </summary>
        /// <param name="imagePath">Orijinal görsel yolu</param>
        /// <param name="detections">Tespit listesi</param>
        /// <returns>Bounding box'lar çizilmiş görsel</returns>
        public static Image<Rgb24> DrawDetections(string imagePath, List<Detection> detections)
        {
            var image = ISImage.Load<Rgb24>(imagePath);
            DrawDetectionsOnImage(image, detections);
            return image;
        }

        /// <summary>
        /// Tespitleri mevcut görsel üzerine çizer
        /// </summary>
        public static void DrawDetectionsOnImage(Image<Rgb24> image, List<Detection> detections)
        {
            // Her sınıf için farklı renk oluştur
            var colors = GenerateColors(NumClasses);

            // Varsayılan font kullan
            var fontFamily = ISSystemFonts.Families.FirstOrDefault();
            ISFont? font = null;
            
            if (fontFamily.Name != null)
            {
                font = fontFamily.CreateFont(14, ISFontStyle.Bold);
            }

            foreach (var detection in detections)
            {
                var color = colors[detection.ClassId % colors.Length];
                var rect = detection.BoundingBox;

                // Bounding box çiz
                image.Mutate(ctx =>
                {
                    // Dikdörtgen çiz
                    ctx.Draw(color, 2f, new ISRectangleF(rect.X, rect.Y, rect.Width, rect.Height));

                    // Etiket metni
                    string label = $"{GetClassName(detection.ClassId)}: {detection.Confidence:P0}";

                    if (font != null)
                    {
                        // Metin arka planı için dikdörtgen boyutu
                        var textOptions = new RichTextOptions(font)
                        {
                            Origin = new ISPointF(rect.X, rect.Y - 20)
                        };

                        // Arka plan dikdörtgeni
                        var textBounds = TextMeasurer.MeasureSize(label, textOptions);
                        var bgRect = new ISRectangleF(rect.X, rect.Y - 22, textBounds.Width + 6, textBounds.Height + 4);
                        ctx.Fill(color, bgRect);

                        // Metni çiz
                        ctx.DrawText(label, font, SixLabors.ImageSharp.Color.White, new ISPointF(rect.X + 3, rect.Y - 20));
                    }
                });
            }
        }

        /// <summary>
        /// Sınıflar için renk paleti oluşturur
        /// </summary>
        private static SixLabors.ImageSharp.Color[] GenerateColors(int count)
        {
            var colors = new SixLabors.ImageSharp.Color[count];
            for (int i = 0; i < count; i++)
            {
                // HSV renk uzayında eşit aralıklı renkler
                float hue = (float)i / count;
                var rgb = HsvToRgb(hue, 0.8f, 0.9f);
                colors[i] = SixLabors.ImageSharp.Color.FromRgb(rgb.r, rgb.g, rgb.b);
            }
            return colors;
        }

        /// <summary>
        /// HSV renk değerini RGB'ye dönüştürür
        /// </summary>
        private static (byte r, byte g, byte b) HsvToRgb(float h, float s, float v)
        {
            float r, g, b;

            int i = (int)(h * 6);
            float f = h * 6 - i;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (i % 6)
            {
                case 0: r = v; g = t; b = p; break;
                case 1: r = q; g = v; b = p; break;
                case 2: r = p; g = v; b = t; break;
                case 3: r = p; g = q; b = v; break;
                case 4: r = t; g = p; b = v; break;
                default: r = v; g = p; b = q; break;
            }

            return ((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>
        /// Kaynakları serbest bırakır
        /// </summary>
        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
