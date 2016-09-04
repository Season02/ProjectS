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
            this.tb_debug = new System.Windows.Forms.TextBox();
            this.SocketPoolLV = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // tb_debug
            // 
            this.tb_debug.AllowDrop = true;
            this.tb_debug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tb_debug.BackColor = System.Drawing.SystemColors.WindowText;
            this.tb_debug.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_debug.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.tb_debug.Location = new System.Drawing.Point(13, 12);
            this.tb_debug.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tb_debug.Multiline = true;
            this.tb_debug.Name = "tb_debug";
            this.tb_debug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_debug.Size = new System.Drawing.Size(705, 603);
            this.tb_debug.TabIndex = 1;
            // 
            // SocketPoolLV
            // 
            this.SocketPoolLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SocketPoolLV.BackColor = System.Drawing.SystemColors.WindowText;
            this.SocketPoolLV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SocketPoolLV.ForeColor = System.Drawing.SystemColors.Window;
            this.SocketPoolLV.Location = new System.Drawing.Point(13, 621);
            this.SocketPoolLV.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SocketPoolLV.Name = "SocketPoolLV";
            this.SocketPoolLV.Size = new System.Drawing.Size(705, 17);
            this.SocketPoolLV.TabIndex = 0;
            this.SocketPoolLV.UseCompatibleStateImageBehavior = false;
            this.SocketPoolLV.View = System.Windows.Forms.View.Details;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InfoText;
            this.ClientSize = new System.Drawing.Size(731, 650);
            this.ControlBox = false;
            this.Controls.Add(this.tb_debug);
            this.Controls.Add(this.SocketPoolLV);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "DebugForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DebugForm";
            this.Activated += new System.EventHandler(this.DebugForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tb_debug;
        private System.Windows.Forms.ListView SocketPoolLV;
    }
}