# Product Requirements Document (PRD)

## Keyboard Utilities - WinForms Desktop Application

### Genel Bakış

Keyboard Utilities, kullanıcıların klavye kullanımını geliştirmek ve optimize etmek için tasarlanmış kapsamlı bir Windows masaüstü uygulamasıdır.

---

## Hedefler

1. **Verimlilik Artışı**: Global hotkey'ler ve snippet'ler ile hızlı işlemler
2. **Beceri Geliştirme**: Typing tutor ile yazma hızı ve doğruluğunu artırma
3. **Görselleştirme**: Key overlay ile tuş basımlarını görsel olarak takip
4. **Kişiselleştirme**: Profiller ve özelleştirilebilir ayarlar

---

## Fonksiyonel Gereksinimler

### FR-1: Hotkey Manager

| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-1.1 | Kullanıcı global hotkey tanımlayabilmeli | Yüksek |
| FR-1.2 | Hotkey kombinasyonları: Ctrl, Alt, Shift + herhangi tuş | Yüksek |
| FR-1.3 | Aksiyon tipleri: RunApp, OpenURL, TypeText, RunCommand | Yüksek |
| FR-1.4 | Hotkey listesi JSON formatında kaydedilmeli | Yüksek |
| FR-1.5 | Çakışan hotkey'ler için uyarı gösterilmeli | Orta |

### FR-2: Typing Tutor

| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-2.1 | Örnek metinler üzerinde pratik yapılabilmeli | Yüksek |
| FR-2.2 | Gerçek zamanlı WPM hesaplama | Yüksek |
| FR-2.3 | Accuracy yüzdesi gösterimi | Yüksek |
| FR-2.4 | Oturum geçmişi kaydedilmeli | Orta |
| FR-2.5 | Geçmiş CSV/JSON olarak export edilebilmeli | Orta |
| FR-2.6 | Farklı zorluk seviyelerinde metinler | Düşük |

### FR-3: Key Display Overlay

| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-3.1 | Always-on-top overlay penceresi | Yüksek |
| FR-3.2 | Basılan tuşları gerçek zamanlı gösterme | Yüksek |
| FR-3.3 | Whitelist: Belirli tuşları filtreleme | Orta |
| FR-3.4 | Şeffaflık ayarı (0-100%) | Orta |
| FR-3.5 | Pozisyon ve boyut hatırlama | Orta |

### FR-4: Keyboard Assist

| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-4.1 | Snippet tanımlama (trigger → expansion) | Yüksek |
| FR-4.2 | Profil oluşturma ve yönetme | Yüksek |
| FR-4.3 | Aktif profil seçimi | Yüksek |
| FR-4.4 | Snippet'ler JSON formatında kaydedilmeli | Yüksek |
| FR-4.5 | Snippet arama ve filtreleme | Düşük |

---

## Fonksiyonel Olmayan Gereksinimler

### NFR-1: Performans
- Uygulama 2 saniye içinde başlamalı
- Keyboard hook 10ms altında tepki vermeli
- Bellek kullanımı 100MB altında kalmalı

### NFR-2: Güvenlik
- Kullanıcı gizlice izlenmemeli
- Tüm özellikler kullanıcı kontrolünde olmalı
- Keyboard hook sadece ilgili özellik aktifken çalışmalı
- Hiçbir veri harici sunuculara gönderilmemeli

### NFR-3: Kullanılabilirlik
- Modern, koyu tema arayüzü
- Sezgisel navigasyon
- Türkçe arayüz desteği

### NFR-4: Sürdürülebilirlik
- Clean Architecture (UI/Services/Core)
- Async/await pattern kullanımı
- Arayüz tabanlı servis tasarımı

---

## Teknik Spesifikasyonlar

| Özellik | Değer |
|---------|-------|
| Platform | Windows 10/11 |
| Framework | .NET 8.0 |
| UI Framework | Windows Forms |
| Dil | C# 12 |
| Ayar Formatı | JSON |
| Mimari | Clean Architecture |

---

## Kapsam Dışı

- macOS/Linux desteği
- Bulut senkronizasyonu
- Çoklu dil desteği (sadece Türkçe)
- Sistem tray'de çalışma (opsiyonel)

---

## Başarı Kriterleri

1. ✅ Tüm 4 özellik çalışır durumda
2. ✅ Ayarlar JSON'da doğru kaydedilip yükleniyor
3. ✅ Keyboard hook güvenilir çalışıyor
4. ✅ Uygulama stabil ve performanslı
5. ✅ Kullanıcı gizliliği korunuyor
