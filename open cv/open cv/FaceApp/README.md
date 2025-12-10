EmguCV Yüz Tanıma (WinForms)

Gereksinimler:
- .NET 8 SDK
- Visual Studio 2022 (Windows Forms bileşenleri)
- NuGet: Emgu.CV, Emgu.CV.UI, Emgu.CV.World, Emgu.CV.runtime.windows

Kurulum:
1) `Assets/haarcascades` klasörüne `haarcascade_frontalface_default.xml` kopyalayın.
2) Visual Studio ile `FaceApp.csproj` açın, NuGet paketlerini geri yükleyin.
3) F5 ile çalıştırın.

Akış:
- Veri Topla Başlat: İsim girin, `Faces/<isim>` altına ~50 örnek kaydeder.
- Model Eğit: LBPH varsayılan, modeli `Models/model_lbph.yml` olarak kaydeder.
- Tanımayı Başlat: Kamerada isim bindirmesi yapar.

Notlar:
- Kamera indeksini `0/1` değiştirerek doğru cihaza erişin.
- Eigen için `FaceModelType.Eigen` seçebilirsiniz.

