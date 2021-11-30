
namespace WinFormUI {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lstStringSettings = new System.Windows.Forms.ListBox();
            this.txtStringSettingValue = new System.Windows.Forms.TextBox();
            this.btnSaveSetting = new System.Windows.Forms.Button();
            this.dgvCablesInProject = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvTableSettingValue = new System.Windows.Forms.DataGridView();
            this.lstTableSettings = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableSettingValue)).BeginInit();
            this.SuspendLayout();
            // 
            // lstStringSettings
            // 
            this.lstStringSettings.FormattingEnabled = true;
            this.lstStringSettings.ItemHeight = 15;
            this.lstStringSettings.Location = new System.Drawing.Point(90, 65);
            this.lstStringSettings.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstStringSettings.Name = "lstStringSettings";
            this.lstStringSettings.Size = new System.Drawing.Size(152, 184);
            this.lstStringSettings.TabIndex = 0;
            this.lstStringSettings.SelectedIndexChanged += new System.EventHandler(this.lstProperties_SelectedIndexChanged);
            // 
            // txtStringSettingValue
            // 
            this.txtStringSettingValue.Location = new System.Drawing.Point(287, 65);
            this.txtStringSettingValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtStringSettingValue.Name = "txtStringSettingValue";
            this.txtStringSettingValue.Size = new System.Drawing.Size(248, 23);
            this.txtStringSettingValue.TabIndex = 1;
            // 
            // btnSaveSetting
            // 
            this.btnSaveSetting.Location = new System.Drawing.Point(287, 112);
            this.btnSaveSetting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveSetting.Name = "btnSaveSetting";
            this.btnSaveSetting.Size = new System.Drawing.Size(167, 37);
            this.btnSaveSetting.TabIndex = 2;
            this.btnSaveSetting.Text = "Save";
            this.btnSaveSetting.UseVisualStyleBackColor = true;
            this.btnSaveSetting.Click += new System.EventHandler(this.btnSaveProperty_Click);
            // 
            // dgvCablesInProject
            // 
            this.dgvCablesInProject.AllowUserToAddRows = false;
            this.dgvCablesInProject.AllowUserToDeleteRows = false;
            this.dgvCablesInProject.AllowUserToResizeColumns = false;
            this.dgvCablesInProject.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvCablesInProject.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvCablesInProject.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCablesInProject.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCablesInProject.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvCablesInProject.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCablesInProject.GridColor = System.Drawing.Color.White;
            this.dgvCablesInProject.Location = new System.Drawing.Point(1417, 112);
            this.dgvCablesInProject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvCablesInProject.MultiSelect = false;
            this.dgvCablesInProject.Name = "dgvCablesInProject";
            this.dgvCablesInProject.RowHeadersVisible = false;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvCablesInProject.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvCablesInProject.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvCablesInProject.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCablesInProject.Size = new System.Drawing.Size(247, 567);
            this.dgvCablesInProject.TabIndex = 3;
            this.dgvCablesInProject.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCablesInProject_CellContentClick);
            this.dgvCablesInProject.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCablesInProject_CellEndEdit);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1414, 77);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cables Used in Project";
            // 
            // dgvTableSettingValue
            // 
            this.dgvTableSettingValue.AllowUserToAddRows = false;
            this.dgvTableSettingValue.AllowUserToDeleteRows = false;
            this.dgvTableSettingValue.AllowUserToResizeColumns = false;
            this.dgvTableSettingValue.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvTableSettingValue.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvTableSettingValue.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTableSettingValue.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTableSettingValue.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvTableSettingValue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableSettingValue.GridColor = System.Drawing.Color.White;
            this.dgvTableSettingValue.Location = new System.Drawing.Point(287, 314);
            this.dgvTableSettingValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvTableSettingValue.MultiSelect = false;
            this.dgvTableSettingValue.Name = "dgvTableSettingValue";
            this.dgvTableSettingValue.RowHeadersVisible = false;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvTableSettingValue.RowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvTableSettingValue.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvTableSettingValue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTableSettingValue.Size = new System.Drawing.Size(883, 567);
            this.dgvTableSettingValue.TabIndex = 5;
            // 
            // lstTableSettings
            // 
            this.lstTableSettings.FormattingEnabled = true;
            this.lstTableSettings.ItemHeight = 15;
            this.lstTableSettings.Location = new System.Drawing.Point(90, 314);
            this.lstTableSettings.Name = "lstTableSettings";
            this.lstTableSettings.Size = new System.Drawing.Size(150, 244);
            this.lstTableSettings.TabIndex = 8;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1787, 893);
            this.Controls.Add(this.lstTableSettings);
            this.Controls.Add(this.dgvTableSettingValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCablesInProject);
            this.Controls.Add(this.btnSaveSetting);
            this.Controls.Add(this.txtStringSettingValue);
            this.Controls.Add(this.lstStringSettings);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmSettings";
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableSettingValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstStringSettings;
        private System.Windows.Forms.TextBox txtStringSettingValue;
        private System.Windows.Forms.Button btnSaveSetting;
        private System.Windows.Forms.DataGridView dgvCablesInProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvTableSettingValue;
        private System.Windows.Forms.ListBox lstTableSettings;
    }
}