
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lstProperties = new System.Windows.Forms.ListBox();
            this.txtPropertyValue = new System.Windows.Forms.TextBox();
            this.btnSaveProperty = new System.Windows.Forms.Button();
            this.dgvCablesInProject = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvSetting = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetting)).BeginInit();
            this.SuspendLayout();
            // 
            // lstProperties
            // 
            this.lstProperties.FormattingEnabled = true;
            this.lstProperties.ItemHeight = 15;
            this.lstProperties.Location = new System.Drawing.Point(90, 65);
            this.lstProperties.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lstProperties.Name = "lstProperties";
            this.lstProperties.Size = new System.Drawing.Size(152, 289);
            this.lstProperties.TabIndex = 0;
            this.lstProperties.SelectedIndexChanged += new System.EventHandler(this.lstProperties_SelectedIndexChanged);
            // 
            // txtPropertyValue
            // 
            this.txtPropertyValue.Location = new System.Drawing.Point(287, 65);
            this.txtPropertyValue.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtPropertyValue.Name = "txtPropertyValue";
            this.txtPropertyValue.Size = new System.Drawing.Size(248, 23);
            this.txtPropertyValue.TabIndex = 1;
            // 
            // btnSaveProperty
            // 
            this.btnSaveProperty.Location = new System.Drawing.Point(323, 112);
            this.btnSaveProperty.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnSaveProperty.Name = "btnSaveProperty";
            this.btnSaveProperty.Size = new System.Drawing.Size(167, 37);
            this.btnSaveProperty.TabIndex = 2;
            this.btnSaveProperty.Text = "Save";
            this.btnSaveProperty.UseVisualStyleBackColor = true;
            this.btnSaveProperty.Click += new System.EventHandler(this.btnSaveProperty_Click);
            // 
            // dgvCablesInProject
            // 
            this.dgvCablesInProject.AllowUserToAddRows = false;
            this.dgvCablesInProject.AllowUserToDeleteRows = false;
            this.dgvCablesInProject.AllowUserToResizeColumns = false;
            this.dgvCablesInProject.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvCablesInProject.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCablesInProject.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCablesInProject.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCablesInProject.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCablesInProject.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCablesInProject.GridColor = System.Drawing.Color.White;
            this.dgvCablesInProject.Location = new System.Drawing.Point(1417, 112);
            this.dgvCablesInProject.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvCablesInProject.MultiSelect = false;
            this.dgvCablesInProject.Name = "dgvCablesInProject";
            this.dgvCablesInProject.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvCablesInProject.RowsDefaultCellStyle = dataGridViewCellStyle3;
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
            // dgvSetting
            // 
            this.dgvSetting.AllowUserToAddRows = false;
            this.dgvSetting.AllowUserToDeleteRows = false;
            this.dgvSetting.AllowUserToResizeColumns = false;
            this.dgvSetting.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvSetting.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSetting.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSetting.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSetting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSetting.GridColor = System.Drawing.Color.White;
            this.dgvSetting.Location = new System.Drawing.Point(323, 225);
            this.dgvSetting.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvSetting.MultiSelect = false;
            this.dgvSetting.Name = "dgvSetting";
            this.dgvSetting.RowHeadersVisible = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvSetting.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSetting.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvSetting.Size = new System.Drawing.Size(883, 625);
            this.dgvSetting.TabIndex = 5;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1787, 893);
            this.Controls.Add(this.dgvSetting);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCablesInProject);
            this.Controls.Add(this.btnSaveProperty);
            this.Controls.Add(this.txtPropertyValue);
            this.Controls.Add(this.lstProperties);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmSettings";
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSetting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstProperties;
        private System.Windows.Forms.TextBox txtPropertyValue;
        private System.Windows.Forms.Button btnSaveProperty;
        private System.Windows.Forms.DataGridView dgvCablesInProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvSetting;
    }
}