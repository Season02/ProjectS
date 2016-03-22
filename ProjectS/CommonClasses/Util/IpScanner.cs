using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;

namespace ProjectS
{
    public class ipScan
    {
        public delegate void Some_Event_Handler(object sender, int mode_code);
        public event Some_Event_Handler SomeEvent;

        public delegate void UpdatelistDelegate(string ip, string machine);//Update listview delegate

        private static List<String> IpTable = new List<string>();
        private static Dictionary<String, Socket> SocketPool = new Dictionary<String, Socket>();
        private static List<String> canditateIP = new List<String>();

        public int postbackIndex;

        public ipScan()
        {
            System.Threading.Thread t = new System.Threading.Thread(delegate()
            {
                //ipScanProceed();
            });
            t.IsBackground = true;
            t.Start();
        }

        public static List<String> ipScanProceed()
        {
            System.Net.IPHostEntry myHost = new System.Net.IPHostEntry();
            try
            {
                myHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());//得到本地主机的DNS信息
                MessageBox.Show( "HostName: " + myHost.HostName.ToString() + "\r\n");

                foreach (var iptable in myHost.AddressList)//显示本地主机的IP地址表
                {
                    if (iptable.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        continue;
                    IpTable.Add(iptable.ToString());
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

            if(IpTable.Count() == 1)
            {
                MessageBox.Show(IpTable[0]);
                return ipScanTarget(IpTable[0]);
            }
            else if(IpTable.Count() > 1)
            {
                foreach (var ip in IpTable)
                    MessageBox.Show(ip);
                return canditateIP;
            }

            return canditateIP;

            //selectHost sh = new selectHost(this);
            //sh.IPLIST = ipList;//Transmit candidate iplist to selectHost Window
            //sh.pipe_Event += new selectHost.pipeevent_Handler((Object s, int _index) =>
            //{
            //    postbackIndex = _index;
            //    System.Threading.Thread thScan = new System.Threading.Thread(new System.Threading.ThreadStart(ipScanTarget));
            //    thScan.IsBackground = true;
            //    thScan.Start();
            //});
            //sh.Show();
        }

        private static List<String> ipScanTarget(String host)
        {
            var watch = new Stopwatch();
            watch.Start();

            string[] list = host.Split('.');//Extract something
            string strIPAddress = list[0] + "." + list[1] + "." + list[2] + ".";

            int nStrat = Int32.Parse("1");//开始扫描地址 
            int nEnd = Int32.Parse("254");//终止扫描地址 

            for (int i = nStrat; i <= nEnd; i++)//扫描的操作 
            {
                canditateIP.Add(strIPAddress + i.ToString());
                //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(TryIp), strIPAddress + i.ToString());
            }
            watch.Stop();
            Task.Run(() =>
            {
                //MessageBox.Show("SCAN FIRST LOOP DONE!" + "\r\n" + "Time elapsed: " + watch.Elapsed.TotalSeconds + "Senconds");
            });

            return canditateIP;
            
            //ReScan();            
        }

        //void TryIp(Object str)
        //{
        //    String strScanIPAdd = str as String;

        //    System.Net.IPAddress myScanIP = System.Net.IPAddress.Parse(strScanIPAdd);//转换成IP地址 
        //    //canditatedIP.Add(myScanIP);//record ip
        //    try
        //    {
        //        System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(myScanIP);//址获取 DNS 主机信息。 
        //        string strHostName = myScanHost.HostName.ToString();//获取主机的名 

        //        System.Threading.Thread t = new System.Threading.Thread(delegate()
        //        {
        //            mw.iplistView.Dispatcher.Invoke(uplist, address.AddressFamily.ToString(), strHostName);
        //            //canditatedIP.Remove(System.Net.IPAddress.Parse(_ip));
        //            socket soc = new socket(mw, _ip);
        //            //soc.socket_received_Event += new socket.socket_Event_Handler(mw.received);
        //            mw.personalInfoList.Add(new slaveList(_ip, machine, "NULL", "NULL"));//list
        //            mw.socketCollection.Add(soc);//socket
        //        });
        //        t.Start();
        //    }
        //    catch (Exception error)
        //    {
        //        //System.Windows.MessageBox.Show(error.Message);
        //    }

        //}

        //public void ReScan()
        //{
        //    System.Threading.Thread ttt = new System.Threading.Thread(delegate()
        //    {
        //        while (true)
        //        {
        //            System.Threading.Thread.Sleep(5000);

        //            foreach (System.Net.IPAddress address in canditatedIP)
        //            {
        //                try
        //                {
        //                    System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(address);//址获取 DNS 主机信息。 
        //                    string strHostName = myScanHost.HostName.ToString();//获取主机的名                            

        //                    System.Threading.Thread t = new System.Threading.Thread(delegate()
        //                    {
        //                        //mw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.debugBox.AppendText(address.AddressFamily.ToString() + "->" + strHostName + "\r\n"); }));
        //                        mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(address.AddressFamily.ToString() + "->" + strHostName + "\r\n"); }));
        //                        UpdatelistDelegate uplist = new UpdatelistDelegate((string _ip, string machine) =>
        //                        {
        //                            canditatedIP.Remove(System.Net.IPAddress.Parse(_ip));
        //                            socket soc = new socket(mw, _ip);
        //                            //soc.socket_received_Event += new socket.socket_Event_Handler(mw.received);
        //                            mw.personalInfoList.Add(new slaveList(_ip, machine, "NULL", "NULL"));//list
        //                            mw.socketCollection.Add(soc);//socket                        
        //                        });
        //                        mw.iplistView.Dispatcher.Invoke(uplist, address.AddressFamily.ToString(), strHostName);
        //                    });
        //                    t.Start();
        //                }
        //                catch (Exception error)
        //                {
        //                    //System.Windows.MessageBox.Show(error.Message);
        //                }
        //            }

        //        }
        //    });
        //    ttt.IsBackground = true;
        //    ttt.Start();
        //}

    }
}
