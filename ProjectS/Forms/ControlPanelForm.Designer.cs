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
            this.CommandListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // CommandListView
            // 
            this.CommandListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CommandListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CommandListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommandListView.Location = new System.Drawing.Point(0, 3);
            this.CommandListView.Name = "CommandListView";
            this.CommandListView.Size = new System.Drawing.Size(725, 578);
            this.CommandListView.TabIndex = 1;
            this.CommandListView.UseCompatibleStateImageBehavior = false;
            this.CommandListView.View = System.Windows.Forms.View.Details;
            // 
            // ControlPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 618);
            this.Controls.Add(this.CommandListView);
            this.Name = "ControlPanelForm";
            this.Text = "ControlPanelForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlPanelForm_FormClosed);
            this.Load += new System.EventHandler(this.ControlPanelForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView CommandListView;
    }
}