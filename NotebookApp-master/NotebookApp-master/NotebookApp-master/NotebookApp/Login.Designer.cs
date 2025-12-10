namespace NotebookApp
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            txtUserName = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtPasword = new TextBox();
            btnLogin = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(475, 364);
            label1.Name = "label1";
            label1.Size = new Size(100, 22);
            label1.TabIndex = 0;
            label1.Text = "GİRİŞ YAP";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(206, 46);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(645, 298);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(491, 422);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(248, 23);
            txtUserName.TabIndex = 1;
            txtUserName.TextChanged += txtUserName_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.InactiveBorder;
            label2.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(319, 423);
            label2.Name = "label2";
            label2.Size = new Size(150, 22);
            label2.TabIndex = 3;
            label2.Text = "Kulannıcı Adı:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.InactiveBorder;
            label3.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(399, 479);
            label3.Name = "label3";
            label3.Size = new Size(70, 22);
            label3.TabIndex = 5;
            label3.Text = "Şifre:";
            // 
            // txtPasword
            // 
            txtPasword.Location = new Point(491, 478);
            txtPasword.Name = "txtPasword";
            txtPasword.Size = new Size(248, 23);
            txtPasword.TabIndex = 2;
            txtPasword.UseSystemPasswordChar = true;
            txtPasword.TextChanged += textBox1_TextChanged;
            // 
            // btnLogin
            // 
            btnLogin.Font = new Font("Consolas", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnLogin.Location = new Point(441, 564);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(134, 61);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Giriş Yap";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1089, 769);
            Controls.Add(btnLogin);
            Controls.Add(label3);
            Controls.Add(txtPasword);
            Controls.Add(label2);
            Controls.Add(txtUserName);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Name = "Login";
            Text = "Not Defteri Uygulaması Giriş Yap";
            Load += Login_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private TextBox txtUserName;
        private Label label2;
        private Label label3;
        private TextBox txtPasword;
        private Button btnLogin;
    }
}