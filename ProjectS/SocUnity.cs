using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using Newtonsoft.Json;

using ProjectS.CommonClasses.Util;

namespace ProjectS
{
    //SOCKET HELPER
    public class SocUnity
    {
        public delegate void DelegateSendDone(String ip, int state);

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
        private int errorCodeConnect = 0;

        //32位最大能一次表达4G的文件，现在不会一次发送传送超过这个量级
        private int DefaultBufferSize = 1024;

        public int ErrorCodeConnect
        {
            get { return errorCodeConnect; }
        }

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

        public bool AutoReconnect = false;//是否开启自动重连
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

            socket.ReceiveTimeout = 2000;//设置RECEIVE 方法接收超时时间
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
                    //this.socket.Close();
                    //this.socket.Dispose();
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

        //Task<String> task = Task.Factory.StartNew<String>(() => DownloadString("kkk"));
        //String taskResult = Task.Result;

        //Func<string, int> methond = Work;
        //IAsyncResult cookie = Method.BeginInovke("test",null,null);

        //int result = Method.EndInvoke(cookie);

        public int ClientMode(String ip, int port)
        {
            System.Net.IPAddress IP = System.Net.IPAddress.Parse(ip);//转换成IP地址
            //int return_code = 0;

            //Thread t = new Thread(new ThreadStart(() =>
            //{
                IPEndPoint address = new IPEndPoint(IP, port);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(IP);//址获取 DNS 主机信息.
                    string strHostName = myScanHost.HostName.ToString();//获取主机的名

                    DebugForm.DMes("ClientMode- connecting to: Host Name: " + strHostName + " ip:" + ip + " port : " + port);

                    socket.Connect(IP, Main.PORT);
                }
                catch (SocketException error)
                {
                    switch (error.ErrorCode)
                    {
                        //The attempt to connect timed out.
                        case 10060:
                            break;

                        //Connection is forcefully rejected.
                        case 10061:
                            break;

                        //Authoritative answer: Host not found.
                        case 11001:
                            break;

                        //Valid name, no data record of requested type
                        case 11004:
                            break;

                        default:
                            MessageBox.Show("error code: " + error.ErrorCode + "\r\n" + error.Message + " ip: " + IP.ToString());
                            break;
                    }

                    //errorCodeConnect = error.ErrorCode;
                    DebugForm.DMes("CMode Error at: " + ip + ":" + port + " with code: " + error.ErrorCode);
                    return error.ErrorCode;
                }
                catch (Exception e)
                {
                    //errorCodeConnect = -1;
                    DebugForm.DMes("ClientMode: connecting Error: " + ip + ":" + port);
                    MessageBox.Show(e.Message + " ip: " + IP.ToString());
                    return -1;
                }

            try
            {
                ClientMode(socket);
                SocketConnected(this, socket, ip);
            }
            catch(NullReferenceException e)
            {
                MessageBox.Show(e.Message + " ip: " + IP.ToString());
                return -2;
            }
            
            //else errorCodeConnect = -2;

            //}));
            //t.IsBackground = true;
            //t.Start();

            return 1;
        }

        public bool ClientMode(Socket socket)
        {
            //if(this.socket != null) return false;
            this.socket = socket;
            connection_count++;
            AutoReconnecter.Start();

            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            port = Convert.ToInt32(socket.RemoteEndPoint.ToString().Split(':')[1]);
            DebugForm.DMes("ClientMode(Socket socket)-> ip: " + ip + " port : " + port);

            //Thread t = new Thread(new ThreadStart(() =>
            //{
            //Task.Run(() =>
            //{
            //    try
            //    {
            socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);
            //    }
            //    catch (Exception e)
            //    {
            //        SocketConnectionLost(this, socket, ip);
            //        MessageBox.Show("ClientMode Error: " + e.Message + " ip: " + ip);
            //    }
            //});
                
            //}));
            //t.IsBackground = true;
            //t.Start();            

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


        /// <summary>
        /// 一个原型，用来进行SOCKET的发送，目前认为发送JSON格式比较合适，但数据的结构处理和原先相同，即发送
        /// 定长的数据，比如1024B，这是基本的情况，在数据比较长，比如发送文件的情况下，用一个属性进行标记，比
        /// 如总包数，当前包索引等。
        /// 
        /// 这个函数应该会有很多的重载版本，因为使用JSON格式，在发送时要指定好包数，附加信息等等，有些简单的命
        /// 令可能因为结构简单就不需要一一指定了.
        /// 
        /// 现在先试着写一个最简单的版本,即兼容原先的 ByteCommand 的版本。
        /// 
        /// 答UTF8肯定能识别汉字的，google网页就是UTF8，只是解码的时候要用原来的编码解码，
        /// 如果是utf8就要用Encoding.UTF8.GetString(bytes）解码 你可以用Encoding对应的编码转换成byte，
        /// 例如： string s="连接"; byte[] bytes= Encoding.UTF8.GetBytes(s); C#这样做是有道理的，
        /// 因为不同的编码对应的Byte是不一样的，在消息设计的时候要么约定只使用一种编码（如UTF8）要么在消息头
        /// 用编码页告诉传输方编码，编码页是int32类型的，可以方便的用bytes处理
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        public void SendByteCommand(byte command, DelegateSendDone doneDelegate)
        {
            Task<int> task = Task<int>.Factory.StartNew(() =>
            {
                if (socket.Connected != true)
                    return -1;

                ////var serize = new Dictionary<String, object>();
                ////serize.Add("type", "byte_command");
                ////serize.Add("total_count", "1");
                ////serize.Add("current_index", "0");
                ////serize.Add("data", data);

                ////string json = JsonConvert.SerializeObject(serize);
                ////var buffer = Encoding.UTF8.GetBytes(json);

                byte[] returnBuffer = new byte[DefaultBufferSize];

                try
                {
                    byte[] readyToSend = StreamUnity.CreateByteCommandPackage(command);
                    DebugForm.DMes("package length: " + readyToSend.Length);

                    //sendBuffer[index] = data;
                    socket.Send(readyToSend, 0, DefaultBufferSize, SocketFlags.None);

                    //超过两秒会异常，之后的逻辑处理可以有SOCKET状态检查，或者其他的
                    socket.Receive(returnBuffer);
                    //这里会阻塞两秒
                    //String returnBufferString = Encoding.UTF8.GetString(returnBuffer);

                    //Dictionary<string, string> htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(returnBufferString);
                    //String returnState = htmlAttributes["state"];

                    if (StreamUnity.CheckEchoStatus(returnBuffer))
                        return 1;
                    else
                        return 0;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n From: socket.SendByteCommand code: " + ex.ErrorCode);
                    return 0;
                }

            });

            task.ContinueWith(unit =>
            {
                //if (unit.Result == 1)
                    doneDelegate(ip, unit.Result);
                //else 根据错误码做相应处理
            });
            
        }

        /// <summary>
        /// 发送体使用 TASK 或 THREAD 使用子线程操作，因为完成一次发送后需要等待
        /// 接收方返回确认，这回阻塞线程，并且需要返回结果，
        /// 
        /// IAsyncResult 的获取可能会阻塞线程，使用回调更加合适
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        //public IAsyncResult Send(byte[] data, AsyncCallback callback, object state)
        //{
        //    Task<int> task = Task<int>.Factory.StartNew(() => su.ClientMode(ip, Main.PORT));

        //    task.ContinueWith(unit =>
        //    {
        //        if (unit.Result == 1)
        //            callback(state);
        //        //else 根据错误码做相应处理

        //    });

        //    return -1;
        //}


        /// <summary>
        /// Socket 异步接受数据
        /// 在收到数据后首先进行 包类型 检查，然后在做其他逻辑处理
        /// 设计上希望能够接受文件与BYTE COMMAND 同时进行，要实现
        /// 这个目标可能需要一个辅助类来帮助接收文件的处理
        /// </summary>
        private void BeginReceiveCallback(IAsyncResult ar)
        {
            Socket socket;
            int stream_length;
            try
            {
                socket = ar.AsyncState as Socket;
                stream_length = socket.EndReceive(ar);

                DebugForm.DMes("ip: " + ip + " data count: " + stream_length);

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
                MessageBox.Show("BeginReceiveCallback err ip:" + ip + "message:" + e.Message + "\nthread :" + Thread.CurrentThread);
                SocketConnectionLost(this, this.socket, ip);
                return;
            }

            Unity unity = StreamUnity.UnityComeTransform(StreamBuffer);

            switch(unity.Type)
            {
                case Unity.Package_Type_ByteCommand:

                    Task.Run(() =>
                    {
                        ByteCommand bc = new ByteCommand(socket);
                        bc.Execute(unity.Data, unity.DataExtra);
                    });
                    break;

                default:
                    break;
            }

            socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);           
            
        }

        private void tryBeginReceiveCallback(IAsyncResult ar)
        {
            socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(tryBeginReceiveCallback), socket);
        }

        //
        // Summary:
        //     被动模式。主要用于在独立线程上接收 Socket 传来的数据，连接断开后不自动重连。
        //
        // Parameters:
        //   socket:
        //     要被监测的 Socket
        //
        // Tip:
        //      BeginReceive 是异步执行的，貌似没有必要用 Task.Run 包裹，另外，由于Begin函数执行后立即返回，try块也就立刻终结了，没有机会捕获异常，也就没有存在意义。
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool PassivityMode(Socket socket)
        {
            //if (this.socket != null) this.socket = null;
            this.socket = socket;
            connection_count++;
            AutoReconnect = false;

            ip = socket.RemoteEndPoint.ToString().Split(':')[0];
            port = Convert.ToInt32(socket.RemoteEndPoint.ToString().Split(':')[1]);
            //MessageBox.Show("ClientMode ip: " + ip + " port : " + port);
            DebugForm.DMes("PassivityMode( ip: " + ip + " port : " + port + " )");

            //Task.Run(() =>
            //{
            //    try
            //    {
            socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);
            //        MessageBox.Show("BeginReceive");
            //    }
            //    catch (Exception e)
            //    {
            //        MessageBox.Show("BeginReceive Outside: " + e.Message + " ip: " + socket.RemoteEndPoint.ToString().Split(':')[0]);
            //        SocketConnectionLost(this, socket, socket.RemoteEndPoint.ToString().Split(':')[0]);
            //    }
            //});

            return true;
        }

        public bool ServerMode(int port)//Only one!!!
        {
            if(!ServerRunning)
            {
                this.port = port;
                server_running = true;

                //Thread t = new Thread(new ThreadStart(() =>
                //{
                    //Task.Run(() =>
                    //{
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
                    //});
                    
                //}));
                //t.IsBackground = true;
                //t.Start();

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

        //private void BeginReceiveCallback(IAsyncResult ar)
        //{
        //    Socket socket;
        //    int stream_length;
        //    try
        //    {
        //        socket = ar.AsyncState as Socket;
        //        stream_length = socket.EndReceive(ar);
        //        if (stream_length == 0)
        //        {
        //            SocketConnectionLost(this, this.socket, ip);
        //            DebugForm.DMes("BeginReceiveCallback error ip: " + ip + " EndReceive by zero!");
        //            MessageBox.Show(ip + " EndReceive by zero!");
        //            return;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("BeginReceiveCallback err ip:" + ip + "message:" + e.Message + "\nthread :" + Thread.CurrentThread);
        //        SocketConnectionLost(this, this.socket, ip);
        //        return;
        //    }

        //    //int length_bytes = StreamBuffer.Length;
        //    //byte[] stream = StreamBuffer;

        //    // fixed: head length 32byte; contain : this packet type[4byte],this packet data length[4](contain head),data
        //    // behind is a packet framework :type + length + data  and then depend on type the section data will change
        //    // example: 
        //    //1,type code command : type 1,length 36byte data 4byte , will do command by data code
        //    //2,type cmd command : type 2 length xxbyte data xx-32 byte, will excute command as data is
        //    //3,tyoe transfer file: type 3 ,length xxbyte data xx-32byte ,data are seprated as [new buffer size][packge count][last packge size][file name]...
        //    //and will prepare for receice the new file,when get the second pacekt,check the type,if not 4,we got a error,if yes,then receive file
        //    //4,etc...

        //    int type = Byte2Int(StreamBuffer, 0);
        //    int length = Byte2Int(StreamBuffer, 1);
        //    byte[] data = new byte[length - HEAD_LENGTH];
        //    Array.Copy(StreamBuffer, HEAD_LENGTH, data, 0, data.Length);
        //    MessageBox.Show("stream_length: " + stream_length + " length: " + length);

        //    switch (type)
        //    {
        //        case 100://clasic byte command
        //            Thread t = new Thread(new ThreadStart(() =>
        //            {
        //                ByteCommand bc = new ByteCommand(socket);
        //                bc.Execute(data);
        //            }));
        //            t.IsBackground = true;
        //            t.Start();
        //            break;

        //        default:
        //            break;
        //    }

        //    if (1 < 0)
        //    {
        //        // 现在尽量采用 字符 判断方式
        //        switch (3)
        //        {
        //            case 1://CMD Comand
        //                try
        //                {
        //                    //byte[] temp = new byte[length_bytes - STREAM_HEAD_LENGTH];
        //                    //Array.Copy(stream, 16, temp, 0, temp.Length);
        //                    //String str = System.Text.Encoding.UTF8.GetString(temp).TrimEnd();

        //                    //MessageBox.Show("Start a Process: " + str);
        //                    //System.Diagnostics.Process.Start(str);
        //                }
        //                catch (Exception e)
        //                {
        //                    MessageBox.Show(e.Message);
        //                }

        //                break;

        //            case 2://Prepare file receive
        //                //try
        //                //{
        //                //    if (Transfering == false)
        //                //    {
        //                //        bufferSize = getHead(StreamBuffer, 4);
        //                //        int size = 0;
        //                //        for (int i = STREAM_HEAD_LENGTH + 4; i < stream.Length; i++)//extract the length of file name ,have a question
        //                //        {
        //                //            if (stream[i] == 0)
        //                //                break;
        //                //            size++;
        //                //        }
        //                //        byte[] temp = new byte[size];
        //                //        Array.Copy(stream, (STREAM_HEAD_LENGTH + 4), temp, 0, temp.Length);
        //                //        fileName = System.Text.Encoding.UTF8.GetString(temp).Trim();
        //                //        loopCount = 0;
        //                //        Transfering = true;

        //                //        MessageBox.Show("buffer size: " + bufferSize.ToString() + " bit file name: " + fileName);
        //                //        Thread.Sleep(200);//I really really have no idea why need this
        //                //        MyFileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        //                //        setStreamSize(bufferSize + STREAM_HEAD_LENGTH);
        //                //        LogBuilder.buildLog("file [ " + fileName + " ] start receving.");
        //                //    }

        //                //}
        //                //catch (Exception e)
        //                //{
        //                //    //MessageBox.Show(e.Message);
        //                //    LogBuilder.buildLog("Prepare file receive Error: " + e.Message);
        //                //    Transfering = false;
        //                //}

        //                break;

        //            case 3://file reveiving
        //                //try
        //                //{
        //                //    if (getHead(stream, 1) != loopCount)
        //                //    {
        //                //        MyFileStream.Write(StreamBuffer, STREAM_HEAD_LENGTH, StreamBufferSize - STREAM_HEAD_LENGTH);
        //                //        MyFileStream.Flush(true);//must be true,i don't know why
        //                //    }
        //                //    else
        //                //    {
        //                //        if (getHead(stream, 3) > 0)
        //                //        {
        //                //            MyFileStream.Write(stream, STREAM_HEAD_LENGTH, getHead(stream, 3) - STREAM_HEAD_LENGTH);
        //                //            MyFileStream.Flush(true);
        //                //        }
        //                //        else
        //                //        {
        //                //            MyFileStream.Write(stream, STREAM_HEAD_LENGTH, stream.Length - STREAM_HEAD_LENGTH);
        //                //            MyFileStream.Flush(true);
        //                //        }

        //                //        System.Media.SystemSounds.Hand.Play();
        //                //        LogBuilder.buildLog("Received Successful: " + fileName + "packet count: " + loopCount + "Last packet size: " + getHead(StreamBuffer, 3));
        //                //        MyFileStream.Close();
        //                //        setStreamSize(1024);

        //                //        fileName = "";
        //                //        SocketComHelper.transmitCommand(socket, MK_FLAG_FILE_RECEIVED, null);
        //                //        MessageBox.Show("FILE RECEIVED!");
        //                //        Transfering = false;
        //                //        //////////////SEND FEEDBACK TO SENDER!//////////////////

        //                //    }

        //                //    ++loopCount;
        //                //}
        //                //catch (Exception e)
        //                //{
        //                //    MyFileStream.Close();
        //                //    setStreamSize(1024);
        //                //    Transfering = false;
        //                //    MessageBox.Show("err MyFileStream.Write: " + e.Message);
        //                //    LogBuilder.buildLog("err MyFileStream.Write: " + e.Message);
        //                //}

        //                break;
        //        }
        //    }
        //    //else
        //    //{
        //    //    if (length_bytes == 1024)
        //    //    {
        //    //        //Thread t = new Thread(new ThreadStart(() =>
        //    //        //{
        //    //        //    beControl(stream);
        //    //        //}));
        //    //        //t.IsBackground = true;
        //    //        //t.Start();
        //    //    }

        //    //}

        //    socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(BeginReceiveCallback), socket);

        //}

    }
}
