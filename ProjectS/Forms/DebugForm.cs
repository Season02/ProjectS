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

        private static List<String> ls_debug = new List<string>();
        private static int ls_debug_pointer = 0;

        System.Timers.Timer ls_updator = new System.Timers.Timer(1);

        public static void DMes(String mes)
        {
            try
            {
                lock(ls_debug)
                {
                    ls_debug.Add("[" + DateTime.Now.ToString() + "] " + mes);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("DMes error: " + e.Message);
            }
        }

        public DebugForm()
        {
            InitializeComponent();
            
            initLV();
            //ProcessCommand.SocketPoolUpdate += new ProcessCommand.SocketPoolUpdate_Event_Handler(SocketPoolUpdated);
            Main.DebugFormShow += new Main.DebugForm_Show_Event_Handler(FormShowOrHide);
        }

        private void FormShowOrHide(object sender, bool show)
        {
            if (show)
                this.Show();
            else
                this.Hide();
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

        private void DebugForm_Activated(object sender, EventArgs e)
        {
            ls_updator.Elapsed += new System.Timers.ElapsedEventHandler((s, ev) => lsUpdaterFunc(s, ev));
            ls_updator.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            ls_updator.Enabled = true; //是否触发Elapsed事件      
            ls_updator.Start();
        }

        private void lsUpdaterFunc(Object sender, EventArgs e)
        {
            ls_updator.Stop();

            while(ls_debug_pointer<ls_debug.Count)
            {
                tb_debug.Invoke(new Action(delegate () { tb_debug.AppendText(ls_debug[ls_debug_pointer] + "\r\n"); }));
                ls_debug_pointer++;
            }

            ls_updator.Start();            
        }

        private void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("closing?");
            e.Cancel = true; ;
        }

    }
}
