﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Threading.Tasks;
 
namespace ProjectS
{
    class LogBuilder
    {
        private System.Timers.Timer logTimer = new System.Timers.Timer(1000);//Timer for log
        private static List<String> buffer = new List<String>();//buffer for reade to write to log file
        private System.IO.FileStream fs;
        private System.IO.StreamWriter sw;
        public const String LogFile = @"d:\SystemLog.txt";

        private static readonly LogBuilder instance = new LogBuilder();//CLR will deal with the multy thread problem

        private LogBuilder()
        {
            init();
        }

        public static LogBuilder getBuilder()
        {
            return instance;
        }

        public void addLog(String log)
        {
            buffer.Add(log);
        }

        public void emptyLog()
        {
            logTimer.Stop();
            try
            {
                if (System.IO.File.Exists(LogFile))
                {
                    System.IO.File.Delete(LogFile);
                }
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show(ex.Message);
            }
            logTimer.Start();
        }

        public static void buildLog(String str)
        {
            //try
            //{
            //    lock(buffer)
            //    {
            //        getBuilder().addLog(str);
            //    }           
            //}
            //catch (Exception e)
            //{
            //    //MessageBox.Show("Building log err: " + e.Message);
            //}
            
        }

        private void init()
        {
            logTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
            {
                try
                {
                    fs = new System.IO.FileStream(LogFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                    sw = new System.IO.StreamWriter(fs);
                    sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                    lock(buffer)
                    {
                        foreach(String str in buffer)
                        {
                            sw.Write("[" + DateTime.Now.ToString() + "] ");
                            //sw.Flush();
                            sw.WriteLine(str);
                            sw.Flush();
                        }
                        buffer.Clear();
                    }                   

                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });
            logTimer.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            logTimer.Enabled = true; //是否触发Elapsed事件
            logTimer.Start();
        }

    }
}
