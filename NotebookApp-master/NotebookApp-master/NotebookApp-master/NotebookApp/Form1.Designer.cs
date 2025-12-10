namespace NotebookApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnAdd = new Button();
            Sözler = new ListBox();
            txtNote = new TextBox();
            label1 = new Label();
            label2 = new Label();
            btnEdit = new Button();
            btnDelete = new Button();
            SuspendLayout();
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.Indigo;
            resources.ApplyResources(btnAdd, "btnAdd");
            btnAdd.ForeColor = SystemColors.ButtonHighlight;
            btnAdd.Name = "btnAdd";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // Sözler
            // 
            Sözler.BackColor = Color.LavenderBlush;
            Sözler.FormattingEnabled = true;
            resources.ApplyResources(Sözler, "Sözler");
            Sözler.Name = "Sözler";
            Sözler.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // txtNote
            // 
            resources.ApplyResources(txtNote, "txtNote");
            txtNote.Name = "txtNote";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // btnEdit
            // 
            btnEdit.BackColor = Color.Fuchsia;
            resources.ApplyResources(btnEdit, "btnEdit");
            btnEdit.ForeColor = SystemColors.ButtonHighlight;
            btnEdit.Name = "btnEdit";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.MediumOrchid;
            resources.ApplyResources(btnDelete, "btnDelete");
            btnDelete.ForeColor = SystemColors.ButtonHighlight;
            btnDelete.Name = "btnDelete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtNote);
            Controls.Add(Sözler);
            Controls.Add(btnAdd);
            Name = "Form1";
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAdd;
        private ListBox Sözler;
        private TextBox txtNote;
        private Label label1;
        private Label label2;
        private Button btnEdit;
        private Button btnDelete;
    }
}