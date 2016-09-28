using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Threading;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var serize = new Dictionary<string, object>();
            serize.Add("a", "1");
            //serize.Add("b","魂");
            //serize.Add("c", 0x02);

            string json = JsonConvert.SerializeObject(serize);

            var buffer = Encoding.UTF8.GetBytes(json);

            Console.WriteLine(buffer.Length);
            Console.WriteLine(json);

            string _json = @"{
            'href': '/account/login.aspx',
            'target': '_blank力'
            }";

            Dictionary<string, string> htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            //Console.WriteLine(htmlAttributes["a"]);

            //Console.WriteLine(htmlAttributes["b"]);

            //Console.WriteLine(Convert.ToInt32(htmlAttributes["c"]) == 0x02);


            AsyncDemo demo = new AsyncDemo("jiangnii");

            // Execute begin method 
            IAsyncResult ar = demo.BeginRun(null, null);

            // You can do other things here 

            // Use end method to block thread until the operation is complete 
            string demoName = demo.EndRun(ar);
            Console.WriteLine(demoName);


            Console.ReadKey();
        }

    }

    public class AsyncDemo
    {
        public delegate void testDelegate(String arg1, int arg2);

        // Use in asynchronous methods 
        private delegate string runDelegate();

        private string m_Name;
        private runDelegate m_Delegate;

        public AsyncDemo(string name)
        {
            m_Name = name;
            m_Delegate = new runDelegate(Run);
        }

        public void Start(testDelegate del, String arg)
        {
            Task.Run(() =>
            {
                String res = arg + arg;

                del(arg, 0);
            });
        }
        
        /**//// 
            /// Synchronous method 
            /// 
            /// 
        public string Run()
        {
            return "My name is " + m_Name;
        }

        /**//// 
            /// Asynchronous end method 
            /// 
            /// 
            /// 
        public string EndRun(IAsyncResult ar)
        {
            if (ar == null)
                throw new NullReferenceException("Arggument ar can't be null");

            try
            {
                return m_Delegate.EndInvoke(ar);
            }
            catch (Exception e)
            {
                // Hide inside method invoking stack 
                throw e;
            }
        }
        
        /**//// 
            /// Asynchronous begin method 
            /// 
            /// 
            /// 
            /// 
        public IAsyncResult BeginRun(AsyncCallback callBack, Object stateObject)
        {
            try
            {
                return m_Delegate.BeginInvoke(callBack, stateObject);
            }
            catch (Exception e)
            {
                // Hide inside method invoking stack 
                throw e;
            }
        }

        
    }

}
