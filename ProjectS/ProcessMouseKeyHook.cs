using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectS
{
    class ProcessMouseKeyHook
    {
        private MouseKeyHook mkh = null;

        public delegate void KeyDown_Event_Handler(object sender, KeyEventArgs e);
        public event KeyDown_Event_Handler KeyDown_Event;

        public ProcessMouseKeyHook()
        {
            mkh = new MouseKeyHook();

            mkh.mouse = false;
            mkh.keys = true;
            mkh.keyLock = false;
            mkh.KeyDown += new KeyEventHandler(hook_KeyDown);
            startKeyMouseHook();
        }

        public void setKeyMouseLock(bool keyLock, bool mouseLock)
        {
            mkh.keyLock = keyLock;
            mkh.mouse = mouseLock;
        }

        public bool isKeyLocked()
        {
            return mkh.keyLock;
        }

        public void setKeyMouseLock()
        {
            mkh.keyLock = !mkh.keyLock;
            mkh.mouse = !mkh.mouse;
        }

        public void startKeyMouseHook()
        {
            mkh.Start();
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDown_Event(sender, e);
        }


    }

    
    
}
