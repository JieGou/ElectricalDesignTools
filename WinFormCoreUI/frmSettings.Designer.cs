namespace WinFormCoreUI {
    partial class frmSettings {
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
            this.lstStringSettings = new System.Windows.Forms.ListBox();
            this.txtStringSettingValue = new System.Windows.Forms.TextBox();
            this.btnSaveSetting = new System.Windows.Forms.Button();
            this.dgvTableSettingValue = new System.Windows.Forms.DataGridView();
            this.lstTableSettings = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableSettingValue)).BeginInit();
            this.SuspendLayout();
            // 
            // lstStringSettings
            // 
            this.lstStringSettings.FormattingEnabled = true;
            this.lstStringSettings.ItemHeight = 15;
            this.lstStringSettings.Location = new System.Drawing.Point(73, 61);
            this.lstStringSettings.Name = "lstStringSettings";
            this.lstStringSettings.Size = new System.Drawing.Size(150, 184);
            this.lstStringSettings.TabIndex = 0;
            this.lstStringSettings.SelectedIndexChanged += new System.EventHandler(this.lstStringSettings_SelectedIndexChanged);
            // 
            // txtStringSettingValue
            // 
            this.txtStringSettingValue.Location = new System.Drawing.Point(247, 61);
            this.txtStringSettingValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStringSettingValue.Name = "txtStringSettingValue";
            this.txtStringSettingValue.Size = new System.Drawing.Size(150, 23);
            this.txtStringSettingValue.TabIndex = 2;
            // 
            // btnSaveSetting
            // 
            this.btnSaveSetting.Location = new System.Drawing.Point(247, 102);
            this.btnSaveSetting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveSetting.Name = "btnSaveSetting";
            this.btnSaveSetting.Size = new System.Drawing.Size(150, 32);
            this.btnSaveSetting.TabIndex = 3;
            this.btnSaveSetting.Text = "Save";
            this.btnSaveSetting.UseVisualStyleBackColor = true;
            this.btnSaveSetting.Click += new System.EventHandler(this.btnSaveSetting_Click);
            // 
            // dgvTableSettingValue
            // 
            this.dgvTableSettingValue.AllowUserToAddRows = false;
            this.dgvTableSettingValue.AllowUserToDeleteRows = false;
            this.dgvTableSettingValue.AllowUserToResizeColumns = false;
            this.dgvTableSettingValue.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvTableSettingValue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTableSettingValue.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTableSettingValue.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTableSettingValue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvTableSettingValue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableSettingValue.GridColor = System.Drawing.Color.White;
            this.dgvTableSettingValue.Location = new System.Drawing.Point(247, 314);
            this.dgvTableSettingValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvTableSettingValue.MaximumSize = new System.Drawing.Size(900, 575);
            this.dgvTableSettingValue.MultiSelect = false;
            this.dgvTableSettingValue.Name = "dgvTableSettingValue";
            this.dgvTableSettingValue.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvTableSettingValue.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvTableSettingValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvTableSettingValue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTableSettingValue.Size = new System.Drawing.Size(900, 540);
            this.dgvTableSettingValue.TabIndex = 6;
            this.dgvTableSettingValue.DataSourceChanged += new System.EventHandler(this.dgvTableSettingValue_DataSourceChanged);
            // 
            // lstTableSettings
            // 
            this.lstTableSettings.FormattingEnabled = true;
            this.lstTableSettings.ItemHeight = 15;
            this.lstTableSettings.Location = new System.Drawing.Point(73, 314);
            this.lstTableSettings.Name = "lstTableSettings";
            this.lstTableSettings.Size = new System.Drawing.Size(150, 154);
            this.lstTableSettings.TabIndex = 7;
            this.lstTableSettings.SelectedIndexChanged += new System.EventHandler(this.lstTableSettings_SelectedIndexChanged);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 861);
            this.Controls.Add(this.lstTableSettings);
            this.Controls.Add(this.dgvTableSettingValue);
            this.Controls.Add(this.btnSaveSetting);
            this.Controls.Add(this.txtStringSettingValue);
            this.Controls.Add(this.lstStringSettings);
            this.Name = "frmSettings";
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableSettingValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox lstStringSettings;
        private TextBox txtStringSettingValue;
        private Button btnSaveSetting;
        private DataGridView dgvTableSettingValue;
        private ListBox lstTableSettings;
    }
}