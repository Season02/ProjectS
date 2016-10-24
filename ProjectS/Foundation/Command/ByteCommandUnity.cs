using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProjectS.Foundation.Net;

namespace ProjectS.Foundation.Command
{
    [Serializable]
    public class ByteCommandUnity
    {

        [Serializable]
        public class Command
        {
            /// <summary>
            /// 命令位，保存命令，另外一个是作为扩充，再256个命令已经不够时使用
            /// </summary>
            private byte rightCommand;
            /// <summary>
            /// 扩充时可用类似 switch leftCommand 然后细分的做法
            /// </summary>
            private byte leftCommand;

            private STaskUnity task;

            public byte RightCommand { get { return rightCommand; } }
            public byte LeftCommand { get { return leftCommand; } }

            public STaskUnity Task { get { return task; } }

            public Command(byte command)
            {
                task = new STaskUnity(false);

                leftCommand = command;
            }

            public Command(byte leftCommand, byte rightCommand)
            {
                task = new STaskUnity(false);

                this.rightCommand = rightCommand;
                this.leftCommand = leftCommand;
            }

        }
    }
}
