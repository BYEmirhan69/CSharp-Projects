using LLMChatbot.WinForms.UI;

namespace LLMChatbot.WinForms;

/// <summary>
/// Uygulama giriş noktası.
/// Windows Forms uygulamasını başlatır.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Ana giriş noktası.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Windows Forms yapılandırmasını başlat
        ApplicationConfiguration.Initialize();
        
        // Ana formu çalıştır
        Application.Run(new MainForm());
    }
}
