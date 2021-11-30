namespace WinFormCoreUI {
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
            this.pnlQuickButtons = new System.Windows.Forms.Panel();
            this.lstDteq = new System.Windows.Forms.ListBox();
            this.btnAddDteqEq = new System.Windows.Forms.Button();
            this.btnDistributionEquipment = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.dgvEquipment = new System.Windows.Forms.DataGridView();
            this.btnSaveDteq = new System.Windows.Forms.Button();
            this.btnLoadList = new System.Windows.Forms.Button();
            this.btnCalculateLoads = new System.Windows.Forms.Button();
            this.btnDeleteDteq = new System.Windows.Forms.Button();
            this.btnAddLoad = new System.Windows.Forms.Button();
            this.btnDeleteLoad = new System.Windows.Forms.Button();
            this.btnSaveLoads = new System.Windows.Forms.Button();
            this.btnCreateCableList = new System.Windows.Forms.Button();
            this.lblAssignedLoads = new System.Windows.Forms.Label();
            this.lblSelectedTag = new System.Windows.Forms.Label();
            this.lblListName = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pnlQuickButtons.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlQuickButtons
            // 
            this.pnlQuickButtons.Controls.Add(this.button1);
            this.pnlQuickButtons.Controls.Add(this.lblAssignedLoads);
            this.pnlQuickButtons.Controls.Add(this.btnCreateCableList);
            this.pnlQuickButtons.Controls.Add(this.btnSaveLoads);
            this.pnlQuickButtons.Controls.Add(this.btnDeleteLoad);
            this.pnlQuickButtons.Controls.Add(this.btnAddLoad);
            this.pnlQuickButtons.Controls.Add(this.btnDeleteDteq);
            this.pnlQuickButtons.Controls.Add(this.btnCalculateLoads);
            this.pnlQuickButtons.Controls.Add(this.btnLoadList);
            this.pnlQuickButtons.Controls.Add(this.btnSaveDteq);
            this.pnlQuickButtons.Controls.Add(this.lstDteq);
            this.pnlQuickButtons.Controls.Add(this.btnAddDteqEq);
            this.pnlQuickButtons.Controls.Add(this.btnDistributionEquipment);
            this.pnlQuickButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlQuickButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQuickButtons.Name = "pnlQuickButtons";
            this.pnlQuickButtons.Size = new System.Drawing.Size(204, 861);
            this.pnlQuickButtons.TabIndex = 0;
            this.pnlQuickButtons.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlQuickButtons_Paint);
            // 
            // lstDteq
            // 
            this.lstDteq.FormattingEnabled = true;
            this.lstDteq.ItemHeight = 15;
            this.lstDteq.Location = new System.Drawing.Point(31, 207);
            this.lstDteq.Name = "lstDteq";
            this.lstDteq.Size = new System.Drawing.Size(150, 94);
            this.lstDteq.TabIndex = 2;
            // 
            // btnAddDteqEq
            // 
            this.btnAddDteqEq.Location = new System.Drawing.Point(31, 138);
            this.btnAddDteqEq.Name = "btnAddDteqEq";
            this.btnAddDteqEq.Size = new System.Drawing.Size(150, 30);
            this.btnAddDteqEq.TabIndex = 1;
            this.btnAddDteqEq.Text = "Add Equipment";
            this.btnAddDteqEq.UseVisualStyleBackColor = true;
            // 
            // btnDistributionEquipment
            // 
            this.btnDistributionEquipment.Location = new System.Drawing.Point(31, 100);
            this.btnDistributionEquipment.Name = "btnDistributionEquipment";
            this.btnDistributionEquipment.Size = new System.Drawing.Size(150, 30);
            this.btnDistributionEquipment.TabIndex = 0;
            this.btnDistributionEquipment.Text = "Distribution Equipment";
            this.btnDistributionEquipment.UseVisualStyleBackColor = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(204, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1580, 100);
            this.pnlHeader.TabIndex = 1;
            // 
            // pnlFooter
            // 
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(204, 769);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1580, 92);
            this.pnlFooter.TabIndex = 2;
            // 
            // pnlRight
            // 
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(1757, 100);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(27, 669);
            this.pnlRight.TabIndex = 3;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.lblSelectedTag);
            this.pnlCenter.Controls.Add(this.lblListName);
            this.pnlCenter.Controls.Add(this.dgvEquipment);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(204, 100);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1553, 669);
            this.pnlCenter.TabIndex = 4;
            // 
            // dgvEquipment
            // 
            this.dgvEquipment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEquipment.Location = new System.Drawing.Point(18, 79);
            this.dgvEquipment.Name = "dgvEquipment";
            this.dgvEquipment.RowTemplate.Height = 25;
            this.dgvEquipment.Size = new System.Drawing.Size(1529, 568);
            this.dgvEquipment.TabIndex = 0;
            // 
            // btnSaveDteq
            // 
            this.btnSaveDteq.Location = new System.Drawing.Point(31, 321);
            this.btnSaveDteq.Name = "btnSaveDteq";
            this.btnSaveDteq.Size = new System.Drawing.Size(150, 30);
            this.btnSaveDteq.TabIndex = 3;
            this.btnSaveDteq.Text = "Save Dist. Equipment";
            this.btnSaveDteq.UseVisualStyleBackColor = true;
            // 
            // btnLoadList
            // 
            this.btnLoadList.Location = new System.Drawing.Point(31, 434);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(150, 30);
            this.btnLoadList.TabIndex = 4;
            this.btnLoadList.Text = "Load List";
            this.btnLoadList.UseVisualStyleBackColor = true;
            // 
            // btnCalculateLoads
            // 
            this.btnCalculateLoads.Location = new System.Drawing.Point(31, 558);
            this.btnCalculateLoads.Name = "btnCalculateLoads";
            this.btnCalculateLoads.Size = new System.Drawing.Size(150, 30);
            this.btnCalculateLoads.TabIndex = 5;
            this.btnCalculateLoads.Text = "Calculate Loads";
            this.btnCalculateLoads.UseVisualStyleBackColor = true;
            // 
            // btnDeleteDteq
            // 
            this.btnDeleteDteq.Location = new System.Drawing.Point(31, 359);
            this.btnDeleteDteq.Name = "btnDeleteDteq";
            this.btnDeleteDteq.Size = new System.Drawing.Size(150, 30);
            this.btnDeleteDteq.TabIndex = 6;
            this.btnDeleteDteq.Text = "Delete Selected Dteq";
            this.btnDeleteDteq.UseVisualStyleBackColor = true;
            // 
            // btnAddLoad
            // 
            this.btnAddLoad.Location = new System.Drawing.Point(31, 470);
            this.btnAddLoad.Name = "btnAddLoad";
            this.btnAddLoad.Size = new System.Drawing.Size(150, 30);
            this.btnAddLoad.TabIndex = 7;
            this.btnAddLoad.Text = "Add Load";
            this.btnAddLoad.UseVisualStyleBackColor = true;
            // 
            // btnDeleteLoad
            // 
            this.btnDeleteLoad.Location = new System.Drawing.Point(31, 506);
            this.btnDeleteLoad.Name = "btnDeleteLoad";
            this.btnDeleteLoad.Size = new System.Drawing.Size(150, 30);
            this.btnDeleteLoad.TabIndex = 8;
            this.btnDeleteLoad.Text = "Delete Load";
            this.btnDeleteLoad.UseVisualStyleBackColor = true;
            // 
            // btnSaveLoads
            // 
            this.btnSaveLoads.Location = new System.Drawing.Point(31, 594);
            this.btnSaveLoads.Name = "btnSaveLoads";
            this.btnSaveLoads.Size = new System.Drawing.Size(150, 30);
            this.btnSaveLoads.TabIndex = 9;
            this.btnSaveLoads.Text = "Save Loads";
            this.btnSaveLoads.UseVisualStyleBackColor = true;
            // 
            // btnCreateCableList
            // 
            this.btnCreateCableList.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCreateCableList.Location = new System.Drawing.Point(31, 698);
            this.btnCreateCableList.Name = "btnCreateCableList";
            this.btnCreateCableList.Size = new System.Drawing.Size(150, 30);
            this.btnCreateCableList.TabIndex = 10;
            this.btnCreateCableList.Text = "Create Cable List";
            this.btnCreateCableList.UseVisualStyleBackColor = true;
            // 
            // lblAssignedLoads
            // 
            this.lblAssignedLoads.AutoSize = true;
            this.lblAssignedLoads.Location = new System.Drawing.Point(31, 189);
            this.lblAssignedLoads.Name = "lblAssignedLoads";
            this.lblAssignedLoads.Size = new System.Drawing.Size(82, 15);
            this.lblAssignedLoads.TabIndex = 11;
            this.lblAssignedLoads.Text = "Show Loading";
            // 
            // lblSelectedTag
            // 
            this.lblSelectedTag.AutoSize = true;
            this.lblSelectedTag.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSelectedTag.Location = new System.Drawing.Point(18, 13);
            this.lblSelectedTag.Name = "lblSelectedTag";
            this.lblSelectedTag.Size = new System.Drawing.Size(37, 21);
            this.lblSelectedTag.TabIndex = 13;
            this.lblSelectedTag.Text = "Tag";
            this.lblSelectedTag.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblListName
            // 
            this.lblListName.AutoSize = true;
            this.lblListName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblListName.Location = new System.Drawing.Point(18, 45);
            this.lblListName.Name = "lblListName";
            this.lblListName.Size = new System.Drawing.Size(86, 21);
            this.lblListName.TabIndex = 12;
            this.lblListName.Text = "List Name";
            this.lblListName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 734);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 30);
            this.button1.TabIndex = 12;
            this.button1.Text = "Create Cable List";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmEquipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 861);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQuickButtons);
            this.Name = "frmEquipment";
            this.Text = "frmEquipment";
            this.pnlQuickButtons.ResumeLayout(false);
            this.pnlQuickButtons.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCenter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipment)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlQuickButtons;
        private Panel pnlHeader;
        private Panel pnlFooter;
        private Panel pnlRight;
        private Panel pnlCenter;
        private Button btnDistributionEquipment;
        private DataGridView dgvEquipment;
        private Button btnAddDteqEq;
        private ListBox lstDteq;
        private Button btnCalculateLoads;
        private Button btnLoadList;
        private Button btnSaveDteq;
        private Button btnDeleteDteq;
        private Button btnDeleteLoad;
        private Button btnAddLoad;
        private Button btnCreateCableList;
        private Button btnSaveLoads;
        private Label lblAssignedLoads;
        private Label lblSelectedTag;
        private Label lblListName;
        private Button button1;
    }
}