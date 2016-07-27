using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectS
{
    //SOCKET HELPER
    class SocUnity
    {
        public delegate void ServerAccepted_Event_Handler(object sender, Socket socket, String ip);
        public delegate void SocketConnectionLost_Event_Handler(object sender, Socket socket, String ip);
        public delegate void SocketReconnected_Event_Handler(object sender, Socket socket, String ip);
        public delegate void StreamComming_Event_Handler(object sender, Socket socket, byte[] stream);
        public delegate void SocketConnected_Event_Handler(object sender, Socket socket, String ip);

        public event ServerAccepted_Event_Handler ServerAccepted;
        public static event SocketConnectionLost_Event_Handler SocketConnectionLost;
        public static event SocketReconnected_Event_Handler SocketReconnected;
        public event StreamComming_Event_Handler StreamComming;
        public static event SocketConnected_Event_Handler SocketConnected;

        private Socket socket;
        private String ip;
        private int port;

        public String Ip
        {
            get { return ip; }
        }

        public int Port
        {
            get { return port; }
        }

        const int HEAD_LENGTH = 32;//byte

        private const int DefaultStreamBufferSize = 1024;
        private static int StreamBufferSize = DefaultStreamBufferSize;
        private byte[] StreamBuffer = new byte[StreamBufferSize];

        public static bool server_running = false;

        public bool ServerRunning
        {
            get { return server_running; }
        }

        public bool AutoReconnect = true;
        private bool NeedReconnect = false;

        //String fileName;
        //bool Transfering = false;

        private int connection_count = 0;

        public int ConnectionCount
        {
            get { return connection_count; }
        }

        private System.Timers.Timer AutoReconnecter;

        private void UpdateStreamBuffer(int size)
        {
            StreamBufferSize = size;
            StreamBuffer = new byte[size];
        }

        public bool Connected()
        {
            try
            {
                return socket.Connected;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public SocUnity()
        {
            AutoReconnecter = new System.Timers.Timer(2000);
            AutoReconnecter.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => AutoReconnectFunc(s, e));
            AutoReconnecter.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            AutoReconnecter.Enabled = true; //是否触发Elapsed事件            

            SocketConnectionLost += new SocketConnectionLost_Event_Handler(SocketConnectionLostFunc);
        }

        private void AutoReconnectFunc(Object Sender, EventArgs e)
        {
            if(AutoReconnect && NeedReconnect)
            {
                AutoReconnecter.Stop();
                DebugForm.DMes("AutoReconnectFunc start: " + ip + " port : " + port);

                try
                {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint address = new IPEndPoint(IPAddress.Parse(ip), port);
                    socket.Connect(IPAddress.Parse(ip), port);
                    ClientMode(socket);
                    NeedReconnect = false;
                    SocketReconnected(this, socket, ip);

                    DebugForm.DMes("AutoReconnectFunc Succeed: " + ip + " port : " + port);

                    //AutoReconnecter.Start();
                }
                catch (SocketException error)
                {
                    MessageBox.Show("AutoReconnectFunc error: " + error.Message);
                    //AutoReconnecter.Start();

                    DebugForm.DMes("AutoReconnectFunc failed: " + ip + " port : " + port + " reason: " + error.Message);

                }
                finally
                {
                    AutoReconnecter.Start();
                }
            }
        }

        public void Stop()
        {
            try
            {
                if (socket != null)
                {
                    this.socket.Close();
                    this.socket.Dispose();
                    this.socket = null;
                }
            }
            catch (Exception e)
            {
                DebugForm.DMes("Socunity stoped. ip: " + ip + " port : " + port + " reason: " + e.Message);
                //MessageBox.Show("Stop: " + e.Message);
            }
        }

        private void SocketConnectionLostFunc(object sender, Socket socket, String SocketIp)
        {
            try
            {
                DebugForm.DMes("SocketConnectionLostFunc. ip: " + ip + " port : " + port);

                if (socket != null)
                {
                    this.socket.Close();
                    this.socket.Dispose();
                    this.socket = null;
                }

                UpdateStreamBuffer(DefaultStreamBufferSize);
            }
            catch(Exception e)
            {
                MessageBox.Show("SocketConnectionLostFunc: " + e.Message);
            }

            if(AutoReconnect)
            {
                NeedReconnect = true;
            }

        }

        public void ClientMode(String ip, int port)
        {
            System.Net.IPAddress IP = System.Net.IPAddress.Parse(ip);//转换成IP地址

            Thread t = new Thread(delegate()
            {
                IPEndPoint address = new IPEndPoint(IP, port);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(IP);//址获取 DNS 主机信息。 
                    string strHostName = myScanHost.HostName.ToString();//获取主机的名            

                    DebugForm.DMes("ClientMode: connecting to: Host Name: " + strHostName + " ip:" + ip + " port : " + port);

                    socket.Connect(IP, Main.PORT);
                }
                catch (SocketException error)
                {
                    switch (error.ErrorCode)
                    {
                        //The attempt to connect timed out.
                        case 10060:
                            return;

                        //Connection is forcefully rejected.
                        case 10061:
                            return;

                        //Authoritative answer: Host not found.
                        case 11001:
                            return;

                        //Valid name, no data record of requested type
                        case 11004:
                            return;

                        default:
                            MessageBox.Show("error code: " + error.ErrorCode + "\r\n" + error.Message + " ip: " + IP.ToString());
                            return;
                    }
                }
                catch (Exception e)
                {
                    DebugForm.DMes("ClientMode: connecting Error: ip:" + ip + " port : " + port);
                    //MessageBox.Show(e.Message + " ip: " + IP.ToString());
                    return;
                }
                ClientMode(socket);
                SocketConnected(this.socket, socket, socket.RemoteEndPoint.ToString().Split(':')[0]);
            });
            t.IsBackground = true;
            t.Start();
        }

        public bool ClientMode(Socket socket)
        {
            if(this.socket != null) return false;
            this.socket = socket;
            connection_count++;
            AutoReconnecter.Start();

            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            port = Convert.ToInt32(socket.RemoteEndPoint.ToString().Split(':')[1]);
            DebugForm.DMes("ClientMode ip: " + ip + " port : " + port);

            Thread t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);
                }
                catch (Exception e)
                {
                    SocketConnectionLost(this, socket, ip);
                    MessageBox.Show("ClientMode Error: " + e.Message + " ip: " + ip);
                }
            }));
            t.IsBackground = true;
            t.Start();            

            return true;
        }

        public static int Byte2Int(byte[] array, int index)
        {
            int data = 0;
            int space = 4;

            for (int i = 0; i < space; i++)
            {
                data += array[i + index * space];
                if (i < (space - 1))//at last loop we do not need shift data
                    data = (data) << 8;
            }
            return data;
        }
        
        private void BeginReceiveCallback(IAsyncResult ar)
        {
            Socket socket;
            int stream_length;
            try
            {
                socket = ar.AsyncState as Socket;
                stream_length = socket.EndReceive(ar);
                if (stream_length == 0)
                {
                    SocketConnectionLost(this, this.socket, ip);
                    DebugForm.DMes("BeginReceiveCallback error ip: " + ip + " EndReceive by zero!");
                    MessageBox.Show(ip + " EndReceive by zero!");
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("BeginReceiveCallback err ip:" + ip + "message:" + e.Message);
                SocketConnectionLost(this, this.socket, ip);
                return;
            }

            //int length_bytes = StreamBuffer.Length;
            //byte[] stream = StreamBuffer;

            // fixed: head length 32byte; contain : this packet type[4byte],this packet data length[4](contain head),data
            // behind is a packet framework :type + length + data  and then depend on type the section data will change
            // example: 
                //1,type code command : type 1,length 36byte data 4byte , will do command by data code
                //2,type cmd command : type 2 length xxbyte data xx-32 byte, will excute command as data is
                //3,tyoe transfer file: type 3 ,length xxbyte data xx-32byte ,data are seprated as [new buffer size][packge count][last packge size][file name]...
                //and will prepare for receice the new file,when get the second pacekt,check the type,if not 4,we got a error,if yes,then receive file
                //4,etc...

            int type = Byte2Int(StreamBuffer, 0);
            int length = Byte2Int(StreamBuffer, 1);
            byte[] data = new byte[length - HEAD_LENGTH];
            Array.Copy(StreamBuffer, HEAD_LENGTH, data, 0, data.Length);
            MessageBox.Show("stream_length: " + stream_length + " length: " + length);

            switch(type)
            {
                case 100://clasic byte command
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        ByteCommand bc = new ByteCommand(socket);
                        bc.Execute(data);
                    }));
                    t.IsBackground = true;
                    t.Start();
                    break;

                default:
                    break;
            }

            if (1 < 0)
            {
                switch (3)
                {
                    case 1://CMD Comand
                        try
                        {
                            //byte[] temp = new byte[length_bytes - STREAM_HEAD_LENGTH];
                            //Array.Copy(stream, 16, temp, 0, temp.Length);
                            //String str = System.Text.Encoding.UTF8.GetString(temp).TrimEnd();

                            //MessageBox.Show("Start a Process: " + str);
                            //System.Diagnostics.Process.Start(str);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }

                        break;

                    case 2://Prepare file receive
                        //try
                        //{
                        //    if (Transfering == false)
                        //    {
                        //        bufferSize = getHead(StreamBuffer, 4);
                        //        int size = 0;
                        //        for (int i = STREAM_HEAD_LENGTH + 4; i < stream.Length; i++)//extract the length of file name ,have a question
                        //        {
                        //            if (stream[i] == 0)
                        //                break;
                        //            size++;
                        //        }
                        //        byte[] temp = new byte[size];
                        //        Array.Copy(stream, (STREAM_HEAD_LENGTH + 4), temp, 0, temp.Length);
                        //        fileName = System.Text.Encoding.UTF8.GetString(temp).Trim();
                        //        loopCount = 0;
                        //        Transfering = true;

                        //        MessageBox.Show("buffer size: " + bufferSize.ToString() + " bit file name: " + fileName);
                        //        Thread.Sleep(200);//I really really have no idea why need this
                        //        MyFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                        //        setStreamSize(bufferSize + STREAM_HEAD_LENGTH);
                        //        LogBuilder.buildLog("file [ " + fileName + " ] start receving.");
                        //    }

                        //}
                        //catch (Exception e)
                        //{
                        //    //MessageBox.Show(e.Message);
                        //    LogBuilder.buildLog("Prepare file receive Error: " + e.Message);
                        //    Transfering = false;
                        //}

                        break;

                    case 3://file reveiving
                        //try
                        //{
                        //    if (getHead(stream, 1) != loopCount)
                        //    {
                        //        MyFileStream.Write(StreamBuffer, STREAM_HEAD_LENGTH, StreamBufferSize - STREAM_HEAD_LENGTH);
                        //        MyFileStream.Flush(true);//must be true,i don't know why
                        //    }
                        //    else
                        //    {
                        //        if (getHead(stream, 3) > 0)
                        //        {
                        //            MyFileStream.Write(stream, STREAM_HEAD_LENGTH, getHead(stream, 3) - STREAM_HEAD_LENGTH);
                        //            MyFileStream.Flush(true);
                        //        }
                        //        else
                        //        {
                        //            MyFileStream.Write(stream, STREAM_HEAD_LENGTH, stream.Length - STREAM_HEAD_LENGTH);
                        //            MyFileStream.Flush(true);
                        //        }

                        //        System.Media.SystemSounds.Hand.Play();
                        //        LogBuilder.buildLog("Received Successful: " + fileName + "packet count: " + loopCount + "Last packet size: " + getHead(StreamBuffer, 3));
                        //        MyFileStream.Close();
                        //        setStreamSize(1024);

                        //        fileName = "";
                        //        SocketComHelper.transmitCommand(socket, MK_FLAG_FILE_RECEIVED, null);
                        //        MessageBox.Show("FILE RECEIVED!");
                        //        Transfering = false;
                        //        //////////////SEND FEEDBACK TO SENDER!//////////////////

                        //    }

                        //    ++loopCount;
                        //}
                        //catch (Exception e)
                        //{
                        //    MyFileStream.Close();
                        //    setStreamSize(1024);
                        //    Transfering = false;
                        //    MessageBox.Show("err MyFileStream.Write: " + e.Message);
                        //    LogBuilder.buildLog("err MyFileStream.Write: " + e.Message);
                        //}

                        break;
                }
            }
            //else
            //{
            //    if (length_bytes == 1024)
            //    {
            //        //Thread t = new Thread(new ThreadStart(() =>
            //        //{
            //        //    beControl(stream);
            //        //}));
            //        //t.IsBackground = true;
            //        //t.Start();
            //    }

            //}

            try
            {
                socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);
            }
            catch (Exception e)
            {
                MessageBox.Show("BeginReceive Inside: " + e.Message);
            }
            
        }

        public bool PassivityMode(Socket socket)
        {
            //if (this.socket != null) this.socket = null;
            this.socket = socket;
            connection_count++;
            AutoReconnect = false;

            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            port = Convert.ToInt32(socket.RemoteEndPoint.ToString().Split(':')[1]);
            //MessageBox.Show("ClientMode ip: " + ip + " port : " + port);
            DebugForm.DMes("ClientMode ip: " + ip + " port : " + port);

            Thread t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);
                    //MessageBox.Show("BeginReceive");
                }
                catch (Exception e)
                {
                    SocketConnectionLost(this, socket, socket.RemoteEndPoint.ToString().Split(':')[0]);
                    MessageBox.Show("BeginReceive Outside: " + e.Message + " ip: " + socket.RemoteEndPoint.ToString().Split(':')[0]);
                }
                //MessageBox.Show("End of line");
            }));
            t.IsBackground = true;
            t.Start();

            return true;
        }

        public bool ServerMode(int port)//Only one!!!
        {
            if(!ServerRunning)
            {
                this.port = port;
                server_running = true;

                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Bind(new IPEndPoint(IPAddress.Any, port));
                        socket.Listen(256);
                        socket.BeginAccept(new AsyncCallback(ServerModeCallback), socket);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(this + e.Message);
                        Thread.Sleep(1000);
                        //ServantMode();
                    }
                }));
                t.IsBackground = true;
                t.Start();

                return true;
            }
            return false;            
        }

        private void ServerModeCallback(IAsyncResult ar)
        {
            var socket = ar.AsyncState as Socket;
            var client = socket.EndAccept(ar);
            var ip = client.RemoteEndPoint.ToString().Split(':')[0];
            ServerAccepted(this, client, ip);
            //MessageBox.Show("ServerModeCallback");
            socket.BeginAccept(new AsyncCallback(ServerModeCallback), socket);
        }

    }
}
