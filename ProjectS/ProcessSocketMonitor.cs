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
    // Connect() -> Monitoring() -> Destroy() -> Connect()...
    class ProcessSocketMonitor
    {
        private Socket socket;//Server socket
        //private List<Socket> SocketPool = new List<Socket>();
        //private Dictionary<String, Socket> SocketPool = new Dictionary<String, Socket>();
        private int server_count;//Count of server

        public delegate void GotConnection_Event_Handler(object sender, Socket socket ,int server_count);
        public delegate void LostConnection_Event_Handler(object sender, Socket socket, int server_count);
        public delegate void GotNewSocket_Event_Handler(object sender, Socket socket, String SocketIp);
        public delegate void SocketCleanup_Event_Handler(object sender, int mode);

        public event GotConnection_Event_Handler GotConnection_Event;
        public event LostConnection_Event_Handler LostConnection_Event;
        public event GotNewSocket_Event_Handler GotNewSocket;
        public event SocketCleanup_Event_Handler SocketCleanup;

        private bool on_global_mode;
        private bool on_master_mode;

        //private Dictionary<String, IPAddress> MachinePortOff = new Dictionary<String, IPAddress>();
        //private Dictionary<String, IPAddress> MachineOffLine = new Dictionary<String, IPAddress>();
        private List<IPAddress> MachineConnectionFailed = new List<IPAddress>();
        private List<IPAddress> MachineOffLine = new List<IPAddress>();

        public bool On_global_mode
        {
            get
            {
                return on_global_mode;
            }
        }

        public ProcessSocketMonitor(Main main)
        {
            Main.MasterModeChanged += new Main.MasterMode_Changed_Event_Handler(mastermodeevent);
            main.GlobalModeChanged += new Main.GlobalMode_Changed_Event_Handler(global_mode_event);
        }

        public void start()
        {
            Thread monitor = new Thread(new ThreadStart(() =>
            {

            }));
            monitor.IsBackground = true;
            monitor.Start();
        }

        private Task ServantMode()
        {
            return Task.Run(() =>
            {
                on_master_mode = false;

                MessageBox.Show("Servant Mode");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, Main.PORT));
                socket.Listen(128);

                try
                {
                    socket.BeginAccept(new AsyncCallback(MasterAccepted), socket);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    Thread.Sleep(1000);
                    //ServantMode();
                }
                
            });
        }


        private void MasterAccepted(IAsyncResult ar)
        {
            if (!on_master_mode)
            {
                var socket = ar.AsyncState as Socket;
                var master = socket.EndAccept(ar);
                var ip = master.RemoteEndPoint.ToString();
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

                MessageBox.Show("Master Mode");
                foreach ( var ip in ipScan.ipScanProceed() )
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TryToGetServant), ip);
            });
        }

        private void TryToGetServant(Object str)
        {
            System.Net.IPAddress IP = System.Net.IPAddress.Parse(str as String);//转换成IP地址

            Thread t = new Thread(delegate()
            {
                IPEndPoint address = new IPEndPoint(IP, Main.PORT);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(IP);//址获取 DNS 主机信息。 
                    string strHostName = myScanHost.HostName.ToString();//获取主机的名                    

                    //socket.BeginConnect(address, new AsyncCallback(ConnectedToServant), socket);
                    socket.Connect(IP, Main.PORT);
                }
                catch (SocketException error)
                {
                    switch(error.ErrorCode)
                    {
                        //The attempt to connect timed out.
                        case 10060:
                            lock(MachineConnectionFailed)
                            MachineConnectionFailed.Add(IP);
                            return;

                        //Connection is forcefully rejected.
                        case 10061:
                            lock (MachineConnectionFailed)
                            MachineConnectionFailed.Add(IP);
                            return;

                        //Authoritative answer: Host not found.
                        case 11001:
                            lock (MachineOffLine)
                            MachineOffLine.Add(IP);
                            return;

                        //Valid name, no data record of requested type
                        case 11004:
                            lock (MachineOffLine)
                            MachineOffLine.Add(IP);
                            return;

                        default:
                            MessageBox.Show("error code: " + error.ErrorCode + "\r\n" + error.Message + " ip: " + IP.ToString());
                            return;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message + " ip: " + IP.ToString());
                    return;
                }
                //MessageBox.Show("new Servant: " + socket.RemoteEndPoint.ToString());
                GotNewSocket(this, socket, socket.RemoteEndPoint.ToString());
            });
            t.Start();
        }

        private void SocketReset()
        {
            if(socket != null)
            {
                socket.Close();
                socket.Dispose();
                socket = null;
            }

            SocketCleanup(this, 0);
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
                    break;

                case Main.Global_MODE:
                    on_global_mode = true;
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
