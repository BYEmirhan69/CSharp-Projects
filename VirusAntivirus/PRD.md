# Virus Antivirüs - Product Requirements Document

## Proje Özeti
Virus Antivirüs, Windows platformunda kullanıcı modunda çalışan, on-demand (isteğe bağlı) dosya ve klasör tarama yapan bir antivirüs uygulamasıdır. Hash tabanlı imza eşleştirme, heuristik analiz, risk skorlama, raporlama ve karantina özellikleri sunar.

## Amaçlar
- Kullanıcıların dosya ve klasörlerini zararlı yazılımlara karşı taramasını sağlamak
- SHA-256 hash tabanlı bilinen tehdit tespiti yapmak
- Heuristik analiz ile şüpheli dosyaları tespit etmek
- Tehdit bulunan dosyaları güvenli şekilde karantinaya almak
- Tarama sonuçlarını JSON formatında raporlamak

## Hedef Kullanıcılar
- Teknik bilgisi orta düzeyde Windows kullanıcıları
- Dosyalarını manuel olarak taramak isteyen son kullanıcılar
- Güvenlik farkındalığı yüksek bireyler

## Kullanım Senaryoları

### Senaryo 1: Tek Dosya Tarama
1. Kullanıcı "Dosya Tara" butonuna tıklar
2. Dosya seçim dialogu açılır
3. Dosya seçilir ve tarama başlar
4. Sonuç anında görüntülenir

### Senaryo 2: Klasör Tarama
1. Kullanıcı "Klasör Tara" butonuna tıklar
2. Klasör seçim dialogu açılır
3. Tüm alt klasörler dahil recursive tarama başlar
4. İlerleme çubuğu güncellenir
5. Tüm sonuçlar tablo halinde gösterilir

### Senaryo 3: Karantina İşlemi
1. Kullanıcı tehdit tespit edilen satıra sağ tıklar
2. "Karantinaya Al" seçeneğini seçer
3. Dosya karantina klasörüne taşınır
4. Orijinal konum ve metadata kaydedilir

## Fonksiyonel Gereksinimler

### FR-01: Dosya Tarama
- Tekil dosya seçip tarama yapılabilmeli
- SHA-256 hash hesaplanmalı
- İmza eşleşmesi kontrol edilmeli
- Heuristik analiz yapılmalı

### FR-02: Klasör Tarama
- Recursive alt klasör tarama
- Exclude pattern desteği (bin, obj, .git vb.)
- Paralel tarama (1-8 thread)
- Fast/Full mod seçimi

### FR-03: İmza Tabanlı Tespit
- signatures.json dosyasından imza yükleme
- SHA-256 hash eşleştirme
- Malware seviyesi belirleme

### FR-04: Heuristik Analiz
- Çift uzantı kontrolü (.pdf.exe)
- Tehlikeli script uzantıları tespiti
- Şüpheli konum kontrolü (Temp, AppData)
- Dosya boyutu anomali tespiti
- Entropy (rastgelelik) analizi
- 0-100 arası risk skoru

### FR-05: Raporlama
- JSON formatında rapor
- Tarih/saat damgalı dosya adı
- Reports klasörüne kayıt
- Özet ve detaylı sonuçlar

### FR-06: Karantina
- Güvenli klasöre taşıma
- Orijinal path metadata kaydı
- SHA-256 tabanlı yeniden adlandırma
- Silme işlemi yok

## Non-Functional Gereksinimler

### Performans
- Büyük dosyalar stream ile okunmalı
- Memory leak olmamalı
- Paralel tarama desteği
- UI responsive kalmalı

### Güvenlik
- Sadece okuma ve taşıma işlemleri
- Kullanıcı onayı ile karantina
- Zararlı kod çalıştırma yok

### Loglama
- Hata logları dosyaya yazılmalı
- Log rotasyonu desteklenmeli
- %LOCALAPPDATA%\VirusAntivirus\Logs konumunda

### Kullanılabilirlik
- Türkçe arayüz
- Sezgisel kontroller
- İlerleme gösterimi
- Hata mesajları kullanıcı dostu

## Mimari & Modüller

### VirusAntivirus.Common
- Logger: Dosya tabanlı loglama
- Config: Yol ve varsayılan ayarlar

### VirusAntivirus.Engine
- Scanning: Tarama servisleri
- Hashing: SHA-256 hesaplama
- Signatures: İmza veritabanı ve eşleştirme
- Heuristics: Risk analizi
- Quarantine: Karantina işlemleri
- Reporting: JSON rapor üretimi

### VirusAntivirus.App
- MainForm: Ana kullanıcı arayüzü
- DataGridView: Sonuç tablosu
- Context Menu: Sağ tık işlemleri

## Riskler & Sınırlamalar

### Mevcut Sınırlamalar
- Real-time koruma yok (sadece on-demand)
- Kernel driver yok
- Ağ taraması yok
- Arşiv içi tarama yok (V2'de planlanıyor)

### Bilinen Riskler
- Yeni/bilinmeyen tehditler tespit edilemeyebilir
- Heuristik false positive üretebilir
- Yönetici hakları gerektiren dosyalara erişilemez

## Roadmap

### V1.0 (Mevcut)
- ✅ Dosya/klasör tarama
- ✅ Hash tabanlı tespit
- ✅ Heuristik analiz
- ✅ JSON raporlama
- ✅ Karantina

### V2.0 (Planlanan)
- [ ] ZIP/RAR arşiv içi tarama
- [ ] Scheduled tarama
- [ ] İmza otomatik güncelleme
- [ ] YARA kuralları desteği

### V3.0 (Gelecek)
- [ ] Real-time dosya izleme
- [ ] Cloud tabanlı tehdit istihbaratı
- [ ] Davranış analizi
