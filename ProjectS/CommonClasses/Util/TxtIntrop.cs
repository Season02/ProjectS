using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProjectS
{
    class TxtIntrop
    {
        private static String PATH = "s.ini";

        public static String[] getAllLine(String file)
        {
            return File.ReadAllLines(file);
        }

        public static void getAllMap()
        {

        }

        //NO CHINESE CODE
        public static String getValueByKey(String file, String key)
        {
            try
            {
                String[] lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    String[] pair = line.Split(new char[] { '=' });
                    if (pair[0].ToLower().Trim().Equals(key.ToLower().Trim()))
                        return pair[1].ToLower().Trim().Equals("") ? "---" : pair[1].ToLower().Trim();
                }
                return "---";
            }
            catch(Exception e)
            {
                return "---";
            }
        }

        public static bool Judgement(String file, String key, String value)
        {
            return getValueByKey(file, key).Equals(value.Trim().ToLower());
        }

        //KEY=VALUE
        public static bool Judgement(String key, String value)
        {
            return getValueByKey(PATH, key).Equals(value.Trim().ToLower());
        }

    }
}
