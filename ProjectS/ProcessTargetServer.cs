using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace ProjectS  
{
    //get connect to internet,if failed try connect to local server.whatever which got connection , monitoring it.
    // Connect() -> Monitoring() -> Destroy() -> Connect()...
    class ProcessTargetServer
    {
        private Socket server;//Server socket
        private int server_count;//Count of server

        public delegate void GotConnection_Event_Handler(object sender, Socket server ,int server_count);
        public delegate void LostConnection_Event_Handler(object sender, Socket server, int server_count);

        public event GotConnection_Event_Handler GotConnection_Event;
        public event LostConnection_Event_Handler LostConnection_Event;

        public ProcessTargetServer()
        {
            Thread monitor = new Thread(new ThreadStart(() =>
            {
                
            }));
            monitor.IsBackground = true;
            monitor.Start();
        }

        //try to get connect to server,if server on internet then return 1,if on local net return 2,if failed to get connect return -1;
        private int Connect()
        {
            return -1;
        }
     

    }
}
