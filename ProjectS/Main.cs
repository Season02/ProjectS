using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.ComponentModel;

namespace ProjectS
{
    class Main
    {
        public static int PORT = 4320;
        private List<Socket> sockets;
        
        private ProcessMouseKeyHook pmkh;
        private ProcessSocketMonitor psm;

        private FormMasterMode fmm;
        private DebugForm dform;
        private bool dform_on = false;

        public delegate void MasterMode_Changed_Event_Handler(object sender, int mode_code);
        public static event MasterMode_Changed_Event_Handler MasterModeChanged;

        public const int MASTER_MODE = 1;
        public const int SERVANT_MODE = 2;

        public const int Global_MODE = 1;
        public const int Local_MODE = 2;

        private bool on_master_mode;        

        public delegate void GlobalMode_Changed_Event_Handler(object sender, int mode_code);
        public static event GlobalMode_Changed_Event_Handler GlobalModeChanged;

        private bool on_global_mode;

        public delegate void Debug_Event_Handler(object sender, int mode_code);
        public static event Debug_Event_Handler Debug;

        //Event use to let debug form show or hide
        public delegate void DebugForm_Show_Event_Handler(object sender, bool show);
        public static event DebugForm_Show_Event_Handler DebugFormShow;

        public Main()
        {
            MasterModeChanged += new MasterMode_Changed_Event_Handler(mastermodeevent);

            init();
            execute();
        }

        private void init()
        {
            on_master_mode = TxtIntrop.Judgement("role", "master");
            on_global_mode = TxtIntrop.Judgement("enviromnet", "global");

            pmkh = new ProcessMouseKeyHook();
            pmkh.KeyDown_Event += new ProcessMouseKeyHook.KeyDown_Event_Handler(eventKeyDown);
            psm = new ProcessSocketMonitor();
            //pcommand = new ProcessCommand(psm);

            dform = new DebugForm();
        }

        private void execute()
        {
            MasterModeChanged(this, on_master_mode == true ? MASTER_MODE : SERVANT_MODE);
            GlobalModeChanged(this, on_global_mode == true ? Global_MODE : Local_MODE);

            //dform.Show();
            //run;
        }

        /* ----Hot Keys---- */
        private void eventKeyDown(object sender, KeyEventArgs e)
        {

            //Task.Run(() =>
            //{
                /* ----Hot Keys With Ctrl + Shift---- */
                if ((int)Control.ModifierKeys == ((int)Keys.Control | (int)Keys.Shift))
                {
                    Task.Run(() =>
                    {
                        switch (e.KeyValue)
                        {
                            //TEST MESSAGE
                            case (int)Keys.Q:
                                MessageBox.Show("hi -_-");

                                break;

                            //MOUSE KEY LOCK
                            case (int)Keys.D1:
                                pmkh.setKeyMouseLock();
                                break;

                            //TEST
                            case (int)Keys.D2:
                                Debug(this, 0);
                                break;

                            //EXIT
                            case (int)Keys.D0:
                                //MessageBox.Show("Bye     ≖‿ ≖✧");
                                //System.Media.SystemSounds.Asterisk.Play();
                                System.Environment.Exit(0);
                                //Application.Exit();
                                break;

                            default:
                                break;
                        }
                    });
                    
                }
                /* ----Hot Keys With Ctrl + Alt---- */
                else if ((int)Control.ModifierKeys == ((int)Keys.Control | (int)Keys.Alt))
                {
                    switch (e.KeyValue)
                    {
                        case (int)Keys.Q:
                            MessageBox.Show("Ctrl + Alt + Q");
                            break;
                        //Master Mode!
                        case (int)Keys.D0:
                            if (!on_master_mode)
                            {
                                MasterModeChanged(this, MASTER_MODE);
                            }

                            break;

                        //Global Mode!
                        case (int)Keys.D9:
                            if (psm.On_global_mode)
                            {
                                onLocal();
                            }
                            else
                            {
                                onGlobal();
                            }

                            break;

                        case (int)Keys.D:
                            //if (!dform_on)
                            //{
                            //    dform_on = true;
                            //    dform.Show();
                            //}
                            //else
                            //{
                            //    dform.Hide();
                            //    dform_on = false;
                            //}

                            SelectForm sf = new SelectForm();
                            sf.Show();
                                
                            break;

                        default:
                            break;
                    }
                }
                if (pmkh.isKeyLocked())
                {
                    //LogBuilder.buildLog("Illegality KeyBord input: " + e.KeyData);             
                }

            //});


        }

        private Task test()
        {
            return Task.Run(() => {DebugFormShow(this,true);});
        }

        private void mastermodeevent(object sender, int mode_code)
        {
            switch (mode_code)
            {
                case MASTER_MODE:
                    Thread thread = new Thread(new ThreadStart(() =>
                    {
                        on_master_mode = true;
                        fmm = new FormMasterMode();
                        fmm.ShowDialog();
                        fmm.Dispose();
                        fmm = null;
                        on_master_mode = false;
                        MasterModeChanged(this, SERVANT_MODE);
                    }));
                    thread.IsBackground = true;
                    thread.Start();
                    break;

                default:
                    break;
            }
        }

        private void onGlobal()
        {
            GlobalModeChanged(this, 0);
        }

        private void onLocal()
        {
            GlobalModeChanged(this, 0);
        }

    }
}
