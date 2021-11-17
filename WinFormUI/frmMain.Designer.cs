
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.stsLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stsLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stsLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lstDteq = new System.Windows.Forms.ListBox();
            this.btnSelectLibraryDb = new System.Windows.Forms.Button();
            this.btnSelectProjectDb = new System.Windows.Forms.Button();
            this.btnDeleteDteq = new System.Windows.Forms.Button();
            this.btnSaveLoads = new System.Windows.Forms.Button();
            this.btnCalculateLoads = new System.Windows.Forms.Button();
            this.btnAddDteq = new System.Windows.Forms.Button();
            this.btnAddLoad = new System.Windows.Forms.Button();
            this.btnLoadList = new System.Windows.Forms.Button();
            this.btnSaveDteq = new System.Windows.Forms.Button();
            this.btnDeleteLoad = new System.Windows.Forms.Button();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.btnDteqList = new System.Windows.Forms.Button();
            this.btnSaveCables = new System.Windows.Forms.Button();
            this.btnAddCable = new System.Windows.Forms.Button();
            this.btnCables = new System.Windows.Forms.Button();
            this.btnDeleteCable = new System.Windows.Forms.Button();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.btnCreateCableList = new System.Windows.Forms.Button();
            this.pnlChildForm = new System.Windows.Forms.Panel();
            this.stsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.pnlChildForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // stsMain
            // 
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stsLabel1,
            this.stsLabel2,
            this.stsLabel3});
            this.stsMain.Location = new System.Drawing.Point(0, 797);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1725, 22);
            this.stsMain.TabIndex = 1;
            this.stsMain.Text = "statusStrip1";
            // 
            // stsLabel1
            // 
            this.stsLabel1.Name = "stsLabel1";
            this.stsLabel1.Size = new System.Drawing.Size(62, 17);
            this.stsLabel1.Text = "Project Db";
            // 
            // stsLabel2
            // 
            this.stsLabel2.Name = "stsLabel2";
            this.stsLabel2.Size = new System.Drawing.Size(61, 17);
            this.stsLabel2.Text = "Library Db";
            // 
            // stsLabel3
            // 
            this.stsLabel3.Name = "stsLabel3";
            this.stsLabel3.Size = new System.Drawing.Size(78, 17);
            this.stsLabel3.Text = "Selected Item";
            // 
            // lstDteq
            // 
            this.lstDteq.FormattingEnabled = true;
            this.lstDteq.Location = new System.Drawing.Point(20, 254);
            this.lstDteq.Name = "lstDteq";
            this.lstDteq.Size = new System.Drawing.Size(150, 108);
            this.lstDteq.TabIndex = 2;
            this.lstDteq.SelectedIndexChanged += new System.EventHandler(this.lstDteq_SelectedIndexChanged);
            // 
            // btnSelectLibraryDb
            // 
            this.btnSelectLibraryDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectLibraryDb.Location = new System.Drawing.Point(20, 753);
            this.btnSelectLibraryDb.Name = "btnSelectLibraryDb";
            this.btnSelectLibraryDb.Size = new System.Drawing.Size(150, 30);
            this.btnSelectLibraryDb.TabIndex = 1;
            this.btnSelectLibraryDb.TabStop = false;
            this.btnSelectLibraryDb.Text = "Select Library Database";
            this.btnSelectLibraryDb.UseVisualStyleBackColor = true;
            // 
            // btnSelectProjectDb
            // 
            this.btnSelectProjectDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectProjectDb.Location = new System.Drawing.Point(20, 717);
            this.btnSelectProjectDb.Name = "btnSelectProjectDb";
            this.btnSelectProjectDb.Size = new System.Drawing.Size(150, 30);
            this.btnSelectProjectDb.TabIndex = 0;
            this.btnSelectProjectDb.TabStop = false;
            this.btnSelectProjectDb.Text = "Select Project Database";
            this.btnSelectProjectDb.UseVisualStyleBackColor = true;
            this.btnSelectProjectDb.Click += new System.EventHandler(this.btnSelectProjectDb_Click);
            // 
            // btnDeleteDteq
            // 
            this.btnDeleteDteq.Location = new System.Drawing.Point(33, 237);
            this.btnDeleteDteq.Name = "btnDeleteDteq";
            this.btnDeleteDteq.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteDteq.TabIndex = 2;
            this.btnDeleteDteq.Text = "Delete Selected";
            this.btnDeleteDteq.UseVisualStyleBackColor = true;
            // 
            // btnSaveLoads
            // 
            this.btnSaveLoads.Location = new System.Drawing.Point(33, 467);
            this.btnSaveLoads.Name = "btnSaveLoads";
            this.btnSaveLoads.Size = new System.Drawing.Size(148, 28);
            this.btnSaveLoads.TabIndex = 3;
            this.btnSaveLoads.Text = "Save Loads";
            this.btnSaveLoads.UseVisualStyleBackColor = true;
            this.btnSaveLoads.Click += new System.EventHandler(this.btnSaveLoads_Click);
            // 
            // btnCalculateLoads
            // 
            this.btnCalculateLoads.Location = new System.Drawing.Point(33, 433);
            this.btnCalculateLoads.Name = "btnCalculateLoads";
            this.btnCalculateLoads.Size = new System.Drawing.Size(148, 28);
            this.btnCalculateLoads.TabIndex = 8;
            this.btnCalculateLoads.Text = "Calculate Loads";
            this.btnCalculateLoads.UseVisualStyleBackColor = true;
            this.btnCalculateLoads.Click += new System.EventHandler(this.btnCalculateLoads_Click);
            // 
            // btnAddDteq
            // 
            this.btnAddDteq.Location = new System.Drawing.Point(33, 203);
            this.btnAddDteq.Name = "btnAddDteq";
            this.btnAddDteq.Size = new System.Drawing.Size(148, 28);
            this.btnAddDteq.TabIndex = 1;
            this.btnAddDteq.Text = "Add Equipment";
            this.btnAddDteq.UseVisualStyleBackColor = true;
            // 
            // btnAddLoad
            // 
            this.btnAddLoad.Location = new System.Drawing.Point(33, 365);
            this.btnAddLoad.Name = "btnAddLoad";
            this.btnAddLoad.Size = new System.Drawing.Size(148, 28);
            this.btnAddLoad.TabIndex = 6;
            this.btnAddLoad.Text = "Add Load";
            this.btnAddLoad.UseVisualStyleBackColor = true;
            // 
            // btnLoadList
            // 
            this.btnLoadList.Location = new System.Drawing.Point(33, 331);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(148, 28);
            this.btnLoadList.TabIndex = 5;
            this.btnLoadList.Text = "Loads List";
            this.btnLoadList.UseVisualStyleBackColor = true;
            this.btnLoadList.Click += new System.EventHandler(this.btnLoadList_Click);
            // 
            // btnSaveDteq
            // 
            this.btnSaveDteq.Location = new System.Drawing.Point(33, 271);
            this.btnSaveDteq.Name = "btnSaveDteq";
            this.btnSaveDteq.Size = new System.Drawing.Size(148, 28);
            this.btnSaveDteq.TabIndex = 4;
            this.btnSaveDteq.Text = "Save Dist. Equipment";
            this.btnSaveDteq.UseVisualStyleBackColor = true;
            this.btnSaveDteq.Click += new System.EventHandler(this.btnSaveDteq_Click);
            // 
            // btnDeleteLoad
            // 
            this.btnDeleteLoad.Location = new System.Drawing.Point(33, 399);
            this.btnDeleteLoad.Name = "btnDeleteLoad";
            this.btnDeleteLoad.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteLoad.TabIndex = 7;
            this.btnDeleteLoad.Text = "Delete Load";
            this.btnDeleteLoad.UseVisualStyleBackColor = true;
            this.btnDeleteLoad.Click += new System.EventHandler(this.btnDeleteLoad_Click);
            // 
            // dgvMain
            // 
            this.dgvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMain.ColumnHeadersHeight = 35;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMain.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMain.EnableHeadersVisualStyles = false;
            this.dgvMain.Location = new System.Drawing.Point(215, 119);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMain.Size = new System.Drawing.Size(1255, 650);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdMain_CellContentClick);
            // 
            // btnDteqList
            // 
            this.btnDteqList.Location = new System.Drawing.Point(33, 169);
            this.btnDteqList.Name = "btnDteqList";
            this.btnDteqList.Size = new System.Drawing.Size(148, 28);
            this.btnDteqList.TabIndex = 0;
            this.btnDteqList.Text = "Distribution Equipment";
            this.btnDteqList.UseVisualStyleBackColor = true;
            this.btnDteqList.Click += new System.EventHandler(this.btnDteqList_Click);
            // 
            // btnSaveCables
            // 
            this.btnSaveCables.Location = new System.Drawing.Point(33, 631);
            this.btnSaveCables.Name = "btnSaveCables";
            this.btnSaveCables.Size = new System.Drawing.Size(148, 28);
            this.btnSaveCables.TabIndex = 9;
            this.btnSaveCables.Text = "Save Cables";
            this.btnSaveCables.UseVisualStyleBackColor = true;
            this.btnSaveCables.Click += new System.EventHandler(this.btnSaveCables_Click);
            // 
            // btnAddCable
            // 
            this.btnAddCable.Location = new System.Drawing.Point(33, 563);
            this.btnAddCable.Name = "btnAddCable";
            this.btnAddCable.Size = new System.Drawing.Size(148, 28);
            this.btnAddCable.TabIndex = 11;
            this.btnAddCable.Text = "Add Cable";
            this.btnAddCable.UseVisualStyleBackColor = true;
            // 
            // btnCables
            // 
            this.btnCables.Location = new System.Drawing.Point(33, 529);
            this.btnCables.Name = "btnCables";
            this.btnCables.Size = new System.Drawing.Size(148, 28);
            this.btnCables.TabIndex = 10;
            this.btnCables.Text = "Cable List";
            this.btnCables.UseVisualStyleBackColor = true;
            this.btnCables.Click += new System.EventHandler(this.btnCables_Click);
            // 
            // btnDeleteCable
            // 
            this.btnDeleteCable.Location = new System.Drawing.Point(33, 597);
            this.btnDeleteCable.Name = "btnDeleteCable";
            this.btnDeleteCable.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteCable.TabIndex = 12;
            this.btnDeleteCable.Text = "Delete Cable";
            this.btnDeleteCable.UseVisualStyleBackColor = true;
            // 
            // pnlMenu
            // 
            this.pnlMenu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlMenu.Controls.Add(this.lstDteq);
            this.pnlMenu.Controls.Add(this.btnSelectLibraryDb);
            this.pnlMenu.Controls.Add(this.btnSelectProjectDb);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(196, 797);
            this.pnlMenu.TabIndex = 14;
            // 
            // btnCreateCableList
            // 
            this.btnCreateCableList.Location = new System.Drawing.Point(45, 738);
            this.btnCreateCableList.Name = "btnCreateCableList";
            this.btnCreateCableList.Size = new System.Drawing.Size(148, 28);
            this.btnCreateCableList.TabIndex = 15;
            this.btnCreateCableList.Text = "Create Cable List";
            this.btnCreateCableList.UseVisualStyleBackColor = true;
            this.btnCreateCableList.Click += new System.EventHandler(this.btnCreateCableList_Click);
            // 
            // pnlChildForm
            // 
            this.pnlChildForm.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlChildForm.Controls.Add(this.btnDteqList);
            this.pnlChildForm.Controls.Add(this.btnCreateCableList);
            this.pnlChildForm.Controls.Add(this.btnAddLoad);
            this.pnlChildForm.Controls.Add(this.btnAddDteq);
            this.pnlChildForm.Controls.Add(this.btnDeleteCable);
            this.pnlChildForm.Controls.Add(this.btnLoadList);
            this.pnlChildForm.Controls.Add(this.btnCables);
            this.pnlChildForm.Controls.Add(this.btnCalculateLoads);
            this.pnlChildForm.Controls.Add(this.btnAddCable);
            this.pnlChildForm.Controls.Add(this.btnSaveDteq);
            this.pnlChildForm.Controls.Add(this.btnSaveLoads);
            this.pnlChildForm.Controls.Add(this.btnSaveCables);
            this.pnlChildForm.Controls.Add(this.btnDeleteLoad);
            this.pnlChildForm.Controls.Add(this.btnDeleteDteq);
            this.pnlChildForm.Controls.Add(this.dgvMain);
            this.pnlChildForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChildForm.Location = new System.Drawing.Point(196, 0);
            this.pnlChildForm.Name = "pnlChildForm";
            this.pnlChildForm.Size = new System.Drawing.Size(1529, 797);
            this.pnlChildForm.TabIndex = 16;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1725, 819);
            this.Controls.Add(this.pnlChildForm);
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.stsMain);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electrical Design Tools";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.pnlChildForm.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel1;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel2;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel3;
        private System.Windows.Forms.ListBox lstDteq;
        private System.Windows.Forms.Button btnSelectProjectDb;
        private System.Windows.Forms.Button btnSelectLibraryDb;
        private System.Windows.Forms.Button btnDeleteCable;
        private System.Windows.Forms.Button btnCables;
        private System.Windows.Forms.Button btnAddCable;
        private System.Windows.Forms.Button btnSaveCables;
        private System.Windows.Forms.Button btnDteqList;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.Button btnDeleteLoad;
        private System.Windows.Forms.Button btnSaveDteq;
        private System.Windows.Forms.Button btnLoadList;
        private System.Windows.Forms.Button btnAddLoad;
        private System.Windows.Forms.Button btnAddDteq;
        private System.Windows.Forms.Button btnCalculateLoads;
        private System.Windows.Forms.Button btnSaveLoads;
        private System.Windows.Forms.Button btnDeleteDteq;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btnCreateCableList;
        private System.Windows.Forms.Panel pnlChildForm;
    }
}

