using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Chat_Desktop1
{
    public partial class FrmServer : XtraForm
    {
        private TcpListener server;
        private Thread listenThread;
        private bool isRunning = false;

        public FrmServer()
        {
            InitializeComponent();
        }

        private void Frm1_Load(object sender, EventArgs e)
        {
            lblDurum.Text = "Status : Stopped";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(textPort.Text);
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                isRunning = true;

                AppendLog($"Server started on port {port}");
                lblDurum.Text = "Status : Running";

                listenThread = new Thread(ListenForClients)
                {
                    IsBackground = true
                };
                listenThread.Start();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error starting server: " + ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                isRunning = false;

                // Yeni bağlantı bekleyen listener'ı güvenli şekilde durdur
                server?.Stop();

                // Thread durması için 1 saniye bekle
                if (listenThread != null && listenThread.IsAlive)
                {
                    listenThread.Join(500);
                }

                lblDurum.Text = "Status : Stopped";
                AppendLog("Server stopped.");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error stopping server: " + ex.Message);
            }
        }


      private void ListenForClients()
{
    while (isRunning)
    {
        try
        {
            // Eğer yeni bağlantı yoksa kısa bir uyku ver, CPU'yu yorma
            if (!server.Pending())
            {
                Thread.Sleep(100);
                continue;
            }

            TcpClient client = server.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClientComm)
            {
                IsBackground = true
            };
            clientThread.Start(client);
        }
        catch (SocketException)
        {
            // Server.Stop() çağrıldığında buraya düşer – sorun değil
            break;
        }
        catch (Exception ex)
        {
            AppendLog("Listener error: " + ex.Message);
            break;
        }
    }

    AppendLog("Listener thread stopped.");
}


        private void HandleClientComm(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            AppendLog("Client connected...");

            try
            {
                while (isRunning)
                {
                    string msg = reader.ReadLine();
                    if (msg == null) break;
                    AppendLog("Client: " + msg);
                }
            }
            catch (Exception ex)
            {
                AppendLog("Client error: " + ex.Message);
            }
            finally
            {
                client.Close();
                AppendLog("Client disconnected.");
            }
        }

        private void AppendLog(string msg)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => AppendLog(msg)));
            }
            else
            {
                txtLog.Text += msg + Environment.NewLine;
            }
        }
    }
}
