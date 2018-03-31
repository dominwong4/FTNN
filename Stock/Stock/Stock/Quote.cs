using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stock.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stock.Stock
{
    class Quote
    {
        bool isInit = false;
        public SortedList<DateTime, KLData> KLDataArr;
        public SortedList<DateTime, KLData> KLDataArr_3min;
        string stockno;
        string market;
        public void init(string stockno,int market,bool isRealTime = true)
        {
            
            if(isInit)
            {
                Debug.error("already inited");
                return;
            }
            isInit = true;
            KLDataArr = new SortedList<DateTime, KLData>();
            KLDataArr_3min = new SortedList<DateTime, KLData>();
            this.stockno = stockno;
            this.market = market.ToString();

            if(isRealTime)
                OnConnect();

            
        }




        public void OnConnect()
        {

            Program.ftnn.pustCallback(FTNN.protocol.K线推送,
                 JObject.FromObject(
                        new
                        {
                            Market = market,
                            StockCode = stockno,
                            StockPushType = "11",
                            StockSubType = "11",
                            KLType = "1",
                            RehabType = "1",
                            Num = "500"
                        })
                        , updateKLData);

            Program.ftnn.request(FTNN.protocol.订阅, JObject.FromObject(
                new
                {
                    Market = market,
                    StockCode = stockno,
                    StockSubType = "11"
                }), (o1) =>
                 {
                     if ((int)o1["ErrCode"] != 0)
                     {
                         Debug.error(o1.ToString());
                         isInit = false;
                         return;
                     }
                     Debug.LogInfo("1005 OK !!!" );
                     Program.ftnn.request(FTNN.protocol.设置推送协议, JObject.FromObject(
                        new
                        {
                            Market = market,
                            StockCode = stockno,
                            StockPushType = "11",
                            StockSubType = "11",
                            KLType = "1",
                            RehabType = "1",
                            Num = "1000"
                        }), (o2) =>
                        {
                            if ((int)o1["ErrCode"] != 0)
                            {
                                Debug.error(o1.ToString());
                                isInit = false;
                                return;
                            }
                            Debug.LogInfo("1008 set call back func OK !!!");
                        });
                 });

            Program.ftnn.request(FTNN.protocol.拉取最近1000根K线数据, JObject.FromObject(
             new
             {
                 Num = "1000",
                 Market = market,
                 StockCode = stockno,
                 KLType = "1",
                 RehabType = "1"
             }), updateKLData );
        }

        void updateKLData(JObject o)
        {

            if ((int)o["ErrCode"] != 0)
            {
                Debug.error(o.ToString());
                isInit = false;
                return;
            }
            if ((string)o["RetData"]["StockCode"] != stockno || o["RetData"]["KLDataArr"] == null)
            {
                Debug.error("No date in");
                isInit = false;
                return;
            }

            foreach (JObject arro in o["RetData"]["KLDataArr"])
            {
                try
                {
                    KLData kLData = new KLData();
                    DateTime time =
                    FTNN.cov.str2DateTime((string)arro["Time"]);

                    kLData.Close = FTNN.cov.int2price((int)arro["Close"]);
                    kLData.High = FTNN.cov.int2price((int)arro["High"]);
                    kLData.Low = FTNN.cov.int2price((int)arro["Low"]);
                    kLData.Open = FTNN.cov.int2price((int)arro["Open"]);
                    kLData.Volume = (long)arro["Volume"];
                    kLData.Turnover = (long)arro["Turnover"];
                    kLData.PERatio = (int)arro["PERatio"];
                    kLData.TurnoverRate = (int)arro["TurnoverRate"];
                    kLData.Time = (string)arro["Time"];
                    kLData.sysTime = time;

                    addKLData(ref time, ref kLData, ref KLDataArr);

                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
                // Debug.LogInfo(arro.ToString());



            }
            Task.Run(() => {
                try
                {
                if (OnUpdate != null)
                    OnUpdate.Invoke();

                }
                catch (Exception e)
                {
                    Debug.LogError("Quote update Error \n"+e.ToString());
                }
            });

        }

        void addKLData(ref DateTime time, ref KLData kLData,ref SortedList<DateTime, KLData> KLDataArr)
        {
            if (kLData.Close == 0 && kLData.High == 0 && kLData.Low == 0 && kLData.Open == 0)
                return;

            if (!KLDataArr.ContainsKey(time))
            {
                KLDataArr.Add(time, kLData);
            }
            else
            {
                KLDataArr[time] = kLData;
            }

            updateKDJ(ref time, ref KLDataArr);
            updateDark_Test(ref time, ref KLDataArr);
            add3minKLData(time, kLData);
        }
        void add3minKLData(DateTime time,KLData kLData)
        {
            int step = time.Minute % 3;
            DateTime target_time = time.AddMinutes(-step);

            if(KLDataArr_3min.ContainsKey(target_time))
            {
                KLData okLData = KLDataArr_3min[target_time];
                if(kLData.Low < okLData.Low)
                    okLData.Low = kLData.Low;
                if(kLData.High > okLData.High)
                    okLData.High = kLData.High;
                /*if (step == 0)
                    okLData.Open = kLData.Open;
                if (step == 2)*/
                    okLData.Close = kLData.Close;

                okLData.sysTime = target_time;

            }
            else
            {
                KLDataArr_3min.Add(target_time, kLData);
            }

            updateKDJ(ref target_time,ref KLDataArr_3min);
            updateDark_Test(ref target_time, ref KLDataArr_3min);
        }
        public static DateTime get3minTimebyMin(DateTime time)
        {
            return time.AddMinutes(-(time.Minute % 3));
        }


        void updateKDJ(/*ref KLData kLData,*/ref DateTime time, ref SortedList<DateTime, KLData> KLDataArr)
        {

            KLData kLData = KLDataArr[time];
            const double N = 9, M1 = 3, M2 = 3;

            KDJ kdj = new KDJ();
            double K = 0,D = 0,J = 0,RSV = 0;
            int index = KLDataArr.IndexOfKey(time);
            

            //Update MinMaxPirce
            double min = Double.MaxValue, max = Double.MinValue;
            for (int i= index; i>= 0 && i > index-N; i--)
            {
                //A slow way!!
                //var t = KLDataArr.ElementAt(i).Value;
                var t = KLDataArr.Values[i];
                if (t.Low < min)
                    min = t.Low;
                if (t.High > max)
                    max = t.High;
            }

            if (index == 0 || Double.IsNaN(KLDataArr.Values[index - 1].kdj.K) )
            {
                RSV = (kLData.Close - min) / (max - min) * 100;
                K = (1 * RSV + (M1 - 1) * 0) / 1;
                D = (1 * K + (M2 - 1) * 0) / 1;
                J = 3 * K - 2 * D;
            }
            else
            {
                var okLData = KLDataArr.Values[index - 1];
                RSV = (kLData.Close - min) / (max - min) * 100;
                K = (1 * RSV + (M1 - 1) * okLData.kdj.K) / M1;
                D = (1 * K + (M2 - 1) * okLData.kdj.D) / M2;
                J = 3 * K - 2 * D;

                //For other
                long timeBetween = kLData.sysTime.Ticks-okLData.sysTime.Ticks;

                kdj.crossPoint = Vector2.checkCrossPoint
                    (
                     new Vector2(okLData.kdj.K, 0),
                     new Vector2( K, timeBetween),
                     new Vector2(okLData.kdj.J, 0),
                     new Vector2( J, timeBetween)
                    );

                /*kdj.angle_KJ = 
                    angleBetween(*/




            }

            kdj.K = K;
            kdj.D = D;
            kdj.J = J;
            kdj.RSV = RSV;
            kLData.kdj = kdj;

            KLDataArr[time] = kLData;
        }

        public void updateBOLL(/*ref KLData kLData,*/ref DateTime time, ref SortedList<DateTime, KLData> KLDataArr)
        {
            const double N = 9;
            double MID = 0;
            int index = KLDataArr.IndexOfKey(time);
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                MID += KLDataArr.Values[i].Close;
            }
            MID /= N;

        }
        public void updateDark_Test(/*ref KLData kLData,*/ref DateTime time, ref SortedList<DateTime, KLData> KLDataArr)
        {


            KLData kLData = KLDataArr[time];
            _Dark_Test dt = new _Dark_Test();

            //Update MA





            double N = 50, N_10 = 0, N_20 = 0, N_50 = 0;
            double MID = 0, MID_10 = 0, MID_20 = 0, MID_50 = 0;
            int index = KLDataArr.IndexOfKey(time);
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                if(N_10 < 10)
                {
                    N_10++;
                    MID_10 += KLDataArr.Values[i].Close;
                }
                if (N_20 < 20)
                {
                    N_20++;
                    MID_20 += KLDataArr.Values[i].Close;
                }
                if (N_50 < 50)
                {
                    N_50++;
                    MID_50 += KLDataArr.Values[i].Close;
                }

                MID += KLDataArr.Values[i].Close;
            }
            MID /= N;
            MID_10 /= N_10;
            MID_20 /= N_20;
            MID_50 /= N_50;

            dt.MA_10 = MID_10;
            dt.MA_20 = MID_20;
            dt.MA_50 = MID_50;


            for (int i = index; i >= 0 && i > index - N_20; i--)
            {
                dt.Avg_dt_20 += Math.Abs(KLDataArr.Values[i].Close - MID_20);
            }
            dt.Avg_dt_20 /= N_20;

            for (int i = index; i >= 0 && i > index - N_10; i--)
            {
                dt.Avg_dt_10 += Math.Abs(KLDataArr.Values[i].Close - MID_10);
            }
            dt.Avg_dt_10 /= N_10;


            N = 20;
            MID = 0;
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                MID += KLDataArr.Values[i].Close;
            }
            MID /= N;
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                dt.Avg_dt_20_bug += Math.Abs(KLDataArr.Values[i].Close - MID);
            }
            dt.Avg_dt_20_bug /= 20;


            /*N = 9;
            MID = 0;
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                MID += KLDataArr.Values[i].Close;
            }
            MID /= N;
            for (int i = index; i >= 0 && i > index - N; i--)
            {
                dt.Avg_dt_9 += Math.Abs(KLDataArr.Values[i].Close - MID);
            }
            dt.Avg_dt_9 /= N;*/


            kLData._dark_Test = dt;
            KLDataArr[time] = kLData;

        }

        public void updateHistory(DateTime From, DateTime To)
        {
            bool isDone = false;
            updateHistory(From, To, () => { isDone = true; });
            while (!isDone)
                Thread.Sleep(10);

            Debug.LogInfo("updateHistory done");
        }

        public void updateHistory(DateTime From, DateTime To,Action callback)
        {
            var sql = JObject.FromObject(new
            {
                Market = market,
                StockCode = stockno,
                KLType = "1",
                RehabType = "1",
                MaxKLNum = "0",
                NeedKLData = "",
                start_date = FTNN.cov.dateTime2Str(From),
                end_date = FTNN.cov.dateTime2Str(To)
            });
            
            string filePath = Program.tmpPath  + @"1024\" +tools.MD5Hash(sql.ToString()) +"_"+ market+"_" + stockno + "_" + From.Year + From.Month + From.Day + "_" + To.Year + To.Month + To.Day + ".1024.tmp";
            System.IO.Directory.CreateDirectory(Program.tmpPath + @"1024\");
            Debug.LogInfo("Start update");
            if (File.Exists(filePath))
            {
                using (StreamReader file = File.OpenText(filePath))
                    using(JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject o = (JObject)JToken.ReadFrom(reader);
                        if ((int)o["ErrCode"] != 0)
                        {
                            Debug.error("updateHistory Error \n" + o.ToString());
                            isInit = false;
                            callback();
                            return;
                        }
                        //Debug.info("updateHistory in file " + o.ToString());
                        foreach (JObject arro in o["RetData"]["HistoryKLArr"])
                        {
                            try
                            {
                                KLData kLData = new KLData();
                                DateTime time =
                                FTNN.cov.str2DateTime((string)arro["Time"]);

                                kLData.Close = (long)arro["Close"] / 1000000000;
                                kLData.High = (long)arro["High"] / 1000000000;
                                kLData.Low = (long)arro["Low"] / 1000000000;
                                kLData.Open = (long)arro["Open"] / 1000000000;
                                kLData.Volume = (long)arro["Volume"] / 1000;
                                kLData.Turnover = (long)arro["Turnover"];
                                kLData.PERatio = (int)arro["PERatio"];
                                kLData.TurnoverRate = (int)arro["TurnoverRate"];
                                kLData.Time = (string)arro["Time"];
                                kLData.sysTime = time;

                                addKLData(ref time, ref kLData, ref KLDataArr);

                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e.ToString());
                            }
                            // Debug.LogInfo(arro.ToString());



                        }
                        Debug.LogInfo("updateHistory callback");
                        callback();

                }
            }
            else
            {

            Program.ftnn.request(FTNN.protocol.历史K线,
                sql, (o) => {
                    if ((int)o["ErrCode"] != 0)
                    {
                        Debug.error("updateHistory Error \n"+o.ToString());
                        isInit = false;
                        callback();
                        return;
                    }
                    //Debug.info("updateHistory online " + o.ToString());
                    foreach (JObject arro in o["RetData"]["HistoryKLArr"])
                    {
                        try
                        {
                            KLData kLData = new KLData();
                            DateTime time =
                            FTNN.cov.str2DateTime((string)arro["Time"]);

                            kLData.Close = (long)arro["Close"] / 1000000000;
                            kLData.High = (long)arro["High"] / 1000000000;
                            kLData.Low = (long)arro["Low"] / 1000000000;
                            kLData.Open = (long)arro["Open"] / 1000000000;
                            kLData.Volume = (long)arro["Volume"] / 1000;
                            kLData.Turnover = (long)arro["Turnover"];
                            kLData.PERatio = (int)arro["PERatio"];
                            kLData.TurnoverRate = (int)arro["TurnoverRate"];
                            kLData.Time = (string)arro["Time"];
                            kLData.sysTime = time;

                            addKLData(ref time, ref kLData, ref KLDataArr);

                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.ToString());
                        }
                        // Debug.LogInfo(arro.ToString());



                    }
                    Debug.LogInfo("updateHistory callback");
                    callback();

                    if(To < DateTime.Now.AddDays(-1) && o["RetData"]["HistoryKLArr"] != null)
                        using (StreamWriter file = File.CreateText(filePath))
                            using(JsonTextWriter writer = new JsonTextWriter(file))
                            {
                                o.WriteTo(writer);
                            }



                });
            }
        }

        public Action OnUpdate;


        public struct KLData
        {
            /*KLData()
            {
                Time = null;
                Close = 0;
                High = 0;
                Low = 0;
                Open = 0;
                Volume = 0;
                Turnover = 0;
                PERatio = 0;
                TurnoverRate = 0;
            }
            KLData(string _Time, double _Close)
            {

            }*/
            public string Time;
            public double Close, High, Low, Open;
            public long Volume;
            public long Turnover;
            public int PERatio;
            public int TurnoverRate;
            public KDJ kdj;
            public SAR sar;
            public BOLL boll;
            public _Dark_Test _dark_Test;
            public DateTime sysTime;

            public override string ToString() =>
            $"KLINE,{Time},{Close},{High},{Low},{Open},{Volume},{Turnover},{PERatio},{TurnoverRate},KDJ,{kdj.K},{kdj.D},{kdj.J},{kdj.RSV},{kdj.crossPoint}";

        }
        public struct BOLL
        {
            public double MID, UPPER, LOWER;
        }
        public struct SAR
        {

        }
        public struct KDJ
        {
            public double K, D, J, RSV;//, MinPrice, MaxPrice;

            //For other
            public Vector2 crossPoint;
            public double angle_K, angle_D, angle_J, angle_KJ;

        }
        public struct _Dark_Test
        {
            public double MA_10, MA_20, MA_50;
            public double Avg_dt_20, Avg_dt_20_bug;
            public double Avg_dt_9, Avg_dt_10;
            public double Avg_Max_dt;

        }
        public static double angleBetween(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            return Math.Atan2(y2 - y1, x2 - x1) - Math.Atan2(y4 - y3, x4 - x3);
        }

    }
}
