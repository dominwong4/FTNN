using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sockets;
using Sockets.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stock
{
    class FTNN
    {
        private const int connectionLine = 4; 
        private ClientSocket[] _mySock = new ClientSocket[connectionLine];
        private byte[][] _receiveBuffer = new byte[connectionLine][];
        Encoding[] encodingReader = new Encoding[connectionLine];
        private Dictionary<string, List<callbackEvent>> callBackDictionary = new Dictionary<string, List<callbackEvent>>();
        public void init()
        {
            for(int i = 0; i < connectionLine; i++)
            {
                int index = i;
                try
                {
                    //_mySock[i] = new ClientSocket("192.168.10.212", 11111);
                    
                    _mySock[i] = new ClientSocket("119.29.141.202", 11111);
                    _mySock[i].Connected += (SocketConnectedArgs args) => { OnConnected(args, index); };
                    _mySock[i].Disconnected += (SocketDisconnectedArgs args) => { OnDisconnected(args, index); };
                    _mySock[i].DataReceived += (DataReceivedArgs args) => { OnDataReceived(args, index); };
                    _mySock[i].Connect();
                }
                catch(Exception e)
                {
                    Debug.error(e.ToString());
                }
                _receiveBuffer[i] = new byte[0];
                encodingReader[i] = System.Text.Encoding.UTF8;
                Task.Run(() => ParseLoop(index));
            }
            OnConnect();
        }

        Encoding encoding = System.Text.Encoding.UTF8;
        /*public void request(int protocol, int version, JObject obj, Action<String> respon,bool subscribe = false)
        {
            request(protocol.ToString(), "1", obj, respon, subscribe);
        }*/
        public void request(protocol _protocol, JObject obj, Action<JObject> respon, bool subscribe = false)
        {
            request((int)_protocol, obj, respon, subscribe);
        }
        public void request(int protocol, JObject obj, Action<JObject> respon, bool subscribe = false)
        {
            request(protocol.ToString(), "1", obj, respon, subscribe);
        }
        public void request(string protocol, string version, JObject obj, Action<JObject> respon, bool subscribe = false)
        {
            var requt = new
            {
                Protocol = protocol,
                ReqParam = obj,
                Version = version
            };
            if (!callBackDictionary.ContainsKey(protocol))
            {
                callBackDictionary.Add(protocol, new List<callbackEvent>());
            }

            callbackEvent cbe = new callbackEvent();
            cbe.obj = obj;
            cbe.respon = respon;
            cbe.isSubscribe = subscribe;
            callBackDictionary[protocol].Add(cbe);

            string requt_str = JsonConvert.SerializeObject(requt);
            _mySock[getDataLine(protocol)].Send(encoding.GetBytes(requt_str + "\r\n"));
            Debug.info("Send ["+ getDataLine(protocol) + "] : " + requt_str);
        }
        //return uid
        public string pustCallback(protocol _protocol, JObject obj, Action<JObject> respon)
        {
            return pustCallback(((int)_protocol).ToString(), obj, respon);
        }

        public string pustCallback(string _protocol, JObject obj, Action<JObject> respon)
        {
            string uid = getCookie();
            string protocol = _protocol;
            callbackEvent cbe = new callbackEvent();
            cbe.obj = obj;
            cbe.respon = respon;
            cbe.isSubscribe = true;

            if (!callBackDictionary.ContainsKey(protocol))
                callBackDictionary.Add(protocol, new List<callbackEvent>());

            callBackDictionary[protocol].Add(cbe);

            return uid;
        }
        public void pustCallback(protocol _protocol, Action<JObject> respon,int uid)
        {

        }

        private void OnConnect()
        {

            Program.ftnn.pustCallback("1036",
                 JObject.FromObject(
                        new
                        {
                        })
                        , (o) =>
                        {
                            int SysunixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            int funnTime = (int)o["RetData"]["TimeStamp"];
                            dt_Time = funnTime - SysunixTimestamp;
                            Debug.LogInfo("sysTime : " + SysunixTimestamp + " funnTime : " + funnTime + " >>  dt = " + dt_Time);
                        }
                );
            
        }
        private int dt_Time = 0;
        public DateTime getFTNN_Time()
        {
            return DateTime.Now.AddSeconds(dt_Time);
        }


        private void OnConnected(SocketConnectedArgs args,int index)
        {
            Debug.info("DataLine[" + index + "] OnConnected to FTNN.");
        }
        private void OnDisconnected(SocketDisconnectedArgs args_, int index)
        {
            Debug.error("DataLine[" + index + "] Disconnected to FTNN. Start to reconnect....");

            _mySock[index] = new ClientSocket("119.29.141.202", 11111);
            _mySock[index].Connected += (SocketConnectedArgs args) => { OnConnected(args, index); };
            _mySock[index].Disconnected += (SocketDisconnectedArgs args) => { OnDisconnected(args, index); };
            _mySock[index].DataReceived += (DataReceivedArgs args) => { OnDataReceived(args, index); };
            _mySock[index].Connect();
        }


        private void OnDataReceived(DataReceivedArgs args, int index)
        {
            lock (_receiveBuffer[index])
            {
                if (_receiveBuffer.Length == 0)
                {
                    _receiveBuffer[index] = args.Data;
                    return;
                }
                var tempBuff = new byte[_receiveBuffer[index].Length + args.Data.Length];
                Buffer.BlockCopy(_receiveBuffer[index], 0, tempBuff, 0, _receiveBuffer[index].Length);
                Buffer.BlockCopy(args.Data, 0, tempBuff, _receiveBuffer[index].Length, args.Data.Length);
                _receiveBuffer[index] = tempBuff;
            }
            //Debug.info("Read [" + index + "] : " );
        }
        private void ParseLoop(int index)
        {
            while(true)
            {
                if (_receiveBuffer.Length == 0 || !_mySock[index].IsConnected)
                {
                    Thread.Sleep(1);
                    continue;
                }
                List<string> outputs;
                lock (_receiveBuffer[index])
                {
                    string asAscii = encodingReader[index].GetString(_receiveBuffer[index]);
                    if (!asAscii.Contains("\r\n"))
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    outputs = asAscii.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (!asAscii.EndsWith("\r\n"))
                    {
                        // -- Last message was incomplete
                        _receiveBuffer[index] = encodingReader[index].GetBytes(outputs[outputs.Count - 1]);
                        // -- Place what was there back onto the stack..
                        outputs.RemoveAt(outputs.Count - 1);
                    }
                    else
                    {
                        _receiveBuffer[index] = new byte[0];
                    }
                } // -- We're done handling all this data. we can let the socket use it again :)

                foreach (string output in outputs)
                {
                    //Debug.info("Read : " + output);
                    try
                    {
                        JObject o = JObject.Parse(output);



                        if (o["Protocol"] != null)
                        {
                            Debug.info("Protocol : " + o["Protocol"]);

                            if (!callBackDictionary.ContainsKey((string)o["Protocol"]))
                            {
                                Debug.error("Unexpect repos : " + output);
                            }
                            //For sync time
                            else if((string)o["Protocol"] == "1036")
                            {
                                if (o["ErrCode"] != null && (int)o["ErrCode"] == 0)
                                {
                                    List<callbackEvent> list = callBackDictionary[(string)o["Protocol"]];
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                            list[i].respon(o);
                                    }
                                }else
                                {
                                    Debug.LogError("Protocol 1036 Error, Sync time error");
                                }
                            }
                            else
                            {
                                bool isRespon = false;
                                lock (callBackDictionary)
                                {
                                    List<callbackEvent> list = callBackDictionary[(string)o["Protocol"]];
                                    bool isDone = false;
                                    if(o["ErrCode"] != null && (int)o["ErrCode"] != 0)
                                    {                                  
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            if (!isDone && !list[i].isSubscribe)
                                            {
                                                list[i].respon(o);
                                                list.RemoveAt(i);
                                                isDone = true;
                                                isRespon = true;
                                                i--;
                                            }
                                            else if (list[i].isSubscribe)
                                            {
                                                list[i].respon(o);
                                                isRespon = true;
                                                isDone = true;
                                            }
                                        }
                                    }

                                    else if (o["RetData"]["Cookie"] != null && o["RetData"]["StockCode"] != null)
                                    {
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            JObject r = list[i].obj;
                                            if (r["Cookie"] != null
                                                && r["StockCode"] != null
                                                && (string)r["Cookie"] == (string)o["RetData"]["Cookie"]
                                                 && (string)r["StockCode"] == (string)o["RetData"]["StockCode"]
                                                )
                                            {
                                                if (!isDone && !list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    list.RemoveAt(i);
                                                    isDone = true;
                                                    isRespon = true;
                                                    i--;
                                                }
                                                else if (list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    isRespon = true;
                                                }
                                            }

                                        }
                                    }

                                    else if(o["RetData"]["StockCode"] != null && o["RetData"]["Cookie"] == null)
                                    { 
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            JObject r = list[i].obj;
                                            if (r["StockCode"] != null
                                                && r["Cookie"] == null
                                             && (string)r["StockCode"] == (string)o["RetData"]["StockCode"]
                                            )
                                            {
                                                if (!isDone && !list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    list.RemoveAt(i);
                                                    isDone = true;
                                                    isRespon = true;
                                                    i--;
                                                }
                                                else if (list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    isRespon = true;
                                                }
                                            }

                                        }
                                    }

                                    else if (o["RetData"]["Cookie"] != null && o["RetData"]["StockCode"] == null)
                                    {
                                        for (int i = 0; i < list.Count; i++)
                                        {
                                            JObject r = list[i].obj;
                                            if (r["Cookie"] != null
                                                && r["StockCode"] != null
                                             && (string)r["Cookie"] == (string)o["RetData"]["Cookie"]
                                            )
                                            {
                                                if (!isDone && !list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    list.RemoveAt(i);
                                                    isDone = true;
                                                    isRespon = true;
                                                    i--;
                                                }
                                                else if (list[i].isSubscribe)
                                                {
                                                    list[i].respon(o);
                                                    isRespon = true;
                                                }
                                            }

                                        }
                                    }

                                    callBackDictionary[(string)o["Protocol"]] = list;
                                }
                                if (!isRespon)
                                {
                                    Debug.error("UnMaped respon : " + output);
                                }
                            }
                        }
                        else
                        {
                            Debug.error("Unkonw repos : " + output);
                        }
                    }catch(Exception e)
                    {
                        Debug.error("Error repos : \n" + output + "\n ERROR : \n" + e.ToString() + "\n:\n" + e.Source);
                    }

                    
                }
            }
        }


        private int getDataLine(string protocol)
        {
            if(protocol.Length == 4)
            {
                //行情拉取接口在同一条长连接上。推送数据在第二条长连接上。交易接口在第三条长连接上。
                if(protocol[0] == '1')
                {
                    return (protocol[2] == '3')?1:2;
                }
                else if (protocol[0] == '6' || protocol[0] == '7')
                {

                    return 3;
                    //return (protocol[1] == '2') ? 3 : 4;
                }
            }
            return 0;
        }
        public struct callbackEvent
        {
            public JObject obj;
            public Action<JObject> respon;
            public bool isSubscribe;
            public string uid;
        }

        public enum protocol
        {
            报价                  = 1001,
            订阅                  = 1005,
            设置推送协议          = 1008,

            拉取最近1000根K线数据 = 1011,

            历史K线               = 1024,


            K线推送               = 1032,

            港股查询持仓列表      = 6009
        }

        static Random _Cookie = new Random();
        public static string getCookie()
        {
            return _Cookie.Next(1, 2147483647).ToString();
        }
       
        public class cov
        {
            public static DateTime str2DateTime(string s)
            {
                return DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss",
                                           System.Globalization.CultureInfo.InvariantCulture);
            }
            public static string dateTime2Str(DateTime d)
            {
                return d.ToString("yyyy-MM-dd");
            }
            public static double int2price(int s)
            {
                return ((double)(s)) / 1000.0;
            }

        }





    }
}
