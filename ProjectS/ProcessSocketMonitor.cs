﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using ProjectS.Foundation.Net;
using ProjectS.Foundation.Command;
using ProjectS.Forms;

namespace ProjectS  
{
    //get connect to internet,if failed try connect to local server.whatever which got connection , monitoring it.
    public class ProcessSocketMonitor
    {
        public delegate void RequestSendByteCommand_Event_Handler(object sender, SocUnity unity, ByteCommandUnity.Command command);

        public delegate void GotNewSocket_Event_Handler(object sender, Socket socket, String SocketIp);
        //public delegate void SocketCleanup_Event_Handler(object sender, int mode);

        public event GotNewSocket_Event_Handler GotNewSocket;
        //public event SocketCleanup_Event_Handler SocketCleanup;

        private bool on_global_mode;
        private bool on_master_mode;

        private List<SocUnity> SocUnityList = new List<SocUnity>();
        private SocUnity ServantSocUnity;

        //IP TASK
        private Dictionary<STaskUnity, SocUnity> taskDic = new Dictionary<STaskUnity, SocUnity>();
        private List<ControlPanelForm> cfList = new List<ControlPanelForm>();

        System.Timers.Timer serGuardian = new System.Timers.Timer(5000);
        
        public bool On_global_mode
        {
            get
            {
                return on_global_mode;
            }
        }

        public ProcessSocketMonitor()
        {
            Main.MasterModeChanged += new Main.MasterMode_Changed_Event_Handler(mastermodeevent);
            Main.GlobalModeChanged += new Main.GlobalMode_Changed_Event_Handler(global_mode_event);

            SocUnity.SocketConnectionLost += new SocUnity.SocketConnectionLost_Event_Handler(SocUnityConnectionLostEvent);
            //ConnectedToServant += new ConnectedToServant_Event_Handler(ConnectedToServantEvent);
        }

        /* ---- ServantMode 设计为被控制端，在Socket中为Server端 ---- */
        private Task ServantMode()
        {
            return Task.Run(() =>
            {
                on_master_mode = false;
                ServantSocUnity = new SocUnity();
                ServantSocUnity.ServerAccepted += new SocUnity.ServerAccepted_Event_Handler(MasterConnected);
                ServantSocUnity.ServerMode(Main.PORT);
            });
        }


        /// <summary>
        /// 设计用来得到一个SOCKET对象，或者一个流对象，因为这是控制面板需要使用的，所以可能一个输出流就足够
        /// 但整体还在建设中，一切都还不确定
        /// 
        /// 或许控制台只需要一个编号，或一个IP，然后做操作时还是有这个或其他类来处理
        /// </summary> 
        /// <param name="ip"></param>
        /// <returns></returns>
        public SocUnity SearchSocketUnity(String ip)
        {
            try
            {
                return SocUnityList.Find((SocUnity su) => { return su.Ip.Equals(ip); });
            }
            catch(ArgumentNullException e)
            {
                return null;
            }
        }

        /// <summary>
        /// 控制面板发送命令时会触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="index"></param>
        /// <param name="command"></param>
        public void RequestSendByteCommendEvent(object sender, SocUnity sunity, ByteCommandUnity.Command command)
        {
            if (sunity == null)
                return;

            try
            {
                sunity.SendByteCommand(command, (ip, result, task) => 
                {
                    if (result == 1)
                    {
                        //这里需要修改，一台电脑上可以启动 多个 SERVANT 这样他们的IP 时相同的无法分辨
                        taskDic.Add(command.Task, sunity);

                        DateTime currentTime = new DateTime();
                        currentTime = DateTime.Now;
                        
                        ((ControlPanelForm)sender).UpdateTaskStatus(command.Task, currentTime);
                    }

                    else
                        MessageBox.Show("Byte Command Send Failed！ target ip: " + ip + "status: " + result);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        /// <summary>
        /// SOCKET连接丢失时执行此函数
        /// 
        /// 此函数任在建设中，对于 SERVANT MODE 中的丢失 SOCKET 现在采用的是直接
        /// 丢弃的做法，但计划是集中到某个集合中，尝试再次利用。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socket"></param>
        /// <param name="ip"></param>
        private void SocUnityConnectionLostEvent(object sender, Socket socket, String ip)
        {
            try
            {
                if (!on_master_mode)
                {
                    var util = SocUnityList.Find((SocUnity su) => { return su.Ip.Equals(ip); });
                    //SocUnityList.Remove(util);
                    util.Stop();
                    //util = null;
                    MessageBox.Show("Master lost :" + ip);
                }
                else
                {
                    MessageBox.Show("Servant lost :" + ip);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("SocUnityConnectionLostEvent // " + e.Message);
            }

        }

        public void ServantListClickedEvent(object sender, string ip)
        {
            try
            {
                var su = SearchSocketUnity(ip);

                if (null != su)
                {
                    ControlPanelForm cpf = new ControlPanelForm(su);
                    cpf.RequestSendByteCommand += new RequestSendByteCommand_Event_Handler(RequestSendByteCommendEvent);

                    //集中保管
                    cfList.Add(cpf);

                    cpf.Show();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        //Servant 在接入到新的 Master 时调用此事件函数
        private void MasterConnected(object sender, Socket socket, String ip)
        {
            if(socket.Connected)
            {
                SocUnity su = new SocUnity();
                su.PassivityMode(socket);
                SocUnityList.Add(su);
            }
            
            //MessageBox.Show("MasterComming : " + ip);
        }

        private void MasterAccepted(IAsyncResult ar)
        {
            if (!on_master_mode)
            {
                var socket = ar.AsyncState as Socket;
                var master = socket.EndAccept(ar);
                var ip = master.RemoteEndPoint.ToString().Split(':')[0];
                //SocketPool.Add(ip, master);
                //MessageBox.Show("new Master: " + ip);
                GotNewSocket(this, master, ip);
                socket.BeginAccept(new AsyncCallback(MasterAccepted), socket);
            }

        }
      
        public static int public_index = -1;
        /// <summary>
        /// 控制模式 首先扫描本地IP地址表 筛选出 IP 前缀，然后进行全地址遍历尝试与所有可能的 Servant 进行
        /// 连接，按照设计对所有 可能的但又未能在本次遍历中连接到的Servant 会进行定时重连操作。
        /// 
        /// 关于自动重连，由于SocUnity自身对Socket中的 Client 模式有自动重连的功能，所以在 ProcessSocketMonitor
        /// 中 只需要监听 SocUnity 的重连事件，然后做相应处理。
        /// </summary>
        /// <returns></returns>
        private Task MasterMode()
        {
            on_master_mode = true;

            return Task.Run(() =>
            {
                IpList list = IpScan.IpScanProceed();
                List<string> tryList;
                //List<String> ipList = IpScan.ipScanProceed();

                //ipList.Clear();
                //ipList.Add(IpScan.IP_LIST_TYPE_TARGET);
                //ipList.Add("127.0.0.1");

                //switch (ipList[0])
                switch (list.Type)
                {
                    //不止一个本地IP，需要挑选
                    case IpList.IP_LIST_TYPE_HOST:
                        //ipList.RemoveAt(0);

                        SelectForm sf = new SelectForm();
                        sf.AddData(list.Ip);
                        sf.ShowDialog();
                        
                        String tmp = list.Ip[public_index];

                        //ipList.Clear();
                        //ipList.Add(tmp);

                        tryList = IpScan.BuidIpList(tmp);

                        break;

                    //遍历完成，直接使用
                    //  现在，我觉得上面一句有问题，不能直接使用，要遍历
                    case IpList.IP_LIST_TYPE_TARGET:
                        //ipList.RemoveAt(0);
                        tryList = IpScan.BuidIpList(list.Ip[0]);

                        break;

                    default:
                        return;
                }

                /*
                var total = tryList.Count;
                var loop = 4;
                int per = total / loop;
                int tail = total % 4;

                for(int i = 0;i < loop; i++)
                {
                    IpScan.trySoc(SocUnityList, tryList, i * per, per);

                    if(i == loop - 1)
                        IpScan.trySoc(SocUnityList, tryList, (i + 1) * per, 1);
                }
                */

                foreach (var ip in tryList)
                {
                    TryToGetServant(ip);
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(TryToGetServant), ip);
                    //Task t = new Task(() => TryToGetServant(ip));
                    //t.Start();

                    //Task.Factory.StartNew(() => TryToGetServant(ip));



                    //SocUnity su = new SocUnity();

                    //Task<int> task = Task<int>.Factory.StartNew(() => su.ClientMode(ip, Main.PORT));

                    //task.ContinueWith(unit =>
                    //{
                    //    if (unit.Result == 1)
                    //        SocUnityList.Add(su);
                    //    //else 根据错误码做相应处理
                    //});
                }

                SocUnity.SocketReconnected += new SocUnity.SocketReconnected_Event_Handler(ServantReconnectedEvent);
            });
        }

        // 尝试与 Servant 进行连接，无论连接成功与否 新建的SocUnity都会加入到SocUnityList中，用于后期维护。
        // 关于还未连接完成的 SocUnity 的后续处理目前任在建设中。
        private void TryToGetServant(string ip)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                SocUnity su = new SocUnity();
                int status = -1000;

                su.ClientMode(ip, Main.PORT, out status);

                if (status == 1)
                {
                    SocUnityList.Add(su);
                    su.ReceivedSTask += new SocUnity.STaskReceived_Event_Handler(ReceivedStask);
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// 接收到 SERVANT 发来的 STASK !!!!!!!!!!!!!!!! 逻辑繁杂，需要优化
        /// </summary>
        /// <param name="stask"></param>
        private void ReceivedStask(object sender, STaskUnity stask)
        {
            var identifier = stask.Identifier;

            //先更新下 TASK 表
            foreach (var raw in taskDic)
            {
                if (raw.Key.Identifier == identifier)
                {
                    //注意不要把更新到这个 task 变量上了，这是一个临时变量，更新它没用
                    var task = raw.Key;

                    if (task.TaskType == STaskUnity.Task.StatusTask)
                    {
                        raw.Key.Status = stask.Status;
                    }
                    else if (task.TaskType == STaskUnity.Task.ProgressTask)
                    {
                        raw.Key.Progress = stask.Progress;
                    }
                }
            }

            //然后通知更新界面
            for (int i = cfList.Count - 1; i >= 0; i--)
            {
                if (cfList[i].IsDisposed)
                {
                    cfList.RemoveAt(i);
                    continue;
                }

                //此循环的目的在于找到一个存在于 大概
                foreach (var raw in taskDic)
                {
                    if(raw.Key.Identifier == stask.Identifier)
                    {
                        var unity = raw.Value;

                        if(cfList[i].Unity == unity)
                        {
                            DateTime currentTime = new DateTime();
                            currentTime = DateTime.Now;

                            cfList[i].UpdateTaskStatus(stask, currentTime);

                            return;
                        }
                    }
                }
            }
        }

        private void ServantReconnectedEvent(object sender, Socket socket, String ip)
        {
            MessageBox.Show("SocketReconnectedEvent ip: " + ip);
        }

        private void ConnectedToServantEvent(object sender, Socket socket, String ip)
        {
            MessageBox.Show("Servant connected : " + ip);
        }

        private void SocketReset()
        {
            SocUnity.SocketConnected -= new SocUnity.SocketConnected_Event_Handler(ConnectedToServantEvent);

            SocUnityList.Clear();
            ServantSocUnity = null;
            //SocketCleanup(this, 0);
        }

        private void mastermodeevent(object sender, int mode_code)
        {
            switch(mode_code)
            {
                case Main.MASTER_MODE:
                    DebugForm.DMes("Master Mode");
                    SocketReset();
                    MasterMode();
                    break;

                case Main.SERVANT_MODE:
                    DebugForm.DMes("Servant Mode");
                    SocketReset();
                    ServantMode();
                    break;

                default:
                    break;
            }
        }

        private void global_mode_event(object sender, int mode_code)
        {
            switch (mode_code)
            {
                case Main.Local_MODE:
                    on_global_mode = false;
                    DebugForm.DMes("Local Mode");
                    break;

                case Main.Global_MODE:
                    on_global_mode = true;
                    DebugForm.DMes("Global Mode");
                    break;

                default:
                    break;
            }
        }

        //try to get connect to server,if server on internet then return 1,if on local net return 2,if failed to get connect return -1;
        private int Connect()
        {
            return -1;
        }
     

    }
}
