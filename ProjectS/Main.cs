using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;

namespace ProjectS
{
    class Main
    {
        public static int PORT = 4320;
        private List<Socket> sockets;
        
        private ProcessMouseKeyHook pmkh;
        private ProcessSocketMonitor psm;
        private ProcessCommand pcommand;

        private FormMasterMode fmm;
        private DebugForm dform;

        public delegate void MasterMode_Changed_Event_Handler(object sender, int mode_code);
        public static event MasterMode_Changed_Event_Handler MasterModeChanged;

        public const int MASTER_MODE = 1;
        public const int SERVANT_MODE = 2;

        public const int Global_MODE = 1;
        public const int Local_MODE = 2;

        private bool on_master_mode;        

        public delegate void GlobalMode_Changed_Event_Handler(object sender, int mode_code);
        public event GlobalMode_Changed_Event_Handler GlobalModeChanged;

        private bool on_global_mode;

        public delegate void Debug_Event_Handler(object sender, int mode_code);
        public static event Debug_Event_Handler Debug;

        public Main()
        {
            MasterModeChanged += new MasterMode_Changed_Event_Handler(mastermodeevent);

            init();
            execute();
        }

        private void execute()
        {
            MasterModeChanged(this, on_master_mode == true? MASTER_MODE:SERVANT_MODE);
            GlobalModeChanged(this, on_global_mode == true? Global_MODE:Local_MODE);

            dform.Show();
            //run;
        }

        private void init()
        {
            on_master_mode = TxtIntrop.Judgement("role", "master");
            on_global_mode = TxtIntrop.Judgement("enviromnet", "global");

            pmkh = new ProcessMouseKeyHook();
            pmkh.KeyDown_Event += new ProcessMouseKeyHook.KeyDown_Event_Handler(eventKeyDown);
            psm = new ProcessSocketMonitor(this);
            pcommand = new ProcessCommand(psm);

            dform = new DebugForm();
        }

        private void eventKeyDown(object sender, KeyEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                if ((int)Control.ModifierKeys == ((int)Keys.Control | (int)Keys.Shift))
                {
                    switch (e.KeyValue)
                    {
                        //TEST MESSAGE
                        case (int)Keys.Q:                    
                            System.Media.SystemSounds.Asterisk.Play();
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
                            System.Media.SystemSounds.Asterisk.Play();
                            System.Environment.Exit(0);
                            //Application.Exit();
                            break;

                        default:
                            break;
                    }

                }
                else if ((int)Control.ModifierKeys == ((int)Keys.Control | (int)Keys.Alt))
                {
                    switch (e.KeyValue)
                    {
                        //Master Mode!
                        case (int)Keys.D0:
                            if(!on_master_mode)
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

                        default:
                            break;
                    }
                }
                if (pmkh.isKeyLocked())
                {
                    //LogBuilder.buildLog("Illegality KeyBord input: " + e.KeyData);             
                }
            }));
            thread.IsBackground = true;
            thread.Start();
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
