using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectS
{
    public partial class BlueForm : Form
    {
        public BlueForm()
        {
            InitializeComponent();
        }

        private void BlueForm_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        public void cleanLab()
        {
            //listLab = new List<Label>();

            foreach (System.Windows.Forms.Control control in this.Controls)//遍历Form上的所有控件
            {
                //if (control is System.Windows.Forms.Label)
                //{
                //    listLab.Add(control as Label);
                //}
                control.Text = "";
            }


        }

    }
}
