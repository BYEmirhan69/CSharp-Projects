
# 🖥️ Chat-Desktop1 (TCP Chat Server)

Bu proje, **C# .NET Framework** kullanılarak geliştirilmiş basit bir **TCP tabanlı Chat Server** uygulamasıdır.  
Sunucu, belirtilen port üzerinden istemcilerden gelen bağlantıları dinler ve mesaj alışverişini yönetir.

---

## 🚀 Özellikler
- TCP protokolüyle çoklu istemci bağlantı desteği  
- Dinamik port seçimi  
- Sunucu başlatma/durdurma işlemleri  
- Bağlantı ve mesaj kayıtlarının log ekranında gösterimi  
- Hata yönetimi ve güvenli thread kapatma  
- **DevExpress XtraForm** arayüzüyle modern görünüm

---

## 🧩 Teknolojiler
- C# (.NET Framework)
- Windows Forms (WinForms)
- DevExpress UI Components
- TCP/IP Socket Programming

---

## ⚙️ Nasıl Çalışır?

1. Uygulama başlatıldığında “Status : Stopped” olarak görünür.  
2. İlgili port numarası (örnek: **8888**) girilir.  
3. **Start Server** butonuna basıldığında:
   - TCP Listener başlar.
   - Gelen istemci bağlantıları dinlenir.
4. İstemci mesajları alındıkça log ekranına yazılır.
5. **Stop Server** butonuna basıldığında:
   - Tüm bağlantılar güvenli şekilde kapatılır.
   - Listener thread sonlandırılır.

---

## 🧑‍💻 Geliştirici
**Emirhan A.**  
📧 emirhanayd69@gmail.com  
💻 Yazılım Mühendisliği / Fırat Üniversitesi  

---

## 📜 Lisans
Bu proje MIT Lisansı ile lisanslanmıştır.  
İstediğiniz gibi kullanabilir, geliştirebilir ve dağıtabilirsiniz.


