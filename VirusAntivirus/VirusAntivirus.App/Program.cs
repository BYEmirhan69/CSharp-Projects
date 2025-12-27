using VirusAntivirus.Common;

namespace VirusAntivirus.App;

static class Program
{
    /// <summary>
    /// Uygulamanın ana giriş noktası.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Logger'ı başlat
        Logger.Initialize();
        Logger.Info("Uygulama başlatılıyor...");

        // Gerekli klasörleri oluştur
        Config.EnsureDirectoriesExist();

        // Eski logları temizle (30 günden eski)
        Logger.CleanOldLogs();

        // Windows Forms yapılandırması
        ApplicationConfiguration.Initialize();

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            Logger.Error("Kritik hata oluştu", ex);
            MessageBox.Show(
                $"Kritik bir hata oluştu:\n{ex.Message}",
                "Virus Antivirüs - Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Logger.Info("Uygulama kapatıldı.");
            Logger.Flush();
        }
    }
}
