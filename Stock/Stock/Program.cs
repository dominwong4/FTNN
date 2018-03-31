using Stock.Stock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    class Program
    {
        static public FTNN ftnn;
        static public string tmpPath;
        static void Main(string[] args)
        {
            Debug.LogInfo(WingSYS.info());
            Debug.consol(WingSYS.info());
            tmpPath = Directory.GetCurrentDirectory() + @"\tmp\";
            Console.WriteLine("Directory.GetCurrentDirectory(); = " + Directory.GetCurrentDirectory());
            System.IO.Directory.CreateDirectory(tmpPath);

            ftnn = new FTNN();
            ftnn.init();


            /*Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("192.168.10.212");
            System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAdd, 11111);
            soc.Connect(remoteEP);
            //Start sending stuf..
            byte[] byData = System.Text.Encoding.ASCII.GetBytes("{\"Protocol\":\"1007\",\"ReqParam\":{\"QueryAllSocket\":\"0\"},\"Version\":\"1\"}\r\n");
            soc.Send(byData);
            System.Diagnostics.Debug.WriteLine("----");
            byte[] buffer = new byte[1024];
            int iRx = soc.Receive(buffer);
            char[] chars = new char[iRx];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            System.String recv = new System.String(chars);

            System.Diagnostics.Debug.WriteLine("----");
            System.Diagnostics.Debug.WriteLine(recv);
            System.Diagnostics.Debug.WriteLine("----");
            soc.Disconnect(false);*/

            //Task.Run(() => { Application.Run(new TrackSell()); });
            //Task.Run(() => { Application.Run(new mainForm()); });
            
            Console.WriteLine("BackTest start ...");
            BackTest bt = new BackTest();
            bt.init();
            Console.WriteLine("BackTest done ...");
            Console.ReadKey();
           Application.Run(new chartView01());


        }
        
    }
}
