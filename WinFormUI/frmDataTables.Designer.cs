
namespace WinFormUI {
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
            this.lblSelectedTag = new System.Windows.Forms.Label();
            this.lblListName = new System.Windows.Forms.Label();
            this.pnlQuickButtons = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.dgvDataTable = new System.Windows.Forms.DataGridView();
            this.lstDataTables = new System.Windows.Forms.ListBox();
            this.btnGetTables = new System.Windows.Forms.Button();
            this.pnlQuickButtons.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectedTag
            // 
            this.lblSelectedTag.AutoSize = true;
            this.lblSelectedTag.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedTag.Location = new System.Drawing.Point(-342, -96);
            this.lblSelectedTag.Name = "lblSelectedTag";
            this.lblSelectedTag.Size = new System.Drawing.Size(37, 21);
            this.lblSelectedTag.TabIndex = 14;
            this.lblSelectedTag.Text = "Tag";
            this.lblSelectedTag.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblListName
            // 
            this.lblListName.AutoSize = true;
            this.lblListName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListName.Location = new System.Drawing.Point(-342, -64);
            this.lblListName.Name = "lblListName";
            this.lblListName.Size = new System.Drawing.Size(86, 21);
            this.lblListName.TabIndex = 13;
            this.lblListName.Text = "List Name";
            this.lblListName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // pnlQuickButtons
            // 
            this.pnlQuickButtons.Controls.Add(this.btnGetTables);
            this.pnlQuickButtons.Controls.Add(this.lstDataTables);
            this.pnlQuickButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlQuickButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlQuickButtons.Name = "pnlQuickButtons";
            this.pnlQuickButtons.Size = new System.Drawing.Size(223, 891);
            this.pnlQuickButtons.TabIndex = 17;
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.dgvDataTable);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(223, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1620, 891);
            this.pnlCenter.TabIndex = 20;
            // 
            // dgvDataTable
            // 
            this.dgvDataTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDataTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDataTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDataTable.ColumnHeadersHeight = 35;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDataTable.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDataTable.EnableHeadersVisualStyles = false;
            this.dgvDataTable.Location = new System.Drawing.Point(16, 188);
            this.dgvDataTable.Name = "dgvDataTable";
            this.dgvDataTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataTable.Size = new System.Drawing.Size(1540, 680);
            this.dgvDataTable.TabIndex = 0;
            // 
            // lstDataTables
            // 
            this.lstDataTables.FormattingEnabled = true;
            this.lstDataTables.Location = new System.Drawing.Point(21, 224);
            this.lstDataTables.Name = "lstDataTables";
            this.lstDataTables.Size = new System.Drawing.Size(180, 238);
            this.lstDataTables.TabIndex = 0;
            this.lstDataTables.SelectedIndexChanged += new System.EventHandler(this.lstDataTables_SelectedIndexChanged);
            // 
            // btnGetTables
            // 
            this.btnGetTables.Location = new System.Drawing.Point(36, 188);
            this.btnGetTables.Name = "btnGetTables";
            this.btnGetTables.Size = new System.Drawing.Size(107, 23);
            this.btnGetTables.TabIndex = 1;
            this.btnGetTables.Text = "Get Tables";
            this.btnGetTables.UseVisualStyleBackColor = true;
            this.btnGetTables.Click += new System.EventHandler(this.btnGetTables_Click);
            // 
            // frmDataTables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1843, 891);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlQuickButtons);
            this.Controls.Add(this.lblSelectedTag);
            this.Controls.Add(this.lblListName);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmDataTables";
            this.Text = "frmDataTables";
            this.Load += new System.EventHandler(this.frmDataTables_Load);
            this.pnlQuickButtons.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSelectedTag;
        private System.Windows.Forms.Label lblListName;
        private System.Windows.Forms.Panel pnlQuickButtons;
        private System.Windows.Forms.Panel pnlCenter;
        private System.Windows.Forms.DataGridView dgvDataTable;
        private System.Windows.Forms.ListBox lstDataTables;
        private System.Windows.Forms.Button btnGetTables;
    }
}