using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;

namespace ProjectS
{
    class ProcessCommand
    {
        const int STREAM_HEAD_LENGTH = 16;

        private static long StreamBufferSize = 1024;
        private byte[] StreamBuffer = new byte[StreamBufferSize];  
        private Dictionary<String, Socket> SocketPool = new Dictionary<String, Socket>();

        public delegate void SocketPoolUpdate_Event_Handler(object sender, int PoolSize, String CurrentIp, bool Increase);
        public static event SocketPoolUpdate_Event_Handler SocketPoolUpdate;

        private bool on_master_mode;

        public ProcessCommand(ProcessSocketMonitor psm)
        {
            psm.GotNewSocket += new ProcessSocketMonitor.GotNewSocket_Event_Handler(GetNewSocket);
            psm.SocketCleanup += new ProcessSocketMonitor.SocketCleanup_Event_Handler(SocketCleanup);
            Main.Debug += new Main.Debug_Event_Handler(Debug);
            Main.MasterModeChanged += new Main.MasterMode_Changed_Event_Handler(mastermodeevent);
        }

        private void mastermodeevent(object sender, int mode_code)
        {
            switch (mode_code)
            {
                case Main.MASTER_MODE:
                    on_master_mode = true;
                    break;

                case Main.SERVANT_MODE:
                    on_master_mode = false;
                    break;

                default:
                    break;
            }
        }

        private void SocketCleanup(object sender,int mode)
        {
            try
            {
                SocketPool.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Debug(object sender, int code)
        {
            try
            {
                Task.Run(() =>
                {
                    MessageBox.Show(SocketPool.Count().ToString());
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void GetNewSocket(object sender, Socket socket, String NewSocketIp)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    SocketPool.Add(NewSocketIp, socket);
                    SocketPoolUpdate(this, SocketPool.Count(), socket.RemoteEndPoint.ToString(), true);
                    Process(socket);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }));
            t.IsBackground = true;
            t.Start();            
        }

        private void Process(Socket socket)
        {
            try
            {
                socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(passStream), socket);
            }
            catch (Exception e)
            {
                MessageBox.Show("BeginReceive Outside: " + e.Message + " ip: " + socket.RemoteEndPoint.ToString());
                Terminate(socket.RemoteEndPoint.ToString());
            }
        }

        void setStreamSize(int size)
        {
            StreamBuffer = new byte[size];
            StreamBufferSize = size;
        }

        private void Terminate(String Socket)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Socket socket = SocketPool[Socket];
                    SocketPool.Remove(Socket);
                    SocketPoolUpdate(this, SocketPool.Count(), Socket, false);
                    socket.Close();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }));
            thread.Start();
        }

        public static int getHead(byte[] array, int index)
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

        String fileName;
        bool Transfering = false;
        private async void passStream(IAsyncResult ar)
        {
            Socket socket = null;
            try
            {
                socket = ar.AsyncState as Socket;
                MessageBox.Show( "passStream from: " + socket.RemoteEndPoint.ToString());
                //length = socket.EndReceive(ar);
                if (socket.EndReceive(ar) == 0)
                {
                    Terminate(socket.RemoteEndPoint.ToString());
                    MessageBox.Show("get zero");
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("pass err " + e.Message);

                try
                {
                    Terminate(socket.RemoteEndPoint.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                return;
            }

            int length_bytes = StreamBuffer.Length;
            byte[] stream = StreamBuffer;

            if (getHead(StreamBuffer, 0) > 0 && getHead(StreamBuffer, 1) > 0)//take care the becontrol list///////////////////////////////////////////////////////////////////////////////////////////////////
            {
                switch (getHead(StreamBuffer, 0))
                {
                    case 1://CMD Comand
                        try
                        {
                            byte[] temp = new byte[length_bytes - STREAM_HEAD_LENGTH];
                            Array.Copy(stream, 16, temp, 0, temp.Length);
                            String str = System.Text.Encoding.UTF8.GetString(temp).TrimEnd();

                            MessageBox.Show("Start a Process: " + str);
                            System.Diagnostics.Process.Start(str);
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
            else
            {
                if (length_bytes == 1024)
                {
                    //Thread t = new Thread(new ThreadStart(() =>
                    //{
                    //    beControl(stream);
                    //}));
                    //t.IsBackground = true;
                    //t.Start();
                }

            }

            try
            {
                socket.BeginReceive(StreamBuffer, 0, StreamBuffer.Length, SocketFlags.None, new AsyncCallback(passStream), socket);
            }
            catch (Exception e)
            {
                MessageBox.Show("BeginReceive Inside: " + e.Message);
            }


        }



    }
}
