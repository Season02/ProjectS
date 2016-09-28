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
            this.bto0x11 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bto0x11
            // 
            this.bto0x11.Location = new System.Drawing.Point(98, 103);
            this.bto0x11.Name = "bto0x11";
            this.bto0x11.Size = new System.Drawing.Size(75, 23);
            this.bto0x11.TabIndex = 0;
            this.bto0x11.Text = "0x11";
            this.bto0x11.UseVisualStyleBackColor = true;
            this.bto0x11.Click += new System.EventHandler(this.bto0x11_Click);
            // 
            // ControlPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.bto0x11);
            this.Name = "ControlPanelForm";
            this.Text = "ControlPanelForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bto0x11;
    }
}