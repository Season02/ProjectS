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
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
            
            initLV();
            ProcessCommand.SocketPoolUpdate += new ProcessCommand.SocketPoolUpdate_Event_Handler(SocketPoolUpdated);
        }

        private void SocketPoolUpdated(object sender, int PoolSize, String CurrentIp, bool Increase)
        {
            //UpdataList(Increase, CurrentIp, DateTime.Now.ToLocalTime().ToString());
            SocketPoolLV.Invoke(new Action(delegate () { UpdataList(Increase, CurrentIp, DateTime.Now.ToLocalTime().ToString()); }));
        }

        void initLV()
        {
            SocketPoolLV.Columns.Add("IP", 165, HorizontalAlignment.Center);
            SocketPoolLV.Columns.Add("TIME", 120, HorizontalAlignment.Center);
        }

        private void UpdataList(bool increase, String ip, String time)
        {
            var lvi = new ListViewItem();
            lvi.Text = ip;
            lvi.SubItems.Add(time);
            SocketPoolLV.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            if (increase)
            {
                SocketPoolLV.Items.Add(lvi);
                SocketPoolLV.Items[SocketPoolLV.Items.Count - 1].EnsureVisible();//滚动到最后
            }
            else
            {
                if (SocketPoolLV.Items.Count > 0 && ip.Trim() != string.Empty)
                {
                    for (int i = 0; i < SocketPoolLV.Items.Count; i++)
                    {
                        if (SocketPoolLV.Items[i].Text.Equals(ip))
                        {
                            SocketPoolLV.Items.RemoveAt(i);
                            break;
                        }
                    }
                }
            }                
            
            SocketPoolLV.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }
        
    }
}
