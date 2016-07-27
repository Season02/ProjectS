using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace ProjectS
{
    public partial class FormMasterMode : Form
    {
        public FormMasterMode()
        {
            InitializeComponent();
        }

        private void FormMasterMode_Load(object sender, EventArgs e)
        {
            initLV();
            SocUnity.SocketConnected += new SocUnity.SocketConnected_Event_Handler(ConnectedToServantEvent);
            SocUnity.SocketConnectionLost += new SocUnity.SocketConnectionLost_Event_Handler(SocketConnectionLostEvent);
        }

        private void ConnectedToServantEvent(object sender, Socket socket, String ip)
        {
            try
            {
                lock (ServantLv)
                {
                    ServantLv.Invoke(new Action(delegate() { UpdataList(true, ip, "online"); }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ConnectedToServantEvent error: " + e.Message);
            }
        }

        private void SocketConnectionLostEvent(object sender, Socket socket, String ip)
        {
            try
            {
                lock (ServantLv)
                {
                    ServantLv.Invoke(new Action(delegate() { UpdataList(false, ip, "offline"); }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ConnectedToServantEvent error: " + e.Message);
            }
        }

        private void FormMasterMode_FormClosing(object sender, FormClosingEventArgs e)
        {

        }


        void initLV()
        {
            ServantLv.Columns.Add("IP", 165, HorizontalAlignment.Center);
            ServantLv.Columns.Add("STATUS", 120, HorizontalAlignment.Center);
        }

        private void UpdataList(bool increase, String ip, String status)
        {
            var lvi = new ListViewItem();
            lvi.Text = ip;
            lvi.SubItems.Add(status);
            ServantLv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            if (increase)
            {
                ServantLv.Items.Add(lvi);
                ServantLv.Items[ServantLv.Items.Count - 1].EnsureVisible();//滚动到最后
                //ServantLv.Items[1].SubItems[0].Text = "";
            }
            else
            {
                if (ServantLv.Items.Count > 0 && ip.Trim() != string.Empty)
                {
                    foreach (ListViewItem item in ServantLv.Items)
                    {
                        if (item.Text.Equals(ip))
                        {
                            item.SubItems[0].Text = status;
                            break;
                        }
                    }
                }
            }

            ServantLv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        private void DMes(String mes)
        {
 
        }


    }
}
