using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ProjectS  
{
    //get connect to internet,if failed try connect to local server.whatever which got connection , monitoring it.
    class ProcessSocketMonitor
    {
        public delegate void GotNewSocket_Event_Handler(object sender, Socket socket, String SocketIp);
        //public delegate void SocketCleanup_Event_Handler(object sender, int mode);

        public event GotNewSocket_Event_Handler GotNewSocket;
        //public event SocketCleanup_Event_Handler SocketCleanup;

        private bool on_global_mode;
        private bool on_master_mode;

        private List<SocUnity> SocUnityList = new List<SocUnity>();
        private SocUnity ServantSocUnity;

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

        private void SocUnityConnectionLostEvent(object sender, Socket socket, String ip)
        {
            try
            {
                if (!on_master_mode)
                {
                    var util = SocUnityList.Find((SocUnity su) => { return su.Ip.Equals(ip); });
                    SocUnityList.Remove(util);
                    util.Stop();
                    util = null;
                    MessageBox.Show("Master lost :" + ip);
                }
                else
                {
                    MessageBox.Show("Servant lost :" + ip);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void MasterConnected(object sender, Socket socket, String ip)
        {
            SocUnity su = new SocUnity();
            su.PassivityMode(socket);
            SocUnityList.Add(su);

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

        private Task MasterMode()
        {
            return Task.Run(() =>
            {
                on_master_mode = true;

                List<String> tmp = ipScan.ipScanProceed();
                switch(tmp[0])
                {
                    //不止一个本地IP，需要挑选
                    case ipScan.IP_LIST_TYPE_HOST:
                        MessageBox.Show("PLEASE FIX THIS FIRST!");
                        break;

                    //遍历完成，直接使用
                    case ipScan.IP_LIST_TYPE_TARGET:
                        tmp.RemoveAt(0);
                        break;

                    default:
                        return;
                }




                foreach ( var ip in tmp)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TryToGetServant), ip);
                }
                
                SocUnity.SocketReconnected += new SocUnity.SocketReconnected_Event_Handler(ServantReconnectedEvent);
            });
        }

        private void ServantReconnectedEvent(object sender, Socket socket, String ip)
        {
            MessageBox.Show("SocketReconnectedEvent ip: " + ip);
        }

        private void TryToGetServant(Object str)
        {
            try
            {
                String ip = str as String;
                SocUnity su = new SocUnity();
                su.ClientMode(ip, Main.PORT);
                SocUnityList.Add(su);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
            
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
