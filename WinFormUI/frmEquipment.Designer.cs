﻿
namespace WinFormUI {
    partial class frmEquipment {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lstDteq = new System.Windows.Forms.ListBox();
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
            this.pnlQuickButtons = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.lblAssignedLoads = new System.Windows.Forms.Label();
            this.lblListName = new System.Windows.Forms.Label();
            this.lblSelectedTag = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.pnlQuickButtons.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstDteq
            // 
            this.lstDteq.FormattingEnabled = true;
            this.lstDteq.Location = new System.Drawing.Point(23, 304);
            this.lstDteq.Name = "lstDteq";
            this.lstDteq.Size = new System.Drawing.Size(148, 108);
            this.lstDteq.TabIndex = 2;
            this.lstDteq.SelectedIndexChanged += new System.EventHandler(this.lstDteq_SelectedIndexChanged);
            // 
            // btnDeleteDteq
            // 
            this.btnDeleteDteq.Location = new System.Drawing.Point(23, 428);
            this.btnDeleteDteq.Name = "btnDeleteDteq";
            this.btnDeleteDteq.Size = new System.Drawing.Size(148, 28);
            this.btnDeleteDteq.TabIndex = 2;
            this.btnDeleteDteq.Text = "Delete Selected";
            this.btnDeleteDteq.UseVisualStyleBackColor = true;
            // 
            // btnSaveLoads
            // 
            this.btnSaveLoads.Location = new System.Drawing.Point(23, 721);
            this.btnSaveLoads.Name = "btnSaveLoads";
            this.btnSaveLoads.Size = new System.Drawing.Size(148, 28);
            this.btnSaveLoads.TabIndex = 3;
            this.btnSaveLoads.Text = "Save Loads";
            this.btnSaveLoads.UseVisualStyleBackColor = true;
            this.btnSaveLoads.Click += new System.EventHandler(this.btnSaveLoads_Click);
            // 
            // btnCalculateLoads
            // 
            this.btnCalculateLoads.Location = new System.Drawing.Point(23, 687);
            this.btnCalculateLoads.Name = "btnCalculateLoads";
            this.btnCalculateLoads.Size = new System.Drawing.Size(148, 28);
            this.btnCalculateLoads.TabIndex = 8;
            this.btnCalculateLoads.Text = "Calculate Loads";
            this.btnCalculateLoads.UseVisualStyleBackColor = true;
            this.btnCalculateLoads.Click += new System.EventHandler(this.btnCalculateLoads_Click);
            // 
            // btnAddDteq
            // 
            this.btnAddDteq.Location = new System.Drawing.Point(23, 232);
            this.btnAddDteq.Name = "btnAddDteq";
            this.btnAddDteq.Size = new System.Drawing.Size(148, 28);
            this.btnAddDteq.TabIndex = 1;
            this.btnAddDteq.Text = "Add Equipment";
            this.btnAddDteq.UseVisualStyleBackColor = true;
            // 
            // btnAddLoad
            // 
            this.btnAddLoad.Location = new System.Drawing.Point(23, 619);
            this.btnAddLoad.Name = "btnAddLoad";
            this.btnAddLoad.Size = new System.Drawing.Size(148, 28);
            this.btnAddLoad.TabIndex = 6;
            this.btnAddLoad.Text = "Add Load";
            this.btnAddLoad.UseVisualStyleBackColor = true;
            // 
            // btnLoadList
            // 
            this.btnLoadList.Location = new System.Drawing.Point(23, 585);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(148, 28);
            this.btnLoadList.TabIndex = 5;
            this.btnLoadList.Text = "Loads List";
            this.btnLoadList.UseVisualStyleBackColor = true;
            this.btnLoadList.Click += new System.EventHandler(this.btnLoadList_Click);
            // 
            // btnSaveDteq
            // 
            this.btnSaveDteq.Location = new System.Drawing.Point(23, 462);
            this.btnSaveDteq.Name = "btnSaveDteq";
            this.btnSaveDteq.Size = new System.Drawing.Size(148, 28);
            this.btnSaveDteq.TabIndex = 4;
            this.btnSaveDteq.Text = "Save Dist. Equipment";
            this.btnSaveDteq.UseVisualStyleBackColor = true;
            this.btnSaveDteq.Click += new System.EventHandler(this.btnSaveDteq_Click);
            // 
            // btnDeleteLoad
            // 
            this.btnDeleteLoad.Location = new System.Drawing.Point(23, 653);
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
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMain.ColumnHeadersHeight = 35;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMain.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMain.EnableHeadersVisualStyles = false;
            this.dgvMain.Location = new System.Drawing.Point(16, 188);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMain.Size = new System.Drawing.Size(1588, 637);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMain_CellMouseDown);
            this.dgvMain.MouseLeave += new System.EventHandler(this.dgvMain_MouseLeave);
            // 
            // btnDteqList
            // 
            this.btnDteqList.Location = new System.Drawing.Point(23, 198);
            this.btnDteqList.Name = "btnDteqList";
            this.btnDteqList.Size = new System.Drawing.Size(148, 28);
            this.btnDteqList.TabIndex = 0;
            this.btnDteqList.Text = "Distribution Equipment";
            this.btnDteqList.UseVisualStyleBackColor = true;
            this.btnDteqList.Click += new System.EventHandler(this.btnDteqList_Click);
            // 
            // pnlQuickButtons
            // 
            this.pnlQuickButtons.Controls.Add(this.lblAssignedLoads);
            this.pnlQuickButtons.Controls.Add(this.lstDteq);
            this.pnlQuickButtons.Controls.Add(this.btnDteqList);
            this.pnlQuickButtons.Controls.Add(this.btnCalculateLoads);
            this.pnlQuickButtons.Controls.Add(this.btnLoadList);
            this.pnlQuickButtons.Controls.Add(this.btnDeleteDteq);
            this.pnlQuickButtons.Controls.Add(this.btnSaveDteq);
            this.pnlQuickButtons.Controls.Add(this.btnAddLoad);
            this.pnlQuickButtons.Controls.Add(this.btnSaveLoads);
            this.pnlQuickButtons.Controls.Add(this.btnDeleteLoad);
            this.pnlQuickButtons.Controls.Add(this.btnAddDteq);
            this.pnlQuickButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlQuickButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQuickButtons.Name = "pnlQuickButtons";
            this.pnlQuickButtons.Size = new System.Drawing.Size(192, 858);
            this.pnlQuickButtons.TabIndex = 16;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Location = new System.Drawing.Point(307, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1553, 59);
            this.pnlHeader.TabIndex = 17;
            // 
            // pnlFooter
            // 
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(192, 848);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1668, 10);
            this.pnlFooter.TabIndex = 18;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.lblSelectedTag);
            this.pnlCenter.Controls.Add(this.lblListName);
            this.pnlCenter.Controls.Add(this.dgvMain);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(192, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1668, 848);
            this.pnlCenter.TabIndex = 19;
            this.pnlCenter.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lblAssignedLoads
            // 
            this.lblAssignedLoads.AutoSize = true;
            this.lblAssignedLoads.Location = new System.Drawing.Point(20, 288);
            this.lblAssignedLoads.Name = "lblAssignedLoads";
            this.lblAssignedLoads.Size = new System.Drawing.Size(81, 13);
            this.lblAssignedLoads.TabIndex = 9;
            this.lblAssignedLoads.Text = "Show Loading";
            // 
            // lblListName
            // 
            this.lblListName.AutoSize = true;
            this.lblListName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListName.Location = new System.Drawing.Point(6, 150);
            this.lblListName.Name = "lblListName";
            this.lblListName.Size = new System.Drawing.Size(86, 21);
            this.lblListName.TabIndex = 10;
            this.lblListName.Text = "List Name";
            this.lblListName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblSelectedTag
            // 
            this.lblSelectedTag.AutoSize = true;
            this.lblSelectedTag.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedTag.Location = new System.Drawing.Point(6, 118);
            this.lblSelectedTag.Name = "lblSelectedTag";
            this.lblSelectedTag.Size = new System.Drawing.Size(37, 21);
            this.lblSelectedTag.TabIndex = 11;
            this.lblSelectedTag.Text = "Tag";
            this.lblSelectedTag.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // frmEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1860, 858);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlQuickButtons);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmEquipment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electrical Design Tools";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.pnlQuickButtons.ResumeLayout(false);
            this.pnlQuickButtons.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCenter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox lstDteq;
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
        private System.Windows.Forms.Panel pnlQuickButtons;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.Label lblAssignedLoads;
        private System.Windows.Forms.Label lblListName;
        private System.Windows.Forms.Label lblSelectedTag;
    }
}

