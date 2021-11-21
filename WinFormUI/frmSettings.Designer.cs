
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
            this.lstProperties = new System.Windows.Forms.ListBox();
            this.txtPropertyValue = new System.Windows.Forms.TextBox();
            this.btnSaveProperty = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstProperties
            // 
            this.lstProperties.FormattingEnabled = true;
            this.lstProperties.Location = new System.Drawing.Point(97, 108);
            this.lstProperties.Name = "lstProperties";
            this.lstProperties.Size = new System.Drawing.Size(131, 121);
            this.lstProperties.TabIndex = 0;
            this.lstProperties.SelectedIndexChanged += new System.EventHandler(this.lstProperties_SelectedIndexChanged);
            // 
            // txtPropertyValue
            // 
            this.txtPropertyValue.Location = new System.Drawing.Point(268, 112);
            this.txtPropertyValue.Name = "txtPropertyValue";
            this.txtPropertyValue.Size = new System.Drawing.Size(213, 20);
            this.txtPropertyValue.TabIndex = 1;
            // 
            // btnSaveProperty
            // 
            this.btnSaveProperty.Location = new System.Drawing.Point(268, 165);
            this.btnSaveProperty.Name = "btnSaveProperty";
            this.btnSaveProperty.Size = new System.Drawing.Size(143, 32);
            this.btnSaveProperty.TabIndex = 2;
            this.btnSaveProperty.Text = "Save";
            this.btnSaveProperty.UseVisualStyleBackColor = true;
            this.btnSaveProperty.Click += new System.EventHandler(this.btnSaveProperty_Click);
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1243, 625);
            this.Controls.Add(this.btnSaveProperty);
            this.Controls.Add(this.txtPropertyValue);
            this.Controls.Add(this.lstProperties);
            this.Name = "frmSettings";
            this.Text = "Project Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstProperties;
        private System.Windows.Forms.TextBox txtPropertyValue;
        private System.Windows.Forms.Button btnSaveProperty;
    }
}