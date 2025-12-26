# 🤖 LLM Chatbot

Windows Forms tabanlı, OpenAI destekli masaüstü sohbet uygulaması.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-UI-0078D4?style=flat-square&logo=windows)
![OpenAI](https://img.shields.io/badge/OpenAI-API-412991?style=flat-square&logo=openai)

## 📖 Proje Tanımı

LLM Chatbot, OpenAI'nin güçlü dil modellerini kullanarak kullanıcıların yapay zeka ile sohbet etmesini sağlayan modern bir masaüstü uygulamasıdır. .NET 8 ve Windows Forms ile geliştirilmiş olup, temiz mimari prensiplerine uygun şekilde tasarlanmıştır.

## ✨ Özellikler

- 💬 **Multi-turn Sohbet**: Konuşma bağlamını koruyan çok turlu diyaloglar
- ⚡ **Async İşlemler**: UI'ın donmadığı akıcı kullanıcı deneyimi
- 🎨 **Modern Arayüz**: Koyu tema ile göz yormayan tasarım
- 🔒 **Güvenli API Yönetimi**: Ortam değişkeni ile anahtar saklama
- 📜 **Sohbet Geçmişi**: Tüm mesajların görüntülenmesi
- ⌨️ **Klavye Kısayolları**: Enter ile hızlı mesaj gönderme

## 📸 Ekran Görüntüsü

<!-- Ekran görüntüsü buraya eklenecek -->
```
┌────────────────────────────────────────────┐
│ [Hazır]                              Status│
├────────────────────────────────────────────┤
│                                            │
│ [10:30] Bot                                │
│ Merhaba! Size nasıl yardımcı olabilirim?   │
│                                            │
│                         [10:31] Sen        │
│              C# nedir kısaca açıklar mısın?│
│                                            │
│ [10:31] Bot                                │
│ C#, Microsoft tarafından geliştirilen,     │
│ nesne yönelimli bir programlama dilidir.   │
│                                            │
├────────────────────────────────────────────┤
│ [Mesajınızı yazın...          ] [Gönder]   │
└────────────────────────────────────────────┘
```

## 🚀 Kurulum

### Gereksinimler
- Windows 10/11
- .NET 8 SDK veya Runtime
- OpenAI API Anahtarı

### Adımlar

1. **Projeyi klonlayın**
   ```bash
   git clone https://github.com/username/LLMChatbot.WinForms.git
   cd LLMChatbot.WinForms
   ```

2. **Bağımlılıkları yükleyin**
   ```bash
   dotnet restore
   ```

3. **Derleyin**
   ```bash
   dotnet build
   ```

## 🔑 Ortam Değişkeni Ayarı

API anahtarınızı ortam değişkeni olarak ayarlayın:

### PowerShell (Geçici)
```powershell
$env:OPENAI_API_KEY = "sk-your-api-key-here"
```

### CMD (Geçici)
```cmd
set OPENAI_API_KEY=sk-your-api-key-here
```

### Kalıcı Ayar (Windows)
1. **Sistem Özellikleri** > **Gelişmiş** > **Ortam Değişkenleri**
2. **Kullanıcı değişkenleri** altında **Yeni** tıklayın
3. Değişken adı: `OPENAI_API_KEY`
4. Değişken değeri: `sk-your-api-key-here`
5. **Tamam** ile kaydedin

> ⚠️ **Önemli**: API anahtarınızı asla kaynak koduna eklemeyin!

## ▶️ Çalıştırma

```bash
dotnet run
```

Veya derlenmiş uygulamayı doğrudan çalıştırın:
```bash
.\bin\Debug\net8.0-windows\LLMChatbot.WinForms.exe
```

## 📝 Kullanım

1. Uygulamayı başlatın
2. Alt kısımdaki metin kutusuna mesajınızı yazın
3. **Gönder** butonuna tıklayın veya **Enter** tuşuna basın
4. Yanıt için bekleyin (durum çubuğunda "Yazıyor..." görünecek)
5. Sohbete devam edin!

### Klavye Kısayolları

| Kısayol | İşlev |
|---------|-------|
| `Enter` | Mesaj gönder |
| `Shift + Enter` | Yeni satır ekle |

## 🛠️ Teknolojiler

| Teknoloji | Kullanım |
|-----------|----------|
| .NET 8 | Framework |
| C# | Programlama dili |
| Windows Forms | UI framework |
| HttpClient | API istekleri |
| System.Text.Json | JSON işleme |
| OpenAI Responses API | LLM backend |

## 📁 Proje Yapısı

```
LLMChatbot.WinForms/
├── UI/
│   └── MainForm.cs          # Ana form ve UI mantığı
├── Core/
│   ├── ChatMessage.cs       # Mesaj modeli
│   └── Conversation.cs      # Sohbet yönetimi
├── Services/
│   └── OpenAiService.cs     # API entegrasyonu
├── Common/
│   └── ConfigHelper.cs      # Yapılandırma
├── Program.cs               # Giriş noktası
├── PRD.md                   # Ürün gereksinimleri
├── README.md                # Bu dosya
└── .gitignore               # Git yoksayma kuralları
```

## 📄 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için [LICENSE](LICENSE) dosyasına bakın.

---

**Geliştirici**: LLM Chatbot Team  
**Versiyon**: 1.0.0
