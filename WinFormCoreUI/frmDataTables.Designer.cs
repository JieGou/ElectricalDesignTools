namespace WinFormCoreUI {
    partial class frmDataTables {
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
            this.pnlQuickButtons = new System.Windows.Forms.Panel();
            this.lstDataTables = new System.Windows.Forms.ListBox();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.dgvDataTable = new System.Windows.Forms.DataGridView();
            this.pnlQuickButtons.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlQuickButtons
            // 
            this.pnlQuickButtons.Controls.Add(this.lstDataTables);
            this.pnlQuickButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlQuickButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQuickButtons.Name = "pnlQuickButtons";
            this.pnlQuickButtons.Size = new System.Drawing.Size(200, 861);
            this.pnlQuickButtons.TabIndex = 18;
            // 
            // lstDataTables
            // 
            this.lstDataTables.FormattingEnabled = true;
            this.lstDataTables.ItemHeight = 15;
            this.lstDataTables.Location = new System.Drawing.Point(28, 168);
            this.lstDataTables.Name = "lstDataTables";
            this.lstDataTables.Size = new System.Drawing.Size(145, 454);
            this.lstDataTables.TabIndex = 0;
            this.lstDataTables.SelectedIndexChanged += new System.EventHandler(this.lstDataTables_SelectedIndexChanged);
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.dgvDataTable);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(200, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1634, 861);
            this.pnlCenter.TabIndex = 21;
            // 
            // dgvDataTable
            // 
            this.dgvDataTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDataTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataTable.ColumnHeadersHeight = 35;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDataTable.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDataTable.EnableHeadersVisualStyles = false;
            this.dgvDataTable.Location = new System.Drawing.Point(43, 168);
            this.dgvDataTable.Name = "dgvDataTable";
            this.dgvDataTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataTable.Size = new System.Drawing.Size(1526, 640);
            this.dgvDataTable.TabIndex = 0;
            // 
            // frmDataTables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1834, 861);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlQuickButtons);
            this.Name = "frmDataTables";
            this.Text = "frmDataTables";
            this.Load += new System.EventHandler(this.frmDataTables_Load);
            this.pnlQuickButtons.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlQuickButtons;
        private ListBox lstDataTables;
        private Panel pnlCenter;
        private DataGridView dgvDataTable;
    }
}