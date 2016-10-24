using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectS.Foundation.Net
{
    [Serializable]
    public class STaskUnity
    {
        public STaskUnity(bool status)
        {
            random();
            taskType = Task.StatusTask;
            this.status = status;
        }

        public STaskUnity(int value)
        {
            random();
            taskType = Task.ProgressTask;

            if (value > 100)
                progress = 100;
            else if (value < 0)
                progress = 0;
            else
                progress = value;
        }

        private void random()
        {
            identifier = Guid.NewGuid().ToString();
        }

        [Serializable]
        public enum Task : byte  //显示指定枚举的底层数据类型
        {
            ProgressTask,
            StatusTask
        }

        private Task taskType;

        public Task TaskType { set { taskType = value; } get { return taskType; } }

        private bool status = false;
        private int progress = 0;

        private string identifier;

        public string Identifier { get { return identifier; } }

        public bool Status
        {
            set
            {
                if (taskType == Task.StatusTask)
                    status = value;
                else
                    status = false;
            }
            get { return status; }
        }

        public int Progress
        {
            set
            {
                if(taskType == Task.ProgressTask)
                {
                    if (value > 100)
                        progress = 100;
                    else if (value < 0)
                        progress = 0;
                    else
                        progress = value;
                }
                else
                {
                    progress = 0;
                }
                
            }

            get
            {
                return progress;
            }
        }


    }
}
