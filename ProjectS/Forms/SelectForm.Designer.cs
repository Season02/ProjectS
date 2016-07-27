namespace ProjectS
{
    partial class SelectForm
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
            this.selection_lv = new System.Windows.Forms.ListView();
            this.firstHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.select_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // selection_lv
            // 
            this.selection_lv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selection_lv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.selection_lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.firstHeader});
            this.selection_lv.Location = new System.Drawing.Point(12, 12);
            this.selection_lv.Name = "selection_lv";
            this.selection_lv.Size = new System.Drawing.Size(182, 219);
            this.selection_lv.TabIndex = 0;
            this.selection_lv.UseCompatibleStateImageBehavior = false;
            this.selection_lv.View = System.Windows.Forms.View.List;
            // 
            // firstHeader
            // 
            this.firstHeader.Text = "Data";
            // 
            // select_bt
            // 
            this.select_bt.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.select_bt.Location = new System.Drawing.Point(41, 246);
            this.select_bt.Name = "select_bt";
            this.select_bt.Size = new System.Drawing.Size(125, 42);
            this.select_bt.TabIndex = 1;
            this.select_bt.Text = "Select";
            this.select_bt.UseVisualStyleBackColor = true;
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 303);
            this.ControlBox = false;
            this.Controls.Add(this.select_bt);
            this.Controls.Add(this.selection_lv);
            this.Name = "SelectForm";
            this.Text = "SelectForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView selection_lv;
        private System.Windows.Forms.Button select_bt;
        private System.Windows.Forms.ColumnHeader firstHeader;
    }
}