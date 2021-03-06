﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace ProjectS
{
    public class IpScan
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

        public IpScan()
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
                return buidIpList(IpScan.IP_LIST_TYPE_TARGET, IpTable);
            }
            else if(IpTable.Count() > 1)
            {
                foreach (var ip in IpTable)
                    DebugForm.DMes("IpTale.Count > 1 : " + ip);

                return buidIpList(IpScan.IP_LIST_TYPE_HOST, IpTable);
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// 获取 进程 所在机器 所有 的IP地址
        /// </summary>
        /// <returns></returns>
        public static IpList IpScanProceed()
        {
            System.Net.IPHostEntry myHost = new System.Net.IPHostEntry();
            var list = new List<string>();

            try
            {
                myHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());//得到本地主机的DNS信息
                DebugForm.DMes("HostName: " + myHost.HostName.ToString());
                //DebugForm.DMes("IpScaning ");

                foreach (var iptable in myHost.AddressList)//显示本地主机的IP地址表
                {
                    //DebugForm.DMes("-> " + iptable);

                    if (iptable.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        continue;
                    list.Add(iptable.ToString());
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }

            if (list.Count() == 1)
            {
                DebugForm.DMes("IpTale.Count == 1 : " + list[0]);

                return new IpList(IpList.IP_LIST_TYPE_TARGET, list);
            }
            else if (list.Count() > 1)
            {
                foreach (var ip in list)
                    DebugForm.DMes("IpTale.Count > 1 : " + ip);

                return new IpList(IpList.IP_LIST_TYPE_HOST, list);
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
                case IpScan.IP_LIST_TYPE_HOST:
                    canditateIP = null;
                    canditateIP = hostIp;
                    canditateIP.Insert(0, IpScan.IP_LIST_TYPE_HOST);
                    
                    break;

                case IpScan.IP_LIST_TYPE_TARGET:
                    canditateIP.Add(IpScan.IP_LIST_TYPE_TARGET);

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

        /// <summary>
        /// 根据所给LIST中提供的IP，开启新线程尝试连接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public static void trySoc(List<SocUnity> container, List<String> list, int index, int length)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                for (int i = index; i < index + length; i++)
                {
                    SocUnity su = new SocUnity();

                    Task<int> task = Task<int>.Factory.StartNew(() => su.ClientMode(list[i], Main.PORT));

                    task.ContinueWith(unit =>
                    {
                        if (unit.Result == 1)
                        {
                            lock(container)
                                container.Add(su);
                        }
                        //else 根据错误码做相应处理
                    });
                }
            }));
            t.IsBackground = true;
            t.Start();

            //Task.Run(() =>
            //{
            //    for(int i = index;i<index + length;i++)
            //    {
            //        SocUnity su = new SocUnity();

            //        Task<int> task = Task<int>.Factory.StartNew(() => su.ClientMode(ip, Main.PORT));

            //        task.ContinueWith(unit =>
            //        {
            //            if (unit.Result == 1)
            //                container.Add(su);
            //            //else 根据错误码做相应处理
            //        });

            //    }
            //});
        }

        // Summary:
        //          如果 listType 为 IP_LIST_TYPE_HOST 那么说明本地有一个以上IP，需要挑选一个作为用来遍历目标机器的前缀;
        //          如果 listType 为 IP_LIST_TYPE_TARGET 那么本机只有一个IP，可以直接用来遍历
        /// <summary>
        /// 从 1到254 生成IP清单
        /// </summary>
        /// <param name="hostIp"></param>
        /// <returns></returns>
        public static List<String> BuidIpList(string hostIp, int start = 1, int end = 254)
        {
            var canditateIP = new List<String>();

            string[] list = hostIp.Split('.');//Extract something
            string strIPAddress = list[0] + "." + list[1] + "." + list[2] + ".";

            //int nStrat = Int32.Parse("1");//开始扫描地址 
            //int nEnd = Int32.Parse("254");//终止扫描地址 

            for (int i = start; i <= end; i++)//扫描的操作 
            {
                canditateIP.Add(strIPAddress + i.ToString());
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

    public class IpList
    {
        
        /// <summary>
        /// 预挑选的本地IP，挑选一个作为IP遍历的前缀
        /// </summary>
        public const string IP_LIST_TYPE_HOST = "IP_LIST_TYPE_HOST";
  
        /// <summary>
        /// 已选好的作为被遍历前缀的IP
        /// </summary>
        public const string IP_LIST_TYPE_TARGET = "IP_LIST_TYPE_TARGET";

        private List<string> ip;
        private string type;

        public List<String> Ip { get { return ip; } }
        public string Type { get { return type; } }

        public IpList(string type, List<string> ip)
        {
            this.type = type;
            this.ip = ip;
        }

    }

}
