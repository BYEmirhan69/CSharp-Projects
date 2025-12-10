using System;
using System.Threading;
using System.Windows.Forms;

namespace SpeechToText
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın giriş noktası.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // UI thread exceptions
            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // Non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Application.Run(new MainForm());
        }
    }
}


