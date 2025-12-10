using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isConnected;
        private string clientName;

        public ClientForm()
        {
            InitializeComponent();
            isConnected = false;
        }

        private void InitializeComponent()
        {
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblServerIP = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblClientName = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtMessages
            // 
            this.txtMessages.Location = new System.Drawing.Point(16, 74);
            this.txtMessages.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessages.Size = new System.Drawing.Size(745, 307);
            this.txtMessages.TabIndex = 0;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(16, 394);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(612, 48);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(107, 15);
            this.txtServerIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(132, 22);
            this.txtServerIP.TabIndex = 2;
            this.txtServerIP.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(320, 15);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(79, 22);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "8888";
            // 
            // txtClientName
            // 
            this.txtClientName.Location = new System.Drawing.Point(467, 15);
            this.txtClientName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(132, 22);
            this.txtClientName.TabIndex = 4;
            this.txtClientName.Text = "User";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(613, 12);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 28);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(720, 12);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 28);
            this.btnDisconnect.TabIndex = 6;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(640, 394);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 49);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblServerIP
            // 
            this.lblServerIP.AutoSize = true;
            this.lblServerIP.Location = new System.Drawing.Point(16, 18);
            this.lblServerIP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerIP.Name = "lblServerIP";
            this.lblServerIP.Size = new System.Drawing.Size(65, 16);
            this.lblServerIP.TabIndex = 8;
            this.lblServerIP.Text = "Server IP:";
            this.lblServerIP.Click += new System.EventHandler(this.lblServerIP_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(267, 18);
            this.lblPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(34, 16);
            this.lblPort.TabIndex = 9;
            this.lblPort.Text = "Port:";
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Location = new System.Drawing.Point(413, 18);
            this.lblClientName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(34, 16);
            this.lblClientName.TabIndex = 10;
            this.lblClientName.Text = "İsim:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(16, 49);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 16);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "Durum: Bağlı Değil";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 458);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblClientName);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblServerIP);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtClientName);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtMessages);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ClientForm";
            this.Text = "Chat Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private TextBox txtMessages;
        private TextBox txtMessage;
        private TextBox txtServerIP;
        private TextBox txtPort;
        private TextBox txtClientName;
        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnSend;
        private Label lblServerIP;
        private Label lblPort;
        private Label lblClientName;
        private Label lblStatus;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtClientName.Text.Trim()))
                {
                    MessageBox.Show("Lütfen bir isim girin!");
                    return;
                }

                clientName = txtClientName.Text.Trim();
                client = new TcpClient();
                client.Connect(txtServerIP.Text, int.Parse(txtPort.Text));
                stream = client.GetStream();

                isConnected = true;
                receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                // İlk olarak ismi gönder
                byte[] nameData = Encoding.UTF8.GetBytes(clientName);
                stream.Write(nameData, 0, nameData.Length);

                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnSend.Enabled = true;
                lblStatus.Text = "Durum: Bağlı";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                txtMessage.Focus();
                AddMessage("Sunucuya bağlandı: " + clientName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && (ModifierKeys & Keys.Shift) == 0)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (!isConnected || string.IsNullOrEmpty(txtMessage.Text.Trim()))
                return;

            try
            {
                string message = txtMessage.Text.Trim();

                // 🔻 Önemli: Satır sonu ekle (\n)
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                stream.Write(data, 0, data.Length);

                // Kendi mesajınızı hemen ekranda göster
                AddMessage(clientName + ": " + message);

                txtMessage.Clear();
                txtMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesaj gönderme hatası: " + ex.Message);
                Disconnect();
            }
        }


        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (isConnected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // Sadece diğer kullanıcıların mesajlarını göster (kendi mesajımızı tekrar gösterme)
                    if (!message.StartsWith(clientName + ":"))
                    {
                        AddMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    AddMessage("Bağlantı hatası: " + ex.Message);
                }
            }
            finally
            {
                Disconnect();
            }
        }

        private void AddMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddMessage), message);
                return;
            }

            txtMessages.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + message + Environment.NewLine);
            txtMessages.ScrollToCaret();
        }

        private void Disconnect()
        {
            try
            {
                isConnected = false;
                stream?.Close();
                client?.Close();

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
                lblStatus.Text = "Durum: Bağlı Değil";
                lblStatus.ForeColor = System.Drawing.Color.Red;

                AddMessage("Sunucudan ayrıldı.");
            }
            catch (Exception ex)
            {
                AddMessage("Bağlantı kesme hatası: " + ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isConnected)
            {
                Disconnect();
            }
            base.OnFormClosing(e);
        }

        private void lblServerIP_Click(object sender, EventArgs e)
        {

        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm());
        }
    }
}
