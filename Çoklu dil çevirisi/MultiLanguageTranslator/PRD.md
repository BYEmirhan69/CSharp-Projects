# PRD - Çoklu Dil Çevirisi Uygulaması
## Product Requirements Document

---

## 1. Proje Özeti

**Proje Adı**: Çoklu Dil Çevirisi (Multi-Language Translation)

**Açıklama**: Windows Forms tabanlı masaüstü çeviri uygulaması. Kullanıcıların metin girerek birden fazla dile aynı anda çeviri yapmasını sağlar. Uygulama arayüzü Türkçe ve İngilizce olarak değiştirilebilir.

**Platform**: Windows

**Teknoloji Stack**:
- C# (.NET 8.0)
- Windows Forms (WinForms)
- Resource Files (.resx) for Localization

---

## 2. Hedefler

### Birincil Hedefler
1. Kullanıcı dostu çoklu dil çeviri arayüzü oluşturmak
2. Birden fazla hedef dile aynı anda çeviri desteği sağlamak
3. Uygulama içi çoklu dil (localization) desteği sunmak
4. Modern ve responsive tasarım uygulamak

### İkincil Hedefler
1. Genişletilebilir çeviri motoru mimarisi
2. Kolay bakım yapılabilir kod yapısı
3. GitHub'a hazır proje yapısı

---

## 3. Fonksiyonel Gereksinimler

### 3.1 Metin Girişi
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-01 | Çok satırlı metin girişi (RichTextBox) | Yüksek |
| FR-02 | Kopyala/Yapıştır desteği | Yüksek |
| FR-03 | Scroll desteği | Orta |

### 3.2 Kaynak Dil Seçimi
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-04 | ComboBox ile kaynak dil seçimi | Yüksek |
| FR-05 | En az 2 kaynak dil desteği (TR, EN) | Yüksek |

### 3.3 Hedef Dil Seçimi
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-06 | CheckedListBox ile çoklu dil seçimi | Yüksek |
| FR-07 | En az 4 hedef dil desteği | Yüksek |
| FR-08 | Tıkla-seç/kaldır özelliği | Orta |

### 3.4 Çeviri İşlemi
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-09 | "Çevir" butonu ile çeviri başlatma | Yüksek |
| FR-10 | Seçilen tüm hedef dillere çeviri | Yüksek |
| FR-11 | Aynı anda sonuç gösterimi | Yüksek |

### 3.5 Çeviri Çıktısı
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-12 | Her dil için ayrı çıktı alanı | Yüksek |
| FR-13 | Dil adı açıkça görünür olmalı | Yüksek |
| FR-14 | Scroll edilebilir sonuç alanı | Orta |
| FR-15 | Dil bayrak emojileri | Düşük |

### 3.6 Localization
| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-16 | Türkçe UI desteği (varsayılan) | Yüksek |
| FR-17 | İngilizce UI desteği | Yüksek |
| FR-18 | Anlık dil değişimi | Yüksek |
| FR-19 | Resource dosyaları ile yönetim | Yüksek |

---

## 4. Fonksiyonel Olmayan Gereksinimler

### 4.1 Performans
- Uygulama 2 saniye içinde açılmalı
- Çeviri işlemi 1 saniye içinde tamamlanmalı

### 4.2 Kullanılabilirlik
- Sezgisel ve kolay kullanım
- Kullanıcı dostu hata mesajları
- Responsive pencere tasarımı

### 4.3 Güvenilirlik
- Uygulama hiçbir koşulda çökmemeli
- Tüm hatalar yakalanmalı ve kullanıcıya gösterilmeli

### 4.4 Bakım Kolaylığı
- Temiz ve okunabilir kod
- Modüler mimari
- İyi belgelenmiş

---

## 5. UI/UX Tasarımı

### 5.1 Ana Pencere Düzeni

```
┌─────────────────────────────────────────────────────────────┐
│  [Logo/Başlık]                      [Arayüz Dili: TR/EN ▼] │
├──────────────────────────┬──────────────────────────────────┤
│                          │                                  │
│  ┌─ Giriş ─────────────┐ │  ┌─ Çeviri Sonuçları ─────────┐ │
│  │                     │ │  │                            │ │
│  │  [Kaynak Metin]     │ │  │  🇬🇧 İngilizce             │ │
│  │                     │ │  │  [Çevirilen metin...]      │ │
│  │                     │ │  │                            │ │
│  ├─────────────────────┤ │  │  🇩🇪 Almanca               │ │
│  │  Kaynak Dil: [▼]    │ │  │  [Çevirilen metin...]      │ │
│  ├─────────────────────┤ │  │                            │ │
│  │  Hedef Diller:      │ │  │  🇫🇷 Fransızca             │ │
│  │  ☑ İngilizce        │ │  │  [Çevirilen metin...]      │ │
│  │  ☑ Almanca          │ │  │                            │ │
│  │  ☐ Fransızca        │ │  │  🇪🇸 İspanyolca            │ │
│  │  ☐ İspanyolca       │ │  │  [Çevirilen metin...]      │ │
│  ├─────────────────────┤ │  │                            │ │
│  │      [ÇEVİR]        │ │  └────────────────────────────┘ │
│  └─────────────────────┘ │                                  │
│                          │                                  │
└──────────────────────────┴──────────────────────────────────┘
```

### 5.2 Renk Paleti
| Öğe | Renk | Hex |
|-----|------|-----|
| Üst Panel | Koyu Mavi | #34495E |
| Arka Plan | Beyaz-Gri | #F5F5F5 |
| Çevir Butonu | Yeşil | #2ECC71 |
| Başlıklar | Koyu Gri | #2C3E50 |
| Dil Başlığı | Mavi | #2980B9 |

### 5.3 Font
- Ana Font: Segoe UI
- Başlık: 11pt Bold
- İçerik: 10pt Regular

---

## 6. Çeviri Motoru

### 6.1 Mock Çeviri Yaklaşımı
- Gerçek API kullanılmıyor
- Dictionary tabanlı kelime eşleme
- Bilinmeyen kelimeler `[kelime]` formatında gösteriliyor

### 6.2 Desteklenen Dil Çiftleri
| Kaynak | Hedef Diller |
|--------|--------------|
| Türkçe | İngilizce, Almanca, Fransızca, İspanyolca |
| İngilizce | Türkçe, Almanca, Fransızca, İspanyolca |

### 6.3 Sözlük Kapasitesi
- Her dil çifti için ~50 kelime
- Temel günlük kullanım kelimeleri
- Selamlaşma, sayılar, renkler, nesneler

---

## 7. Hata Yönetimi

| Hata Durumu | Mesaj (TR) | Mesaj (EN) |
|-------------|------------|------------|
| Boş metin | Lütfen çevrilecek bir metin girin. | Please enter text to translate. |
| Hedef dil yok | Lütfen en az bir hedef dil seçin. | Please select at least one target language. |
| Aynı dil | Kaynak dil ile hedef dil aynı olamaz. | Source and target languages cannot be the same. |

---

## 8. Gelecek Geliştirmeler

### Faz 2 (Planlanan)
- [ ] Gerçek çeviri API entegrasyonu (Google Translate, DeepL)
- [ ] Daha fazla dil desteği
- [ ] Çeviri geçmişi
- [ ] Metin dosyası içe/dışa aktarma

### Faz 3 (Gelecek)
- [ ] Sesli okuma özelliği
- [ ] Klavye kısayolları
- [ ] Tema desteği (Açık/Koyu mod)
- [ ] Sözlük genişletme arayüzü

---

## 9. Teknik Detaylar

### 9.1 Dosya Yapısı
```
MultiLanguageTranslator/
├── MultiLanguageTranslator.csproj    # Proje dosyası
├── Program.cs                         # Giriş noktası
├── MainForm.cs                        # Ana form (UI)
├── Services/
│   └── MockTranslationEngine.cs      # Çeviri motoru
├── Resources/
│   ├── Strings.resx                  # TR kaynakları
│   └── Strings.en.resx               # EN kaynakları
├── README.md                          # Proje dokümantasyonu
├── PRD.md                             # Bu dosya
└── .gitignore                         # Git ignore dosyası
```

### 9.2 Layout Kontrolleri
- **SplitContainer**: Sol-sağ panel ayrımı
- **TableLayoutPanel**: Form içi düzen
- **FlowLayoutPanel**: Sonuç kartları

---

## 10. Sonuç

Bu PRD, Çoklu Dil Çevirisi uygulamasının tüm gereksinimlerini ve özelliklerini tanımlar. Uygulama, kullanıcı dostu bir arayüz ile birden fazla dile çeviri yapma ve uygulama içi dil değiştirme özelliklerini sunar.

---

**Doküman Versiyonu**: 1.0  
**Son Güncelleme**: 2024  
**Durum**: Tamamlandı ✅
