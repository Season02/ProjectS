namespace ProjectS
{
    partial class DebugForm
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
            this.SocketPoolLV = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // SocketPoolLV
            // 
            this.SocketPoolLV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SocketPoolLV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SocketPoolLV.Location = new System.Drawing.Point(12, 12);
            this.SocketPoolLV.Name = "SocketPoolLV";
            this.SocketPoolLV.Size = new System.Drawing.Size(319, 399);
            this.SocketPoolLV.TabIndex = 0;
            this.SocketPoolLV.UseCompatibleStateImageBehavior = false;
            this.SocketPoolLV.View = System.Windows.Forms.View.Details;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 423);
            this.Controls.Add(this.SocketPoolLV);
            this.Name = "DebugForm";
            this.Text = "DebugForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView SocketPoolLV;
    }
}