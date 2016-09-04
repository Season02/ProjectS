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
            this.secondHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.firstHeader,
            this.secondHeader});
            this.selection_lv.Location = new System.Drawing.Point(16, 14);
            this.selection_lv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.selection_lv.Name = "selection_lv";
            this.selection_lv.Size = new System.Drawing.Size(243, 253);
            this.selection_lv.TabIndex = 0;
            this.selection_lv.UseCompatibleStateImageBehavior = false;
            this.selection_lv.View = System.Windows.Forms.View.Details;
            this.selection_lv.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.selection_lv_ItemSelectionChanged);
            this.selection_lv.DoubleClick += new System.EventHandler(this.selection_lv_DoubleClick);
            // 
            // firstHeader
            // 
            this.firstHeader.Text = "Nothing";
            // 
            // secondHeader
            // 
            this.secondHeader.Text = "Data";
            this.secondHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // select_bt
            // 
            this.select_bt.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.select_bt.Location = new System.Drawing.Point(55, 284);
            this.select_bt.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.select_bt.Name = "select_bt";
            this.select_bt.Size = new System.Drawing.Size(167, 48);
            this.select_bt.TabIndex = 1;
            this.select_bt.Text = "Select";
            this.select_bt.UseVisualStyleBackColor = true;
            this.select_bt.Click += new System.EventHandler(this.select_bt_Click);
            // 
            // SelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 350);
            this.ControlBox = false;
            this.Controls.Add(this.select_bt);
            this.Controls.Add(this.selection_lv);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SelectForm";
            this.Text = "SelectForm";
            this.Resize += new System.EventHandler(this.SelectForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView selection_lv;
        private System.Windows.Forms.Button select_bt;
        private System.Windows.Forms.ColumnHeader firstHeader;
        private System.Windows.Forms.ColumnHeader secondHeader;
    }
}