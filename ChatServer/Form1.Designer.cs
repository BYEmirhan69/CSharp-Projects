using System;

namespace Chat_Desktop1
{
    partial class FrmServer
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
            this.lblPort = new DevExpress.XtraEditors.LabelControl();
            this.textPort = new DevExpress.XtraEditors.TextEdit();
            this.btnStart = new DevExpress.XtraEditors.SimpleButton();
            this.btnStop = new DevExpress.XtraEditors.SimpleButton();
            this.lblDurum = new DevExpress.XtraEditors.LabelControl();
            this.txtLog = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.textPort.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPort
            // 
            this.lblPort.Location = new System.Drawing.Point(12, 12);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(36, 16);
            this.lblPort.TabIndex = 1;
            this.lblPort.Text = "Port : ";
            this.lblPort.Click += new System.EventHandler(this.lblPort_Click);
            // 
            // textPort
            // 
            this.textPort.EditValue = "8888";
            this.textPort.Location = new System.Drawing.Point(81, 12);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(125, 22);
            this.textPort.TabIndex = 2;
            this.textPort.EditValueChanged += new System.EventHandler(this.textPort_EditValueChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(248, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(94, 29);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start Server";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(382, 8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 29);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop Server";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblDurum
            // 
            this.lblDurum.Location = new System.Drawing.Point(13, 55);
            this.lblDurum.Name = "lblDurum";
            this.lblDurum.Size = new System.Drawing.Size(96, 16);
            this.lblDurum.TabIndex = 6;
            this.lblDurum.Text = "Status : Stopped";
            this.lblDurum.Click += new System.EventHandler(this.lblDurum_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(-3, 101);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(602, 414);
            this.txtLog.TabIndex = 7;
            this.txtLog.TabStop = false;
            this.txtLog.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // FrmServer
            // 
            this.Appearance.ForeColor = System.Drawing.Color.Black;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1424, 591);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblDurum);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.textPort);
            this.Controls.Add(this.lblPort);
            this.IconOptions.SvgImage = global::Chat_Desktop1.Properties.Resources.mr;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmServer";
            this.Text = "Chat Server";
            this.Load += new System.EventHandler(this.Frm1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textPort.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lblDurum_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void textPort_EditValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void lblPort_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblPort;
        private DevExpress.XtraEditors.TextEdit textPort;
        private DevExpress.XtraEditors.SimpleButton btnStart;
        private DevExpress.XtraEditors.SimpleButton btnStop;
        private DevExpress.XtraEditors.LabelControl lblDurum;
        private System.Windows.Forms.GroupBox txtLog;
    }
}

