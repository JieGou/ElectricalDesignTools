
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
            this.lstProperties = new System.Windows.Forms.ListBox();
            this.txtPropertyValue = new System.Windows.Forms.TextBox();
            this.btnSaveProperty = new System.Windows.Forms.Button();
            this.dgvCablesInProject = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).BeginInit();
            this.SuspendLayout();
            // 
            // lstProperties
            // 
            this.lstProperties.FormattingEnabled = true;
            this.lstProperties.Location = new System.Drawing.Point(97, 108);
            this.lstProperties.Name = "lstProperties";
            this.lstProperties.Size = new System.Drawing.Size(131, 251);
            this.lstProperties.TabIndex = 0;
            this.lstProperties.SelectedIndexChanged += new System.EventHandler(this.lstProperties_SelectedIndexChanged);
            // 
            // txtPropertyValue
            // 
            this.txtPropertyValue.Location = new System.Drawing.Point(266, 108);
            this.txtPropertyValue.Name = "txtPropertyValue";
            this.txtPropertyValue.Size = new System.Drawing.Size(213, 20);
            this.txtPropertyValue.TabIndex = 1;
            // 
            // btnSaveProperty
            // 
            this.btnSaveProperty.Location = new System.Drawing.Point(297, 149);
            this.btnSaveProperty.Name = "btnSaveProperty";
            this.btnSaveProperty.Size = new System.Drawing.Size(143, 32);
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
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCablesInProject.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCablesInProject.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCablesInProject.GridColor = System.Drawing.Color.White;
            this.dgvCablesInProject.Location = new System.Drawing.Point(644, 108);
            this.dgvCablesInProject.MultiSelect = false;
            this.dgvCablesInProject.Name = "dgvCablesInProject";
            this.dgvCablesInProject.RowHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvCablesInProject.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCablesInProject.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvCablesInProject.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCablesInProject.Size = new System.Drawing.Size(212, 491);
            this.dgvCablesInProject.TabIndex = 3;
            this.dgvCablesInProject.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCablesInProject_CellContentClick);
            this.dgvCablesInProject.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCablesInProject_CellEndEdit);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(641, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cables Used in Project";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1342, 714);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvCablesInProject);
            this.Controls.Add(this.btnSaveProperty);
            this.Controls.Add(this.txtPropertyValue);
            this.Controls.Add(this.lstProperties);
            this.Name = "frmSettings";
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCablesInProject)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstProperties;
        private System.Windows.Forms.TextBox txtPropertyValue;
        private System.Windows.Forms.Button btnSaveProperty;
        private System.Windows.Forms.DataGridView dgvCablesInProject;
        private System.Windows.Forms.Label label1;
    }
}