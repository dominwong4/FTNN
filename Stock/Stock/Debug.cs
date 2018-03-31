using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock
{
    class Debug
    {
        static public void LogInfo(string s)
        {
            olog("info : " + s);
        }
        static public void LogError(string s)
        {
            olog("error : " + s);
        }
        static public void LogWarning(string s)
        {
            olog("warning : " + s);
        }
        static public void info(string s)
        {
            olog("info : " + s);
        }
        static public void error(string s)
        {
            olog("error : " + s);
        }
        static public void warning(string s)
        {
            olog("warning : " + s);
        }
        static private void olog(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
           // Console.WriteLine(s);
        }
        static System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"stock.log", true);
        static public void consol(string s)
        {
            //olog("consol : " + s);
            Console.Write(s);
            file.Write(s);
        }
    }
}
