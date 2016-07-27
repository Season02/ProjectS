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
    public partial class SelectForm : Form
    {
        public SelectForm()
        {
            InitializeComponent();

            UpdataList("192.168.3.100");
            UpdataList("192.168.3.107");
        }

        private void UpdataList(String data)
        {
            var lvi = new ListViewItem();
            lvi.Text = data;
            //lvi.SubItems.Add(status);
            selection_lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            //if (increase)
            //{
                selection_lv.Items.Add(lvi);
                selection_lv.Items[selection_lv.Items.Count - 1].EnsureVisible();//滚动到最后
                //ServantLv.Items[1].SubItems[0].Text = "";
            //}
            //else
            //{
            //    if (selection_lv.Items.Count > 0 && ip.Trim() != string.Empty)
            //    {
            //        foreach (ListViewItem item in selection_lv.Items)
            //        {
            //            if (item.Text.Equals(ip))
            //            {
            //                item.SubItems[0].Text = status;
            //                break;
            //            }
            //        }
            //    }
            //}

            selection_lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

    }
}
