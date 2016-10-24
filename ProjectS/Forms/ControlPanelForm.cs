using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProjectS.Foundation.Command;
using ProjectS.Foundation.Net;

namespace ProjectS.Forms
{
    public partial class ControlPanelForm : Form
    {
        /// <summary>
        /// 用来与 SocketUtils 通信
        /// </summary>
        private SocUnity unity;
        //private int socUnityIndex = -1;

        private Dictionary<string, Object> pairTable = new Dictionary<string, Object>();
        private List<Object> controls = new List<object>();

        public SocUnity Unity
        {
            get
            {
                return unity;
            }
        }

        public event ProcessSocketMonitor.RequestSendByteCommand_Event_Handler RequestSendByteCommand;

        ImageList HeightImageList1 = new ImageList();
        /// <summary>
        /// 似乎设置前应该确认下是否为 NULL 但现在还没有切换到Servant 会清除此值得设定就
        /// 先不管了 ///三连击
        /// </summary>
        //public SocUnity Unity
        //{
        //    set
        //    {
        //        unity = value;
        //    }
        //    //get
        //    //{
        //    //    return data;
        //    //}
        //}

        public ControlPanelForm(SocUnity unity)
        {
            InitializeComponent();

            this.unity = unity;

            HeightImageList1.ImageSize = new Size(20,20);
            CommandListView.SmallImageList = HeightImageList1;
        }

        public void UpdateTaskStatus(STaskUnity task, DateTime date)
        {
            var identifier = task.Identifier;

            foreach(var row in pairTable)
            {
                if (row.Key.Equals(identifier))
                {
                    Invoke(new Action(delegate ()
                    {
                        var controler = row.Value;

                        if (controler.GetType() == typeof(ListViewItem))
                        {
                            var item = controler as ListViewItem;
                            item.SubItems[2].Text = task.Status.ToString();
                            item.SubItems[3].Text = date.ToString();
                        }

                    }));
                    
                    return;
                }
                else
                    continue;
            }
        }

        private void ControlPanelForm_Load(object sender, EventArgs e)
        {
            InitListView();
        }

        private void InitListView()
        {
            CommandListView.Columns.Add("WIDTH = 0", 0, HorizontalAlignment.Center);
            CommandListView.Columns.Add("COMMAND", CommandListView.Width / 4, HorizontalAlignment.Center);
            CommandListView.Columns.Add("STATUS", CommandListView.Width / 4, HorizontalAlignment.Center);
            CommandListView.Columns.Add("DATE", CommandListView.Width / 4, HorizontalAlignment.Center);
            CommandListView.Columns.Add("FIRE", CommandListView.Width / 4, HorizontalAlignment.Center);

            SetByteCommand("0x11", 0x11);
            SetByteCommand("0x12", 0x12);
            SetByteCommand("0x13", 0x13);
            SetByteCommand("PLAY/PAUSE", 0x90);
            SetByteCommand("STOP", 0x91);
            SetByteCommand("PREVIOUS", 0x92);
            SetByteCommand("NEXT", 0x93);
        }

        private void SetByteCommand(string title, byte command)
        {
            SetItem(CommandListView, title, command);
        }

        private void SetItem(ListView lv, string title, byte command)
        {
            var titles = new string[] { "-", title, "-", "-", "-" };
            var item = new ListViewItem(titles);
            lv.Items.Add(item);

            var index = lv.Items.IndexOf(item);
            SetButton(lv, index, (s, e) =>
            {
                var com = new ByteCommandUnity.Command(command);
                var identifier = com.Task.Identifier;
                
                //把发送任务做记录
                pairTable.Add(identifier, item);

                RequestSendByteCommand(this, unity, com);
            });

        }

        //private void SetByteCommand(string title, byte command)
        //{


        //    SetItem(CommandListView, title, (s, e) => 
        //    {
        //        var com = new ByteCommandUnity.Command(command);
        //        var identifier = com.Task.Identifier;

        //        RequestSendByteCommand(this, socUnityIndex, com); }
        //    );
        //}

        //private void SetItem(ListView lv, string title, EventHandler e)
        //{
        //    var titles = new string[] { "-", title, "-", "-", "-" };
        //    var item = new ListViewItem(titles);
        //    lv.Items.Add(item);

        //    var index = lv.Items.IndexOf(item);
        //    SetButton(lv, index, e);

        //}

        private void SetButton(ListView lv, int index, EventHandler e)
        {
            var btn = new Button();
            btn.Text = "LAUNCH";
            btn.Click += e;
            lv.Controls.Add(btn);
            btn.Size = new Size(lv.Items[index].SubItems[4].Bounds.Width,
            lv.Items[index].SubItems[4].Bounds.Height);

            btn.Location = new Point(lv.Items[index].SubItems[4].Bounds.Left, lv.Items[index].SubItems[4].Bounds.Top);
        }

        private void ControlPanelForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
