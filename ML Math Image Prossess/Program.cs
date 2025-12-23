// ===== Program.cs =====
// YoloWinForms uygulamasının giriş noktası
// .NET 8 WinForms uygulaması için standart başlatma kodu

namespace YoloWinForms
{
    /// <summary>
    /// Uygulamanın ana giriş noktası sınıfı
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana giriş noktası metodu
        /// High DPI desteği ve görsel stiller etkinleştirilir
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Yüksek DPI desteğini etkinleştir
            ApplicationConfiguration.Initialize();
            
            // Ana formu başlat ve uygulamayı çalıştır
            Application.Run(new Form1());
        }
    }
}
