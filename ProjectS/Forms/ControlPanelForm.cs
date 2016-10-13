using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectS.Forms
{
    public partial class ControlPanelForm : Form
    {
        /// <summary>
        /// 用来与 SocketUtils 通信
        /// </summary>
        private SocUnity unity;

        /// <summary>
        /// 似乎设置前应该确认下是否为 NULL 但现在还没有切换到Servant 会清除此值得设定就
        /// 先不管了 ///三连击
        /// </summary>
        public SocUnity Unity
        {
            set
            {
                unity = value;
            }
            //get
            //{
            //    return data;
            //}
        }

        public ControlPanelForm()
        {
            InitializeComponent();
        }

        private void bto0x11_Click(object sender, EventArgs e)
        {
            unity.SendByteCommand(0x11, (ip,result) => { MessageBox.Show("send information: ip - " + ip + " status - " + result); });
        }
    }
}
