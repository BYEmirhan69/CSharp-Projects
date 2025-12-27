# Virus Antivirüs

Windows için kullanıcı modunda çalışan, on-demand (isteğe bağlı) antivirüs uygulaması.

## 🛡️ Özellikler

- **Dosya Tarama**: Tek dosya seçerek hızlı tarama
- **Klasör Tarama**: Recursive alt klasör tarama desteği
- **Hash Tabanlı Tespit**: SHA-256 ile bilinen zararlı imza eşleştirme
- **Heuristik Analiz**: Çift uzantı, şüpheli konum, entropy analizi ile risk skorlama
- **JSON Raporlama**: Detaylı tarama raporları
- **Karantina**: Tehditli dosyaları güvenli şekilde izole etme
- **Paralel Tarama**: 1-8 thread ile hızlı tarama
- **Exclude Patterns**: İstenmeyen klasör/dosyaları hariç tutma

## 📋 Gereksinimler

- Windows 10/11
- .NET 8.0 Runtime
- Visual Studio 2022 (geliştirme için)

## 🚀 Kurulum

### Kaynak Koddan Derleme

```bash
git clone https://github.com/kullanici/VirusAntivirus.git
cd VirusAntivirus
dotnet build
```

### Çalıştırma

```bash
dotnet run --project VirusAntivirus.App
```

Veya Visual Studio'da `VirusAntivirus.sln` dosyasını açın ve F5 ile çalıştırın.

## 📁 Proje Yapısı

```
VirusAntivirus/
├── VirusAntivirus.sln
├── VirusAntivirus.App/          # WinForms UI
├── VirusAntivirus.Engine/       # Tarama motoru
├── VirusAntivirus.Common/       # Ortak yardımcılar
├── signatures.json              # İmza veritabanı
├── PRD.md                       # Ürün gereksinimleri
└── README.md
```

## 🔧 Kullanım

1. Uygulamayı başlatın
2. **Dosya Tara**: Tek dosya taramak için
3. **Klasör Tara**: Tüm klasörü recursive taramak için
4. Sonuçlar tabloda görüntülenir
5. Satıra sağ tıklayarak:
   - Karantinaya Al
   - Dosya Konumunu Aç
   - Detayları Gör

### Exclude Patterns
Taramadan hariç tutmak istediğiniz klasör/dosya isimlerini alt alta yazın:
```
bin
obj
.git
node_modules
```

### Tarama Modları
- **Fast**: Hızlı tarama, düşük eşikler
- **Full**: Detaylı tarama, tüm kontroller

## 📝 signatures.json Güncelleme

İmza veritabanını güncellemek için `signatures.json` dosyasını düzenleyin:

```json
[
  {
    "name": "TestMalware",
    "sha256": "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
    "severity": "Malware"
  }
]
```

Alanlar:
- `name`: Tehdit adı
- `sha256`: Dosyanın SHA-256 hash değeri (küçük harf)
- `severity`: Tehdit seviyesi (`Malware`, `PUP`, `Adware`)

## 📊 Raporlar

Tarama raporları şu konumda oluşturulur:
```
VirusAntivirus.App/bin/Debug/net8.0-windows/Reports/
```

Rapor formatı: `scan_report_YYYYMMDD_HHMMSS.json`

## 🔒 Karantina

Karantinaya alınan dosyalar:
```
%LOCALAPPDATA%\VirusAntivirus\Quarantine\
```

Her dosya için:
- `<sha256>.quarantine`: Karantinaya alınmış dosya
- `<sha256>.meta.json`: Orijinal konum ve metadata

## 📸 Ekran Görüntüsü

![Virus Antivirüs Ana Ekran](docs/screenshot.png)

*Ekran görüntüsü eklenmeli*

## ⚠️ Sınırlamalar

- Real-time koruma yok (sadece on-demand tarama)
- Kernel driver kullanmaz
- Arşiv içi tarama yok (gelecek sürümde)
- Sadece Windows desteği

## 📄 Lisans

Bu proje eğitim amaçlıdır. MIT Lisansı altında dağıtılmaktadır.

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/yeniOzellik`)
3. Commit yapın (`git commit -m 'Yeni özellik eklendi'`)
4. Push yapın (`git push origin feature/yeniOzellik`)
5. Pull Request açın

## 📞 İletişim

Sorularınız için issue açabilirsiniz.
