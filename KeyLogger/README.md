# Keyboard Utilities

Modern bir WinForms (.NET 8) klavye yardımcı araçları uygulaması.

## 🚀 Özellikler

### 1. Hotkey Manager
- Global kısayol tuşları tanımlama (Ctrl+Alt+X gibi)
- Aksiyon tipleri: Uygulama Çalıştır, URL Aç, Metin Yaz, Komut Çalıştır
- JSON formatında ayar kaydetme/yükleme

### 2. Typing Tutor
- Örnek metinler üzerinde yazma pratiği
- Gerçek zamanlı WPM (Dakikadaki Kelime) hesaplama
- Doğruluk (Accuracy) yüzdesi
- Oturum geçmişi görüntüleme
- CSV/JSON export desteği

### 3. Key Display Overlay
- Always-on-top tuş gösterim penceresi
- Basılan tuşları görsel olarak gösterme
- Whitelist: Sadece belirli tuşları göster
- Şeffaflık ve pozisyon ayarları

### 4. Keyboard Assist
- Snippet tanımlama (örn: "btw" → "by the way")
- Profil desteği (Work, Personal, Coding)
- Otomatik snippet genişletme

## 📋 Gereksinimler

- .NET 8.0 SDK
- Windows 10/11

## 🔧 Kurulum

```bash
# Projeyi klonlayın
git clone <repo-url>
cd KeyLogger

# Derleyin
dotnet build --configuration Release

# Çalıştırın
dotnet run --project src/KeyboardUtils.App
```

## 📁 Proje Yapısı

```
KeyLogger/
├── src/
│   ├── KeyboardUtils.Core/       # Models, Interfaces, Events
│   ├── KeyboardUtils.Services/   # Business logic servisleri
│   └── KeyboardUtils.App/        # WinForms UI
├── README.md
├── PRD.md
└── .gitignore
```

## ⚙️ Ayarlar

Uygulama ayarları `%APPDATA%\KeyboardUtils\settings.json` dosyasında saklanır.

## 🔒 Gizlilik

> **ÖNEMLİ:** Bu uygulama kullanıcıyı gizlice izlemez. 
> - Tüm özellikler kullanıcı tarafından manuel olarak açılır
> - Keyboard hook sadece ilgili özellik aktifken çalışır
> - Hiçbir veri harici sunuculara gönderilmez

## 📄 Lisans

MIT License

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request açın
