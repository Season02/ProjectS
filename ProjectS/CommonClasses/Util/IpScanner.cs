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
        // Summary:
        //          预挑选的本地IP，挑选一个作为IP遍历的前缀
        public const String IP_LIST_TYPE_HOST = "IP_LIST_TYPE_HOST";
        // Summary:
        //          已选好的作为被遍历前缀的IP
        public const String IP_LIST_TYPE_TARGET = "IP_LIST_TYPE_TARGET";

        public delegate void Some_Event_Handler(object sender, int mode_code);

        public delegate void UpdatelistDelegate(string ip, string machine);//Update listview delegate

        private static List<String> IpTable = new List<string>();
        private static Dictionary<String, Socket> SocketPool = new Dictionary<String, Socket>();
        //private static List<String> canditateIP = new List<String>();

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
                DebugForm.DMes("HostName: " + myHost.HostName.ToString());
                //MessageBox.Show( "HostName: " + myHost.HostName.ToString() + "\r\n");

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
                DebugForm.DMes("IpTale.Count == 1 : " + IpTable[0]);
                return buidIpList(ipScan.IP_LIST_TYPE_TARGET, IpTable);
            }
            else if(IpTable.Count() > 1)
            {
                foreach (var ip in IpTable)
                    DebugForm.DMes("IpTale.Count > 1 : " + ip);

                return buidIpList(ipScan.IP_LIST_TYPE_HOST, IpTable);
            }
            else
            {
                return null;
            }
            
        }

        public static List<String> cutHead(List<String> list)
        {
            list.RemoveAt(0);
            return list;
        }

        // Summary:
        //          如果 listType 为 IP_LIST_TYPE_HOST 那么说明本地有一个以上IP，需要挑选一个作为用来遍历目标机器的前缀;
        //          如果 listType 为 IP_LIST_TYPE_TARGET 那么本机只有一个IP，可以直接用来遍历
        public static List<String> buidIpList(String listType, List<String> hostIp)
        {
            List<String> canditateIP = new List<String>();

            //check validation
            switch(listType)
            {
                case ipScan.IP_LIST_TYPE_HOST:
                    canditateIP = null;
                    canditateIP = hostIp;
                    canditateIP.Insert(0, ipScan.IP_LIST_TYPE_HOST);
                    
                    break;

                case ipScan.IP_LIST_TYPE_TARGET:
                    canditateIP.Add(ipScan.IP_LIST_TYPE_TARGET);

                    string[] list = hostIp[0].Split('.');//Extract something
                    string strIPAddress = list[0] + "." + list[1] + "." + list[2] + ".";

                    int nStrat = Int32.Parse("1");//开始扫描地址 
                    int nEnd = Int32.Parse("254");//终止扫描地址 

                    for (int i = nStrat; i <= nEnd; i++)//扫描的操作 
                    {
                        canditateIP.Add(strIPAddress + i.ToString());
                    }

                    break;

                default:
                    return null;
            }

            return canditateIP;
        }

        private static List<String> ipScanTarget(String host)
        {
            List<String> canditateIP = new List<String>();

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
