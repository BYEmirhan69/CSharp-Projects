# YoloWinForms 🚀  
YOLOv8 ONNX ile C# WinForms Nesne Tespiti

## 📌 Proje Hakkında
**YoloWinForms**, C# WinForms kullanılarak geliştirilmiş bir masaüstü nesne tespit uygulamasıdır.  
Uygulama, **YOLOv8 ONNX** modeli ile seçilen görseller üzerinde nesne tespiti yapar, bounding box’ları çizer ve sonucu kaydetmenizi sağlar.

Proje **VS Code** ile geliştirilmiştir ve **Visual Studio Designer kullanılmamıştır**.  
Tüm arayüz kod ile oluşturulmuştur.

---

## 🎯 Özellikler
- 🖼️ JPG / PNG görsel seçme
- 🤖 YOLOv8 ONNX ile nesne tespiti
- 📐 640x640 letterbox resize
- 🎯 Ayarlanabilir confidence (güven eşiği)
- ✂️ Non-Maximum Suppression (NMS)
- 🟩 Bounding box çizimi
- 💾 Sonuç görselini kaydetme (PNG / JPG)
- ⚡ `dotnet run` ile tek komut çalıştırma

---

## 🛠️ Kullanılan Teknolojiler
- **.NET 8**
- **C#**
- **WinForms**
- **Microsoft.ML.OnnxRuntime**
- **ImageSharp**
- **YOLOv8 (ONNX)**

---

## 📂 Proje Yapısı

```text
YoloWinForms/
├── Program.cs
├── Form1.cs
├── YoloV8Onnx.cs
├── YoloWinForms.csproj
├── PRD.md
├── README.md
└── models/
    └── yolov8n.onnx

⚙️ Kurulum

1️⃣ Gereksinimler

.NET 8 SDK

VS Code (önerilir)

Windows işletim sistemi

2️⃣ Model Dosyası

YOLOv8 ONNX modelini indirin:

bash
Kodu kopyala
pip install ultralytics
yolo export model=yolov8n.pt format=onnx opset=12
Oluşan yolov8n.onnx dosyasını şu dizine kopyalayın:

text
Kodu kopyala
YoloWinForms/models/yolov8n.onnx

▶️ Çalıştırma

Proje klasöründe terminal açın:

bash
Kodu kopyala
dotnet restore
dotnet run

🧪 Kullanım

Görsel butonuna basarak bir resim seçin

Tespit Et butonuna basın

Nesneler bounding box ile işaretlenir

Kaydet butonu ile sonucu diske kaydedin

📌 Notlar

YOLOv8’de objectness yoktur, sadece class confidence kullanılır

Sınıf isimleri yerine class ID gösterilmektedir

Video, webcam ve model eğitimi bu projenin kapsamı dışındadır

❌ Kapsam Dışı

Video işleme

Webcam desteği

Model eğitimi

COCO class isimleri

✅ Başarı Kriterleri

dotnet run ile sorunsuz çalışması

Nesne tespiti yapabilmesi

UI donmaması

Sonuç görselinin kaydedilebilmesi

👨‍💻 Geliştirici Notu

Bu proje, bilgisayarlı görü ve ONNX inference mantığını öğrenmek amacıyla hazırlanmış bir demo uygulamadır.

İleride eklenebilecek özellikler:

COCO class name desteği

Webcam / video inference

CUDA hızlandırma

Tek .exe publish