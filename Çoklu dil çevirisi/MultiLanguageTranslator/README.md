# Çoklu Dil Çevirisi (Multi-Language Translation)

Windows Forms tabanlı çoklu dil çeviri uygulaması. Türkçe ve İngilizce arayüz desteği ile birden fazla dile aynı anda çeviri yapabilirsiniz.

## 🚀 Özellikler

### Çeviri Özellikleri
- ✅ Türkçe ve İngilizce'den çeviri desteği
- ✅ Birden fazla hedef dile aynı anda çeviri
- ✅ Desteklenen hedef diller: İngilizce, Almanca, Fransızca, İspanyolca, Türkçe
- ✅ Dictionary tabanlı mock çeviri motoru
- ✅ Düzenli ve okunabilir çeviri sonuçları

### Uygulama İçi Çoklu Dil (Localization)
- ✅ Türkçe (varsayılan) ve İngilizce arayüz
- ✅ Anlık dil değişimi (uygulama yeniden başlatma gerektirmez)
- ✅ Resource (.resx) dosyaları ile lokalizasyon

### Arayüz Özellikleri
- ✅ Modern ve temiz tasarım
- ✅ Responsive (yeniden boyutlandırılabilir) pencere
- ✅ SplitContainer, TableLayoutPanel, FlowLayoutPanel kullanımı
- ✅ Kullanıcı dostu hata mesajları

## 📁 Proje Yapısı

```
MultiLanguageTranslator/
├── MultiLanguageTranslator.csproj
├── Program.cs
├── MainForm.cs
├── Services/
│   └── MockTranslationEngine.cs
├── Resources/
│   ├── Strings.resx (Türkçe)
│   └── Strings.en.resx (İngilizce)
├── README.md
├── PRD.md
└── .gitignore
```

## 🛠️ Gereksinimler

- .NET 8.0 SDK
- Windows işletim sistemi
- Visual Studio 2022 veya Visual Studio Code

## 📦 Kurulum

### 1. Projeyi Klonlayın
```bash
git clone https://github.com/kullanici/multi-language-translator.git
cd multi-language-translator/MultiLanguageTranslator
```

### 2. Projeyi Derleyin
```bash
dotnet build
```

### 3. Uygulamayı Çalıştırın
```bash
dotnet run
```

## 🎮 Kullanım

1. **Kaynak Dil Seçin**: Sol panelden kaynak dili (Türkçe veya İngilizce) seçin
2. **Metin Girin**: Çevrilecek metni üst metin kutusuna yazın
3. **Hedef Dilleri Seçin**: Çevirmek istediğiniz dilleri işaretleyin
4. **Çevir**: "Çevir" butonuna tıklayın
5. **Sonuçları Görüntüleyin**: Sağ panelde tüm çeviriler görünecektir

### Arayüz Dilini Değiştirme
- Sağ üst köşedeki "Arayüz Dili" dropdown'ından Türkçe veya English seçin
- Arayüz anında değişecektir

## 📸 Ekran Görüntüleri

### Ana Ekran (Türkçe Arayüz)
![Ana Ekran TR](screenshots/main-screen-tr.png)

### Ana Ekran (İngilizce Arayüz)
![Ana Ekran EN](screenshots/main-screen-en.png)

### Çeviri Sonuçları
![Çeviri Sonuçları](screenshots/translation-results.png)

## 🔧 Geliştirme

### Visual Studio 2022
1. `MultiLanguageTranslator.csproj` dosyasını açın
2. F5 ile çalıştırın

### Visual Studio Code
1. Klasörü VS Code ile açın
2. Terminal'de `dotnet run` komutunu çalıştırın

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request açın

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👤 Geliştirici

Bu proje eğitim ve demonstrasyon amaçlı geliştirilmiştir.

---

**Not**: Bu uygulama gerçek çeviri API'si kullanmaz. Mock çeviri motoru dictionary tabanlı basit bir çeviri sistemi sunar.
