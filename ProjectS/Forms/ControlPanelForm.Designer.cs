namespace ProjectS.Forms
{
    partial class ControlPanelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TestBtn_1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestBtn_1
            // 
            this.TestBtn_1.Location = new System.Drawing.Point(98, 103);
            this.TestBtn_1.Name = "TestBtn_1";
            this.TestBtn_1.Size = new System.Drawing.Size(75, 23);
            this.TestBtn_1.TabIndex = 0;
            this.TestBtn_1.Text = "Test 1";
            this.TestBtn_1.UseVisualStyleBackColor = true;
            this.TestBtn_1.Click += new System.EventHandler(this.TestBtn_1_Click);
            // 
            // ControlPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.TestBtn_1);
            this.Name = "ControlPanelForm";
            this.Text = "ControlPanelForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TestBtn_1;
    }
}