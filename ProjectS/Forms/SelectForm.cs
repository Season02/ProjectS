﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectS
{
    public partial class SelectForm : Form
    {
        public int index = -1;

        private System.Timers.Timer autoSeclector;
        private int loops;

        public SelectForm()
        {
            InitializeComponent();
            
            InitListView();
            InitAutoSelector();

            //var data = new List<String>();
            //data.Add("192.168.3.100");
            //data.Add("192.168.3.103");
            //data.Add("192.168.3.105");

            //AddData(data);
        }

        private void InitAutoSelector(int loops = 3, double intervsl = 400)
        {
            this.loops = loops;
            //CountDownLb.Text = lb + "3";
            //CountDownLb.Invoke(new Action(delegate () { CountDownLb.Text = lb + "3"; }));

            autoSeclector = new System.Timers.Timer(intervsl);
            autoSeclector.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => AutoSelectorFunc(s, e));
            autoSeclector.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true)
            autoSeclector.Enabled = true; //是否触发Elapsed事件    
        }

        private void AutoSelectorFunc(Object Sender, EventArgs e)
        {
            if(loops-- > 0)
            {
                CountDownLb.Invoke(new Action(delegate () { CountDownLb.Text = "--" + loops + "--";}));
            }
            else
            {
                autoSeclector.Enabled = false;

                selection_lv.Invoke(new Action(delegate () 
                {
                    foreach (ListViewItem item in selection_lv.Items)
                    {
                        if(item.Text.Contains("192.168"))
                        {
                            item.Selected = true;
                       
                            ProcessSocketMonitor.public_index = item.Index;
                            this.Close();

                            return;
                        }

                    }
                }));
                
            }
        }

        public void AddData(List<String> data)
        {
            selection_lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

            foreach (var item in data)
            {
                var lvi = new ListViewItem();
                lvi.Text = item;
                //lvi.SubItems.Add(item);
                selection_lv.Items.Add(lvi);
            }
            
            selection_lv.Items[selection_lv.Items.Count - 1].EnsureVisible();//滚动到最后
            selection_lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        private void InitListView()
        {
            selection_lv.Columns[0].Width = selection_lv.Width;
            selection_lv.Columns[1].Width = 0;
            selection_lv.Columns[0].TextAlign = HorizontalAlignment.Center;
        }

        private void repaint()
        {
            selection_lv.Columns[0].Width = selection_lv.Width;
        }

        private void SelectForm_Resize(object sender, EventArgs e)
        {
            repaint();
        }

        private void UpdataList(String data)
        {
            var lvi = new ListViewItem();
            //lvi.Text = data;
            lvi.SubItems.Add(data);
            selection_lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
                                          //if (increase)
                                          //{
            selection_lv.Items.Add(lvi);
            selection_lv.Items[selection_lv.Items.Count - 1].EnsureVisible();//滚动到最后
                                                                             //ServantLv.Items[1].SubItems[0].Text = "";
                                                                             //}
                                                                             //else
                                                                             //{
                                                                             //    if (selection_lv.Items.Count > 0 && ip.Trim() != string.Empty)
                                                                             //    {
                                                                             //        foreach (ListViewItem item in selection_lv.Items)
                                                                             //        {
                                                                             //            if (item.Text.Equals(ip))
                                                                             //            {
                                                                             //                item.SubItems[0].Text = status;
                                                                             //                break;
                                                                             //            }
                                                                             //        }
                                                                             //    }
                                                                             //}

            selection_lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }

        private void selection_lv_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if(selection_lv.SelectedItems.Count > 0)
            {
                ProcessSocketMonitor.public_index = selection_lv.SelectedItems[0].Index;
                //MessageBox.Show(selection_lv.SelectedItems[0].Index + "");
            }
        }

        private void select_bt_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void selection_lv_DoubleClick(object sender, EventArgs e)
        {
            if (selection_lv.SelectedItems.Count > 0)
            {
                ProcessSocketMonitor.public_index = selection_lv.SelectedItems[0].Index;
                //MessageBox.Show(selection_lv.SelectedItems[0].Index + "");
                this.Close();
            }
        }

        private void selection_lv_MouseUp(object sender, MouseEventArgs e)
        {
            autoSeclector.Enabled = false;
        }
    }
}
