# LLM Chatbot - Ürün Gereksinimleri Dokümanı (PRD)

## 1. Proje Özeti

**LLM Chatbot**, Windows Forms tabanlı bir masaüstü sohbet uygulamasıdır. OpenAI'nin Responses API'sini kullanarak kullanıcıların yapay zeka ile etkileşimli sohbet etmesini sağlar. Uygulama .NET 8 framework'ü üzerine inşa edilmiştir ve modern bir kullanıcı deneyimi sunar.

## 2. Amaçlar

- Kullanıcılara kolay kullanılabilir bir AI sohbet arayüzü sunmak
- Multi-turn (çok turlu) konuşmaları desteklemek
- Hızlı ve donmayan bir kullanıcı deneyimi sağlamak
- Güvenli API anahtar yönetimi uygulamak
- Temiz ve sürdürülebilir kod mimarisi oluşturmak

## 3. Hedef Kullanıcılar

| Kullanıcı Tipi | Açıklama |
|----------------|----------|
| Geliştiriciler | AI destekli kod yardımı almak isteyen yazılımcılar |
| Teknik Uzmanlar | Teknik sorularına hızlı yanıt arayanlar |
| Öğrenciler | Programlama ve teknik konularda yardım arayanlar |
| Genel Kullanıcılar | AI ile sohbet etmek isteyen herkes |

## 4. Kullanım Senaryoları

### Senaryo 1: İlk Kullanım
1. Kullanıcı uygulamayı başlatır
2. Sistem API anahtarı durumunu kontrol eder
3. Karşılama mesajı gösterilir
4. Kullanıcı mesaj yazmaya başlayabilir

### Senaryo 2: Sohbet Akışı
1. Kullanıcı mesaj yazar
2. Gönder butonuna tıklar veya Enter'a basar
3. "Yazıyor..." durumu görüntülenir
4. Bot yanıtı ekrana gelir
5. Kullanıcı yeni mesaj yazabilir (multi-turn)

### Senaryo 3: Hata Durumu
1. API hatası oluşur
2. Kullanıcıya okunabilir hata mesajı gösterilir
3. Kullanıcı tekrar deneyebilir

## 5. Fonksiyonel Gereksinimler

| ID | Gereksinim | Öncelik |
|----|------------|---------|
| FR-01 | Mesaj gönderme | Yüksek |
| FR-02 | Multi-turn sohbet desteği | Yüksek |
| FR-03 | Sohbet geçmişi görüntüleme | Yüksek |
| FR-04 | Durum göstergesi (Hazır/Yazıyor) | Orta |
| FR-05 | Otomatik scroll | Orta |
| FR-06 | Hata mesajları gösterimi | Yüksek |
| FR-07 | Boş mesaj engelleme | Düşük |

## 6. Teknik Gereksinimler

| Bileşen | Teknoloji |
|---------|-----------|
| Platform | Windows Forms |
| Framework | .NET 8 |
| Dil | C# |
| HTTP İstemci | HttpClient |
| JSON | System.Text.Json |
| API | OpenAI Responses API |

### Performans Gereksinimleri
- UI thread asla bloke olmamalı
- Async/await pattern zorunlu
- Yanıt süresi kullanıcıya gösterilmeli

## 7. Mimari Kararlar

### Katmanlı Mimari
```
┌─────────────────────────────────────┐
│            UI Katmanı               │
│         (MainForm.cs)               │
├─────────────────────────────────────┤
│          Service Katmanı            │
│        (OpenAiService.cs)           │
├─────────────────────────────────────┤
│           Core Katmanı              │
│  (ChatMessage.cs, Conversation.cs)  │
├─────────────────────────────────────┤
│          Common Katmanı             │
│        (ConfigHelper.cs)            │
└─────────────────────────────────────┘
```

### Tasarım Prensipleri
- **Single Responsibility**: Her sınıf tek bir sorumluluğa sahip
- **Dependency Inversion**: UI, servislere bağımlı değil abstraction'lara bağımlı
- **Separation of Concerns**: UI iş mantığından ayrı

## 8. Güvenlik Notları

> [!CAUTION]
> API anahtarları asla kaynak koda gömülmemelidir!

| Güvenlik Önlemi | Uygulama |
|-----------------|----------|
| API Key Saklama | Environment Variable (OPENAI_API_KEY) |
| Kod İçi Anahtar | ❌ Kesinlikle yasak |
| Hata Mesajları | API anahtarı ifşa etmez |
| HTTPS | Tüm API çağrıları HTTPS üzerinden |

## 9. Gelecek Geliştirmeler

### Kısa Vadeli (v1.1)
- [ ] Sohbet geçmişini dosyaya kaydetme
- [ ] Model seçimi dropdown
- [ ] Tema desteği (açık/koyu)

### Orta Vadeli (v1.5)
- [ ] Streaming response desteği
- [ ] Markdown rendering
- [ ] Kod syntax highlighting

### Uzun Vadeli (v2.0)
- [ ] Çoklu sohbet sekmeleri
- [ ] Eklenti sistemi
- [ ] Yerel model desteği (Ollama)

---

*Doküman Versiyonu: 1.0*  
*Son Güncelleme: Aralık 2024*
