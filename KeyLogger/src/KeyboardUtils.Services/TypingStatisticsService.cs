using System.Text;
using System.Text.Json;
using KeyboardUtils.Core.Interfaces;
using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Services;

/// <summary>
/// Yazma istatistikleri hesaplama servisi
/// </summary>
public class TypingStatisticsService : ITypingService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// WPM (Words Per Minute) hesapla
    /// Standart: 5 karakter = 1 kelime
    /// </summary>
    public double CalculateWpm(string typedText, double elapsedSeconds)
    {
        if (string.IsNullOrEmpty(typedText) || elapsedSeconds <= 0)
            return 0;

        double words = typedText.Length / 5.0;
        double minutes = elapsedSeconds / 60.0;
        
        return Math.Round(words / minutes, 1);
    }

    /// <summary>
    /// Accuracy (doğruluk) yüzdesi hesapla
    /// </summary>
    public double CalculateAccuracy(string targetText, string typedText)
    {
        if (string.IsNullOrEmpty(targetText))
            return 0;

        if (string.IsNullOrEmpty(typedText))
            return 0;

        int correctChars = 0;
        int minLength = Math.Min(targetText.Length, typedText.Length);

        for (int i = 0; i < minLength; i++)
        {
            if (targetText[i] == typedText[i])
            {
                correctChars++;
            }
        }

        // Fazla veya eksik karakter için penalty
        int maxLength = Math.Max(targetText.Length, typedText.Length);
        double accuracy = (double)correctChars / maxLength * 100;
        
        return Math.Round(Math.Max(0, accuracy), 1);
    }

    public TypingSession CreateSession(string targetText)
    {
        return new TypingSession
        {
            TargetText = targetText,
            Date = DateTime.Now,
            IsCompleted = false
        };
    }

    public TypingSession CompleteSession(TypingSession session, string typedText, double elapsedSeconds)
    {
        session.TypedText = typedText;
        session.DurationSeconds = elapsedSeconds;
        session.Wpm = CalculateWpm(typedText, elapsedSeconds);
        session.Accuracy = CalculateAccuracy(session.TargetText, typedText);
        session.TotalKeystrokes = typedText.Length;
        session.ErrorCount = CountErrors(session.TargetText, typedText);
        session.IsCompleted = true;
        
        return session;
    }

    private static int CountErrors(string targetText, string typedText)
    {
        int errors = 0;
        int minLength = Math.Min(targetText.Length, typedText.Length);

        for (int i = 0; i < minLength; i++)
        {
            if (targetText[i] != typedText[i])
            {
                errors++;
            }
        }

        // Eksik veya fazla karakterler de hata
        errors += Math.Abs(targetText.Length - typedText.Length);
        
        return errors;
    }

    public async Task<string> ExportHistoryToCsvAsync(IEnumerable<TypingSession> sessions, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ID,Tarih,WPM,Accuracy,Süre(sn),Tuş Sayısı,Hata,Zorluk,Tamamlandı");

        foreach (var session in sessions)
        {
            sb.AppendLine($"{session.Id},{session.Date:yyyy-MM-dd HH:mm:ss},{session.Wpm},{session.Accuracy},{session.DurationSeconds},{session.TotalKeystrokes},{session.ErrorCount},{session.Difficulty},{session.IsCompleted}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
        return filePath;
    }

    public async Task<string> ExportHistoryToJsonAsync(IEnumerable<TypingSession> sessions, string filePath)
    {
        var json = JsonSerializer.Serialize(sessions.ToList(), JsonOptions);
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        return filePath;
    }

    public IEnumerable<PracticeText> GetDefaultPracticeTexts()
    {
        return new List<PracticeText>
        {
            new()
            {
                Title = "Kolay - Temel Kelimeler",
                Content = "Merhaba dünya. Bu bir test metnidir. Klavye ile yazı yazmak kolaydır.",
                Difficulty = "Kolay",
                Category = "Temel"
            },
            new()
            {
                Title = "Kolay - Sayılar",
                Content = "1234567890 rakamları önemlidir. 10 elma, 20 armut, 30 portakal.",
                Difficulty = "Kolay",
                Category = "Sayılar"
            },
            new()
            {
                Title = "Orta - Pangram",
                Content = "The quick brown fox jumps over the lazy dog. Pack my box with five dozen liquor jugs.",
                Difficulty = "Orta",
                Category = "İngilizce"
            },
            new()
            {
                Title = "Orta - Türkçe Metin",
                Content = "Türkiye'nin en güzel şehirlerinden biri olan İstanbul, iki kıtayı birbirine bağlayan özel bir konuma sahiptir.",
                Difficulty = "Orta",
                Category = "Türkçe"
            },
            new()
            {
                Title = "Zor - Programlama",
                Content = "public static void Main(string[] args) { Console.WriteLine(\"Hello, World!\"); return; }",
                Difficulty = "Zor",
                Category = "Kod"
            },
            new()
            {
                Title = "Zor - Özel Karakterler",
                Content = "@#$%^&*()_+-=[]{}|;':\",./<>? gibi özel karakterleri yazmak pratik gerektirir!",
                Difficulty = "Zor",
                Category = "Özel"
            },
            new()
            {
                Title = "Uzun - Makale",
                Content = "Yazılım geliştirme, karmaşık sistemlerin tasarımı, oluşturulması ve bakımı ile ilgili disiplinler arası bir alandır. Modern yazılım mühendisliği, kullanıcı deneyimi, güvenlik, performans ve ölçeklenebilirlik gibi birçok faktörü göz önünde bulundurmayı gerektirir. İyi bir yazılım mühendisi, hem teknik becerilere hem de problem çözme yeteneğine sahip olmalıdır.",
                Difficulty = "Orta",
                Category = "Uzun Metin"
            }
        };
    }
}
