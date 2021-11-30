namespace WinFormCoreUI {
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
            this.btnSelectProject = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnEquipment = new System.Windows.Forms.Button();
            this.btnMainTest = new System.Windows.Forms.Button();
            this.btnDataTables = new System.Windows.Forms.Button();
            this.btnSelectLibrary = new System.Windows.Forms.Button();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.pnlChildForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChildForm
            // 
            this.pnlChildForm.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlChildForm.Controls.Add(this.pictureBox1);
            this.pnlChildForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChildForm.Location = new System.Drawing.Point(213, 0);
            this.pnlChildForm.Name = "pnlChildForm";
            this.pnlChildForm.Size = new System.Drawing.Size(1571, 811);
            this.pnlChildForm.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(651, 274);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(263, 263);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // btnSelectProject
            // 
            this.btnSelectProject.Location = new System.Drawing.Point(26, 95);
            this.btnSelectProject.Name = "btnSelectProject";
            this.btnSelectProject.Size = new System.Drawing.Size(160, 32);
            this.btnSelectProject.TabIndex = 0;
            this.btnSelectProject.Text = "Select Project";
            this.btnSelectProject.UseVisualStyleBackColor = true;
            this.btnSelectProject.Click += new System.EventHandler(this.btnSelectProject_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(26, 133);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(160, 32);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "Project Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnEquipment
            // 
            this.btnEquipment.Location = new System.Drawing.Point(26, 213);
            this.btnEquipment.Name = "btnEquipment";
            this.btnEquipment.Size = new System.Drawing.Size(160, 32);
            this.btnEquipment.TabIndex = 2;
            this.btnEquipment.Text = "Equipment and Cables";
            this.btnEquipment.UseVisualStyleBackColor = true;
            this.btnEquipment.Click += new System.EventHandler(this.btnEquipment_Click);
            // 
            // btnMainTest
            // 
            this.btnMainTest.Location = new System.Drawing.Point(26, 39);
            this.btnMainTest.Name = "btnMainTest";
            this.btnMainTest.Size = new System.Drawing.Size(160, 32);
            this.btnMainTest.TabIndex = 3;
            this.btnMainTest.Text = "Test";
            this.btnMainTest.UseVisualStyleBackColor = true;
            this.btnMainTest.Click += new System.EventHandler(this.btnMainTest_Click);
            // 
            // btnDataTables
            // 
            this.btnDataTables.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnDataTables.Location = new System.Drawing.Point(26, 711);
            this.btnDataTables.Name = "btnDataTables";
            this.btnDataTables.Size = new System.Drawing.Size(160, 32);
            this.btnDataTables.TabIndex = 4;
            this.btnDataTables.Text = "Data Tables";
            this.btnDataTables.UseVisualStyleBackColor = true;
            // 
            // btnSelectLibrary
            // 
            this.btnSelectLibrary.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSelectLibrary.Location = new System.Drawing.Point(26, 751);
            this.btnSelectLibrary.Name = "btnSelectLibrary";
            this.btnSelectLibrary.Size = new System.Drawing.Size(160, 32);
            this.btnSelectLibrary.TabIndex = 5;
            this.btnSelectLibrary.Text = "Select Library";
            this.btnSelectLibrary.UseVisualStyleBackColor = true;
            this.btnSelectLibrary.Click += new System.EventHandler(this.btnSelectLibrary_Click);
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlMenu.Controls.Add(this.btnSelectLibrary);
            this.pnlMenu.Controls.Add(this.btnDataTables);
            this.pnlMenu.Controls.Add(this.btnMainTest);
            this.pnlMenu.Controls.Add(this.btnEquipment);
            this.pnlMenu.Controls.Add(this.btnSettings);
            this.pnlMenu.Controls.Add(this.btnSelectProject);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(213, 811);
            this.pnlMenu.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 811);
            this.Controls.Add(this.pnlChildForm);
            this.Controls.Add(this.pnlMenu);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pnlChildForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private Panel pnlChildForm;
        private PictureBox pictureBox1;
        private Button btnSelectProject;
        private Button btnSettings;
        private Button btnEquipment;
        private Button btnMainTest;
        private Button btnDataTables;
        private Button btnSelectLibrary;
        private Panel pnlMenu;
    }
}