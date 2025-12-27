using KeyboardUtils.Core.Models;

namespace KeyboardUtils.Core.Interfaces;

/// <summary>
/// Typing istatistik servisi arayüzü
/// </summary>
public interface ITypingService
{
    /// <summary>WPM hesapla</summary>
    double CalculateWpm(string typedText, double elapsedSeconds);
    
    /// <summary>Accuracy hesapla</summary>
    double CalculateAccuracy(string targetText, string typedText);
    
    /// <summary>Yazma oturumu oluştur</summary>
    TypingSession CreateSession(string targetText);
    
    /// <summary>Oturumu bitir ve istatistikleri hesapla</summary>
    TypingSession CompleteSession(TypingSession session, string typedText, double elapsedSeconds);
    
    /// <summary>Geçmişi CSV olarak dışa aktar</summary>
    Task<string> ExportHistoryToCsvAsync(IEnumerable<TypingSession> sessions, string filePath);
    
    /// <summary>Geçmişi JSON olarak dışa aktar</summary>
    Task<string> ExportHistoryToJsonAsync(IEnumerable<TypingSession> sessions, string filePath);
    
    /// <summary>Örnek pratik metinleri getir</summary>
    IEnumerable<PracticeText> GetDefaultPracticeTexts();
}
