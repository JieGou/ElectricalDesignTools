
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSelectProjectDb = new System.Windows.Forms.Button();
            this.btnSelectLibraryDb = new System.Windows.Forms.Button();
            this.btnDteqList = new System.Windows.Forms.Button();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.btnDeleteLoad = new System.Windows.Forms.Button();
            this.btnSaveDteq = new System.Windows.Forms.Button();
            this.btnLoadList = new System.Windows.Forms.Button();
            this.btnAssignLoads = new System.Windows.Forms.Button();
            this.btnAddLoad = new System.Windows.Forms.Button();
            this.btnAddDteq = new System.Windows.Forms.Button();
            this.btnCalculateLoads = new System.Windows.Forms.Button();
            this.btnSaveLoads = new System.Windows.Forms.Button();
            this.btnDeleteDteq = new System.Windows.Forms.Button();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.stsLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stsLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.stsLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnDeleteCable = new System.Windows.Forms.Button();
            this.btnCables = new System.Windows.Forms.Button();
            this.btnAddCable = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnSaveCables = new System.Windows.Forms.Button();
            this.lstDteq = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.lstDteq);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelectProjectDb);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelectLibraryDb);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteCable);
            this.splitContainer1.Panel2.Controls.Add(this.btnCables);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddCable);
            this.splitContainer1.Panel2.Controls.Add(this.button4);
            this.splitContainer1.Panel2.Controls.Add(this.btnSaveCables);
            this.splitContainer1.Panel2.Controls.Add(this.btnDteqList);
            this.splitContainer1.Panel2.Controls.Add(this.dgvMain);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteLoad);
            this.splitContainer1.Panel2.Controls.Add(this.btnSaveDteq);
            this.splitContainer1.Panel2.Controls.Add(this.btnLoadList);
            this.splitContainer1.Panel2.Controls.Add(this.btnAssignLoads);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddLoad);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddDteq);
            this.splitContainer1.Panel2.Controls.Add(this.btnCalculateLoads);
            this.splitContainer1.Panel2.Controls.Add(this.btnSaveLoads);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteDteq);
            this.splitContainer1.Size = new System.Drawing.Size(1834, 845);
            this.splitContainer1.SplitterDistance = 182;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnSelectProjectDb
            // 
            this.btnSelectProjectDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectProjectDb.Location = new System.Drawing.Point(14, 739);
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
            this.btnSelectLibraryDb.Location = new System.Drawing.Point(14, 775);
            this.btnSelectLibraryDb.Name = "btnSelectLibraryDb";
            this.btnSelectLibraryDb.Size = new System.Drawing.Size(150, 30);
            this.btnSelectLibraryDb.TabIndex = 1;
            this.btnSelectLibraryDb.TabStop = false;
            this.btnSelectLibraryDb.Text = "Select Library Database";
            this.btnSelectLibraryDb.UseVisualStyleBackColor = true;
            // 
            // btnDteqList
            // 
            this.btnDteqList.Location = new System.Drawing.Point(39, 174);
            this.btnDteqList.Name = "btnDteqList";
            this.btnDteqList.Size = new System.Drawing.Size(148, 28);
            this.btnDteqList.TabIndex = 0;
            this.btnDteqList.Text = "Distribution Equipment";
            this.btnDteqList.UseVisualStyleBackColor = true;
            this.btnDteqList.Click += new System.EventHandler(this.btnDteqList_Click);
            // 
            // dgvMain
            // 
            this.dgvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvMain.ColumnHeadersHeight = 35;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMain.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvMain.EnableHeadersVisualStyles = false;
            this.dgvMain.Location = new System.Drawing.Point(222, 150);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMain.Size = new System.Drawing.Size(1374, 624);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdMain_CellContentClick);
            // 
            // btnDeleteLoad
            // 
            this.btnDeleteLoad.Location = new System.Drawing.Point(39, 442);
            this.btnDeleteLoad.Name = "btnDeleteLoad";
            this.btnDeleteLoad.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteLoad.TabIndex = 7;
            this.btnDeleteLoad.Text = "Delete Load";
            this.btnDeleteLoad.UseVisualStyleBackColor = true;
            this.btnDeleteLoad.Click += new System.EventHandler(this.btnDeleteLoad_Click);
            // 
            // btnSaveDteq
            // 
            this.btnSaveDteq.Location = new System.Drawing.Point(39, 310);
            this.btnSaveDteq.Name = "btnSaveDteq";
            this.btnSaveDteq.Size = new System.Drawing.Size(148, 28);
            this.btnSaveDteq.TabIndex = 4;
            this.btnSaveDteq.Text = "Save Dist. Equipment";
            this.btnSaveDteq.UseVisualStyleBackColor = true;
            this.btnSaveDteq.Click += new System.EventHandler(this.btnSaveDteq_Click);
            // 
            // btnLoadList
            // 
            this.btnLoadList.Location = new System.Drawing.Point(39, 374);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(148, 28);
            this.btnLoadList.TabIndex = 5;
            this.btnLoadList.Text = "Loads List";
            this.btnLoadList.UseVisualStyleBackColor = true;
            this.btnLoadList.Click += new System.EventHandler(this.btnLoadList_Click);
            // 
            // btnAssignLoads
            // 
            this.btnAssignLoads.Location = new System.Drawing.Point(39, 276);
            this.btnAssignLoads.Name = "btnAssignLoads";
            this.btnAssignLoads.Size = new System.Drawing.Size(148, 28);
            this.btnAssignLoads.TabIndex = 3;
            this.btnAssignLoads.Text = "Assign Loads";
            this.btnAssignLoads.UseVisualStyleBackColor = true;
            this.btnAssignLoads.Click += new System.EventHandler(this.btnAssignLoads_Click);
            // 
            // btnAddLoad
            // 
            this.btnAddLoad.Location = new System.Drawing.Point(39, 408);
            this.btnAddLoad.Name = "btnAddLoad";
            this.btnAddLoad.Size = new System.Drawing.Size(148, 28);
            this.btnAddLoad.TabIndex = 6;
            this.btnAddLoad.Text = "Add Load";
            this.btnAddLoad.UseVisualStyleBackColor = true;
            // 
            // btnAddDteq
            // 
            this.btnAddDteq.Location = new System.Drawing.Point(39, 208);
            this.btnAddDteq.Name = "btnAddDteq";
            this.btnAddDteq.Size = new System.Drawing.Size(148, 28);
            this.btnAddDteq.TabIndex = 1;
            this.btnAddDteq.Text = "Add Equipment";
            this.btnAddDteq.UseVisualStyleBackColor = true;
            // 
            // btnCalculateLoads
            // 
            this.btnCalculateLoads.Location = new System.Drawing.Point(39, 476);
            this.btnCalculateLoads.Name = "btnCalculateLoads";
            this.btnCalculateLoads.Size = new System.Drawing.Size(148, 28);
            this.btnCalculateLoads.TabIndex = 8;
            this.btnCalculateLoads.Text = "Calculate Loads";
            this.btnCalculateLoads.UseVisualStyleBackColor = true;
            this.btnCalculateLoads.Click += new System.EventHandler(this.btnCalculateLoads_Click);
            // 
            // btnSaveLoads
            // 
            this.btnSaveLoads.Location = new System.Drawing.Point(39, 510);
            this.btnSaveLoads.Name = "btnSaveLoads";
            this.btnSaveLoads.Size = new System.Drawing.Size(148, 28);
            this.btnSaveLoads.TabIndex = 3;
            this.btnSaveLoads.Text = "Save Loads";
            this.btnSaveLoads.UseVisualStyleBackColor = true;
            this.btnSaveLoads.Click += new System.EventHandler(this.btnSaveLoads_Click);
            // 
            // btnDeleteDteq
            // 
            this.btnDeleteDteq.Location = new System.Drawing.Point(39, 242);
            this.btnDeleteDteq.Name = "btnDeleteDteq";
            this.btnDeleteDteq.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteDteq.TabIndex = 2;
            this.btnDeleteDteq.Text = "Delete Selected";
            this.btnDeleteDteq.UseVisualStyleBackColor = true;
            // 
            // stsMain
            // 
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stsLabel1,
            this.stsLabel2,
            this.stsLabel3});
            this.stsMain.Location = new System.Drawing.Point(0, 823);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1834, 22);
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
            // btnDeleteCable
            // 
            this.btnDeleteCable.Location = new System.Drawing.Point(39, 648);
            this.btnDeleteCable.Name = "btnDeleteCable";
            this.btnDeleteCable.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteCable.TabIndex = 12;
            this.btnDeleteCable.Text = "Delete Load";
            this.btnDeleteCable.UseVisualStyleBackColor = true;
            // 
            // btnCables
            // 
            this.btnCables.Location = new System.Drawing.Point(39, 580);
            this.btnCables.Name = "btnCables";
            this.btnCables.Size = new System.Drawing.Size(148, 28);
            this.btnCables.TabIndex = 10;
            this.btnCables.Text = "Cable List";
            this.btnCables.UseVisualStyleBackColor = true;
            // 
            // btnAddCable
            // 
            this.btnAddCable.Location = new System.Drawing.Point(39, 614);
            this.btnAddCable.Name = "btnAddCable";
            this.btnAddCable.Size = new System.Drawing.Size(148, 28);
            this.btnAddCable.TabIndex = 11;
            this.btnAddCable.Text = "Add Cable";
            this.btnAddCable.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(39, 682);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(148, 28);
            this.button4.TabIndex = 13;
            this.button4.Text = "Calculate Loads";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // btnSaveCables
            // 
            this.btnSaveCables.Location = new System.Drawing.Point(39, 716);
            this.btnSaveCables.Name = "btnSaveCables";
            this.btnSaveCables.Size = new System.Drawing.Size(148, 28);
            this.btnSaveCables.TabIndex = 9;
            this.btnSaveCables.Text = "Save Cables";
            this.btnSaveCables.UseVisualStyleBackColor = true;
            // 
            // lstDteq
            // 
            this.lstDteq.FormattingEnabled = true;
            this.lstDteq.Location = new System.Drawing.Point(19, 165);
            this.lstDteq.Name = "lstDteq";
            this.lstDteq.Size = new System.Drawing.Size(145, 173);
            this.lstDteq.TabIndex = 2;
            this.lstDteq.SelectedIndexChanged += new System.EventHandler(this.lstDteq_SelectedIndexChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1834, 845);
            this.Controls.Add(this.stsMain);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electrical Design Tools";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSelectProjectDb;
        private System.Windows.Forms.Button btnSelectLibraryDb;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel1;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel2;
        private System.Windows.Forms.Button btnLoadList;
        private System.Windows.Forms.Button btnSaveLoads;
        private System.Windows.Forms.Button btnCalculateLoads;
        private System.Windows.Forms.Button btnSaveDteq;
        private System.Windows.Forms.Button btnAssignLoads;
        private System.Windows.Forms.Button btnDteqList;
        private System.Windows.Forms.Button btnDeleteDteq;
        private System.Windows.Forms.Button btnAddDteq;
        private System.Windows.Forms.Button btnDeleteLoad;
        private System.Windows.Forms.Button btnAddLoad;
        private System.Windows.Forms.ToolStripStatusLabel stsLabel3;
        private System.Windows.Forms.Button btnDeleteCable;
        private System.Windows.Forms.Button btnCables;
        private System.Windows.Forms.Button btnAddCable;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnSaveCables;
        private System.Windows.Forms.ListBox lstDteq;
    }
}

