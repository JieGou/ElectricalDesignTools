namespace WinFormCoreUI {
    partial class frmLoads {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlQuickButtons = new System.Windows.Forms.Panel();
            this.cbxLoadTypeToAdd = new System.Windows.Forms.ComboBox();
            this.btnLoadList = new System.Windows.Forms.Button();
            this.btnCalculateLoads = new System.Windows.Forms.Button();
            this.btnToggleEditor = new System.Windows.Forms.Button();
            this.btnSaveLoads = new System.Windows.Forms.Button();
            this.btnDeleteLoad = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.dgvEditor = new System.Windows.Forms.DataGridView();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.lblSelectedTag = new System.Windows.Forms.Label();
            this.lblListName = new System.Windows.Forms.Label();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.pnlQuickButtons.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEditor)).BeginInit();
            this.pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlQuickButtons
            // 
            this.pnlQuickButtons.Controls.Add(this.cbxLoadTypeToAdd);
            this.pnlQuickButtons.Controls.Add(this.btnLoadList);
            this.pnlQuickButtons.Controls.Add(this.btnCalculateLoads);
            this.pnlQuickButtons.Controls.Add(this.btnToggleEditor);
            this.pnlQuickButtons.Controls.Add(this.btnSaveLoads);
            this.pnlQuickButtons.Controls.Add(this.btnDeleteLoad);
            this.pnlQuickButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlQuickButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQuickButtons.Name = "pnlQuickButtons";
            this.pnlQuickButtons.Size = new System.Drawing.Size(200, 861);
            this.pnlQuickButtons.TabIndex = 1;
            // 
            // cbxLoadTypeToAdd
            // 
            this.cbxLoadTypeToAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbxLoadTypeToAdd.FormattingEnabled = true;
            this.cbxLoadTypeToAdd.Location = new System.Drawing.Point(29, 724);
            this.cbxLoadTypeToAdd.Name = "cbxLoadTypeToAdd";
            this.cbxLoadTypeToAdd.Size = new System.Drawing.Size(149, 23);
            this.cbxLoadTypeToAdd.TabIndex = 10;
            this.cbxLoadTypeToAdd.Text = "MOTOR";
            this.cbxLoadTypeToAdd.SelectedIndexChanged += new System.EventHandler(this.cbxLoadTypeToAdd_SelectedIndexChanged);
            // 
            // btnLoadList
            // 
            this.btnLoadList.Location = new System.Drawing.Point(29, 169);
            this.btnLoadList.Name = "btnLoadList";
            this.btnLoadList.Size = new System.Drawing.Size(150, 30);
            this.btnLoadList.TabIndex = 4;
            this.btnLoadList.Text = "Load List";
            this.btnLoadList.UseVisualStyleBackColor = true;
            this.btnLoadList.Click += new System.EventHandler(this.btnLoadList_Click);
            // 
            // btnCalculateLoads
            // 
            this.btnCalculateLoads.Location = new System.Drawing.Point(29, 293);
            this.btnCalculateLoads.Name = "btnCalculateLoads";
            this.btnCalculateLoads.Size = new System.Drawing.Size(150, 30);
            this.btnCalculateLoads.TabIndex = 5;
            this.btnCalculateLoads.Text = "Calculate Loads";
            this.btnCalculateLoads.UseVisualStyleBackColor = true;
            // 
            // btnToggleEditor
            // 
            this.btnToggleEditor.Location = new System.Drawing.Point(29, 670);
            this.btnToggleEditor.Name = "btnToggleEditor";
            this.btnToggleEditor.Size = new System.Drawing.Size(150, 30);
            this.btnToggleEditor.TabIndex = 7;
            this.btnToggleEditor.Text = "Show / Hide Editor";
            this.btnToggleEditor.UseVisualStyleBackColor = true;
            this.btnToggleEditor.Click += new System.EventHandler(this.btnToggleEditor_Click);
            // 
            // btnSaveLoads
            // 
            this.btnSaveLoads.Location = new System.Drawing.Point(29, 329);
            this.btnSaveLoads.Name = "btnSaveLoads";
            this.btnSaveLoads.Size = new System.Drawing.Size(150, 30);
            this.btnSaveLoads.TabIndex = 9;
            this.btnSaveLoads.Text = "Save Loads";
            this.btnSaveLoads.UseVisualStyleBackColor = true;
            // 
            // btnDeleteLoad
            // 
            this.btnDeleteLoad.Location = new System.Drawing.Point(29, 241);
            this.btnDeleteLoad.Name = "btnDeleteLoad";
            this.btnDeleteLoad.Size = new System.Drawing.Size(150, 30);
            this.btnDeleteLoad.TabIndex = 8;
            this.btnDeleteLoad.Text = "Delete Load";
            this.btnDeleteLoad.UseVisualStyleBackColor = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(200, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1659, 100);
            this.pnlHeader.TabIndex = 2;
            // 
            // pnlFooter
            // 
            this.pnlFooter.Controls.Add(this.dgvEditor);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(200, 661);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1659, 200);
            this.pnlFooter.TabIndex = 3;
            // 
            // dgvEditor
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.dgvEditor.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEditor.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvEditor.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvEditor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvEditor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEditor.Location = new System.Drawing.Point(18, 52);
            this.dgvEditor.Name = "dgvEditor";
            this.dgvEditor.RowHeadersVisible = false;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(159)))), ((int)(((byte)(224)))));
            this.dgvEditor.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvEditor.RowTemplate.Height = 25;
            this.dgvEditor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvEditor.Size = new System.Drawing.Size(1580, 88);
            this.dgvEditor.TabIndex = 1;
            this.dgvEditor.Validating += new System.ComponentModel.CancelEventHandler(this.dgvEditor_Validating);
            // 
            // pnlRight
            // 
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(1832, 100);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(27, 561);
            this.pnlRight.TabIndex = 4;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.lblSelectedTag);
            this.pnlCenter.Controls.Add(this.lblListName);
            this.pnlCenter.Controls.Add(this.dgvMain);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(200, 100);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1632, 561);
            this.pnlCenter.TabIndex = 5;
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
            // dgvMain
            // 
            this.dgvMain.AllowUserToResizeColumns = false;
            this.dgvMain.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.dgvMain.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMain.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.NullValue = null;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMain.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Location = new System.Drawing.Point(21, 80);
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(159)))), ((int)(((byte)(224)))));
            this.dgvMain.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvMain.RowTemplate.Height = 25;
            this.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMain.ShowCellToolTips = false;
            this.dgvMain.Size = new System.Drawing.Size(1580, 467);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMain_CellClick);
            this.dgvMain.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMain_CellDoubleClick);
            // 
            // frmLoads
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1859, 861);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQuickButtons);
            this.Name = "frmLoads";
            this.Text = "frmLoads";
            this.Load += new System.EventHandler(this.frmLoads_Load);
            this.pnlQuickButtons.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEditor)).EndInit();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCenter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Button btnLoadList;
        private Button btnCalculateLoads;
        private Button btnToggleEditor;
        private Button btnSaveLoads;
        private Button btnDeleteLoad;
        private Panel pnlQuickButtons;
        private Panel pnlHeader;
        private Panel pnlFooter;
        private Panel pnlRight;
        private Panel pnlCenter;
        private Label lblSelectedTag;
        private Label lblListName;
        private DataGridView dgvMain;
        private ComboBox cbxLoadTypeToAdd;
        private DataGridView dgvEditor;
    }
}