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
using ProjectS.Forms;

namespace ProjectS
{
    public partial class FormMasterMode : Form
    {
        public delegate void ServantListClicked_Event_Handler(object sender, string ip);
        public ServantListClicked_Event_Handler ServantListClicked;

        /// <summary>
        /// 用来与 SocketUtils 通信
        /// </summary>
        private ProcessSocketMonitor socketMonitor;

        /// <summary>
        /// 似乎设置前应该确认下是否为 NULL 但现在还没有切换到Servant 会清除此值得设定就
        /// 先不管了 ///三连击
        /// </summary>
        public ProcessSocketMonitor SocketMonitor
        {
            set
            {
                socketMonitor = value;
            }
            //get
            //{
            //    return data;
            //}
        }

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
                            item.SubItems[1].Text = status;
                            break;
                        }
                    }
                }
            }

            ServantLv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        /// <summary>
        /// 发送流什么的目前没有这方面想法，简单粗暴的把 SocketUtil 传递给
        /// Control Panel 就OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServantLv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ServantLv.SelectedItems.Count > 0)
            {
                ServantListClicked(this, ServantLv.SelectedItems[0].Text);

                //var su = socketMonitor.SearchSocketUnity(ServantLv.SelectedItems[0].Text);

                //if(null != su)
                //{
                //    ////获取到SOCKET,传递给 CPF
                //    ControlPanelForm cpf = new ControlPanelForm();

                //    cpf.RequestSendByteCommand += new ProcessSocketMonitor.RequestSendByteCommand_Event_Handler(socketMonitor.RequestSendByteCommendEvent);

                //    //cpf.Unity = su;
                //    cpf.Show();
                //}

            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            //ControlPanelForm cpf = new ControlPanelForm();
            //cpf.Show();
        }
    }
}
