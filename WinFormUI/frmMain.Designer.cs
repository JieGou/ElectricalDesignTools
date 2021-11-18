
namespace WinFormUI {
    partial class frmMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlChildForm = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnDistribution = new System.Windows.Forms.Button();
            this.btnCableSettings = new System.Windows.Forms.Button();
            this.btnSelectProjectDb = new System.Windows.Forms.Button();
            this.btnSelectLibraryDb = new System.Windows.Forms.Button();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.btnDataTables = new System.Windows.Forms.Button();
            this.btnCostSettings = new System.Windows.Forms.Button();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.pnlChildForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChildForm
            // 
            this.pnlChildForm.BackColor = System.Drawing.SystemColors.Control;
            this.pnlChildForm.Controls.Add(this.pictureBox1);
            this.pnlChildForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChildForm.Location = new System.Drawing.Point(196, 0);
            this.pnlChildForm.Name = "pnlChildForm";
            this.pnlChildForm.Size = new System.Drawing.Size(1590, 853);
            this.pnlChildForm.TabIndex = 18;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(592, 285);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(269, 263);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // btnDistribution
            // 
            this.btnDistribution.Location = new System.Drawing.Point(22, 203);
            this.btnDistribution.Name = "btnDistribution";
            this.btnDistribution.Size = new System.Drawing.Size(148, 28);
            this.btnDistribution.TabIndex = 0;
            this.btnDistribution.Text = "Distribution and Loads";
            this.btnDistribution.UseVisualStyleBackColor = true;
            this.btnDistribution.Click += new System.EventHandler(this.btnDistribution_Click);
            // 
            // btnCableSettings
            // 
            this.btnCableSettings.Location = new System.Drawing.Point(22, 237);
            this.btnCableSettings.Name = "btnCableSettings";
            this.btnCableSettings.Size = new System.Drawing.Size(148, 28);
            this.btnCableSettings.TabIndex = 10;
            this.btnCableSettings.Text = "Cable Settings";
            this.btnCableSettings.UseVisualStyleBackColor = true;
            this.btnCableSettings.Click += new System.EventHandler(this.btnCableSettings_Click);
            // 
            // btnSelectProjectDb
            // 
            this.btnSelectProjectDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectProjectDb.Location = new System.Drawing.Point(20, 773);
            this.btnSelectProjectDb.Name = "btnSelectProjectDb";
            this.btnSelectProjectDb.Size = new System.Drawing.Size(150, 30);
            this.btnSelectProjectDb.TabIndex = 0;
            this.btnSelectProjectDb.TabStop = false;
            this.btnSelectProjectDb.Text = "Select Project Database";
            this.btnSelectProjectDb.UseVisualStyleBackColor = true;
            this.btnSelectProjectDb.Click += new System.EventHandler(this.btnSelectProjectDb_Click);
            // 
            // btnSelectLibraryDb
            // 
            this.btnSelectLibraryDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectLibraryDb.Location = new System.Drawing.Point(20, 809);
            this.btnSelectLibraryDb.Name = "btnSelectLibraryDb";
            this.btnSelectLibraryDb.Size = new System.Drawing.Size(150, 30);
            this.btnSelectLibraryDb.TabIndex = 1;
            this.btnSelectLibraryDb.TabStop = false;
            this.btnSelectLibraryDb.Text = "Select Library Database";
            this.btnSelectLibraryDb.UseVisualStyleBackColor = true;
            this.btnSelectLibraryDb.Click += new System.EventHandler(this.btnSelectLibraryDb_Click);
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlMenu.Controls.Add(this.btnDataTables);
            this.pnlMenu.Controls.Add(this.btnCostSettings);
            this.pnlMenu.Controls.Add(this.btnDistribution);
            this.pnlMenu.Controls.Add(this.btnSelectLibraryDb);
            this.pnlMenu.Controls.Add(this.btnSelectProjectDb);
            this.pnlMenu.Controls.Add(this.btnCableSettings);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(196, 853);
            this.pnlMenu.TabIndex = 17;
            // 
            // btnDataTables
            // 
            this.btnDataTables.Location = new System.Drawing.Point(24, 412);
            this.btnDataTables.Name = "btnDataTables";
            this.btnDataTables.Size = new System.Drawing.Size(148, 28);
            this.btnDataTables.TabIndex = 12;
            this.btnDataTables.Text = "Data Tables";
            this.btnDataTables.UseVisualStyleBackColor = true;
            this.btnDataTables.Click += new System.EventHandler(this.btnDataTables_Click);
            // 
            // btnCostSettings
            // 
            this.btnCostSettings.Location = new System.Drawing.Point(22, 271);
            this.btnCostSettings.Name = "btnCostSettings";
            this.btnCostSettings.Size = new System.Drawing.Size(148, 28);
            this.btnCostSettings.TabIndex = 11;
            this.btnCostSettings.Text = "Cost Settings";
            this.btnCostSettings.UseVisualStyleBackColor = true;
            // 
            // stsMain
            // 
            this.stsMain.Location = new System.Drawing.Point(196, 831);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1590, 22);
            this.stsMain.TabIndex = 19;
            this.stsMain.Text = "statusStrip1";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1786, 853);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.pnlChildForm);
            this.Controls.Add(this.pnlMenu);
            this.Name = "frmMain";
            this.Text = "Electrical Design Tool";
            this.pnlChildForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlChildForm;
        private System.Windows.Forms.Button btnDistribution;
        private System.Windows.Forms.Button btnCableSettings;
        private System.Windows.Forms.Button btnSelectProjectDb;
        private System.Windows.Forms.Button btnSelectLibraryDb;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btnCostSettings;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.Button btnDataTables;
    }
}