using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Stock.Stock.Quote;

namespace Stock.Stock
{
    class BackTest
    {
        Quote quote;
        DateTime currDate;
        public void init()
        {
            Thread.Sleep(100);
            quote = new Quote();
            quote.init("800000", 1, false);

            tranList = new List<Tran>();
            tranList_closed = new List<Tran>();
            //quote.updateHistory(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));
            DateTime lastStockDate;
            for (
            //DateTime currDate = new DateTime(2017, 02, 01);
            currDate = new DateTime(2018,3, 1, 09, 30, 00), lastStockDate = currDate;
            currDate < DateTime.Now;
            currDate = currDate.AddDays(1)
            )
            {
                if ((currDate.DayOfWeek == DayOfWeek.Saturday || currDate.DayOfWeek == DayOfWeek.Sunday))
                    continue;

                quote.KLDataArr.Clear();
                quote.KLDataArr_3min.Clear();
                Debug.consol("BT:" + currDate.ToShortDateString()+". updateHistory ");
                //quote.updateHistory(lastStockDate, currDate);
                quote.updateHistory(lastStockDate, currDate);
                if (quote.KLDataArr.ContainsKey(currDate))
                {
                    lastStockDate = currDate;
                }
                else
                {
                    Debug.consol("Done. Today Close.. Skip \n");
                    continue;
                }

                Debug.consol("Done. Looper ");
                looper();
                Debug.consol("Done. dayend ");
                dayend();
                Debug.consol("Done. \n");
            }



            //result_output();
        }

        public void looper()
        {
            var start = currDate;
            var end = currDate.AddDays(1);
            var KLDataArr = quote.KLDataArr.SkipWhile(dt => dt.Key < start)
            .TakeWhile(dt => dt.Key <= end);

            bool isOverSellOrBuy = false, oversell_3m = false, overbuy_3m= false;

            int Beforepoint_3m = 0;
            int Beforepoint_1m = 0;


            double max_kj_between = 0;
            double Before_max_kj_between = 0;
            foreach (var KLData in KLDataArr)
            {
                settlement(KLData.Value);




                var KLData_1m = KLData.Value;
                var KLData_3m = quote.KLDataArr_3min[Quote.get3minTimebyMin(KLData.Key)];



                if (KLData_3m.kdj.J < 0 || KLData_3m.kdj.J > 100)
                    isOverSellOrBuy = true;


                if (KLData_3m.kdj.J < 0)
                    oversell_3m = true;

                if (KLData_3m.kdj.J > 100)
                    overbuy_3m = true;




                if (KLData_1m.sysTime.Minute %3 == 2/*Time == KLData_3m.Time*/)
                {

                    if (KLData_3m.kdj.crossPoint.Y != 0)
                    {
                        if(KLData_3m.kdj.J > KLData_3m.kdj.K)
                        {
                            //if(KLData_3m.kdj.K > 80)
                            //if (isOverSellOrBuy)
                            //if (/*isOverSellOrBuy &&*//*&& KLData.Value.kdj.J > KLData.Value.kdj.K*/ //&& Beforepoint_1m > 3) 
                            // if (isOverSellOrBuy)
                            if  (/*max_kj_between > 5 &&*/ KLData_3m._dark_Test.Avg_dt_20 >30
                                
                                )
                            { 
                            buy(1, KLData_3m, isOverSellOrBuy.ToString() + "," + max_kj_between + 
                                ",DarkTest," + Math.Abs(KLData_3m._dark_Test.MA_50 - KLData_3m._dark_Test.MA_10)
                                /* + "," + KLData_3m._dark_Test.Avg_dt_20_bug
                                 + "," + KLData_3m._dark_Test.Avg_dt_20
                                 + "," + KLData_3m._dark_Test.Avg_dt_10
                                 + "," + KLData_3m._dark_Test.MA_10
                                 + "," + KLData_3m._dark_Test.MA_20
                                 + "," + KLData_3m._dark_Test.MA_50
                                 + "," + max_kj_between
                                  */
                                );
                            }
                            //if(KLData_3m._dark_Test.MA_10 < KLData_3m._dark_Test.MA_20 )

                            //else if(KLData_3m._dark_Test.Avg_dt < 20 && Math.Abs(KLData_3m.kdj.K - KLData_3m.kdj.J) < 15)
                            //     buy(1, KLData_3m, isOverSellOrBuy.ToString() + "," + max_kj_between + ",DarkTest," + KLData_3m._dark_Test.Avg_dt);


                            isOverSellOrBuy = false;
                        }
                        else
                        {
                            //if (KLData_3m.kdj.K < 20)
                            //if (isOverSellOrBuy)
                            //if (KLData.Value.kdj.J < KLData.Value.kdj.K)
                            //if (/*isOverSellOrBuy &&*/// && Beforepoint_1m > 3/*&& KLData.Value.kdj.J < KLData.Value.kdj.K*/) 
                            // if (isOverSellOrBuy)
                            if (/*max_kj_between > 5 && */KLData_3m._dark_Test.Avg_dt_20 > 30)
                            { 
                            buy(-1, KLData_3m, isOverSellOrBuy.ToString() + "," + max_kj_between + 
                                ",DarkTest," + Math.Abs(KLData_3m._dark_Test.MA_50 - KLData_3m._dark_Test.MA_10)
                                /* + "," + KLData_3m._dark_Test.Avg_dt_20_bug
                                 + "," + KLData_3m._dark_Test.Avg_dt_20
                                 + "," + KLData_3m._dark_Test.Avg_dt_10
                                 + "," + KLData_3m._dark_Test.MA_10
                                 + "," + KLData_3m._dark_Test.MA_20
                                 + "," + KLData_3m._dark_Test.MA_50
                                 + "," + max_kj_between
                                  */
                                );
                            }
                            //if (KLData_3m._dark_Test.MA_10 > KLData_3m._dark_Test.MA_20)

                            //else if (KLData_3m._dark_Test.Avg_dt < 20 && Math.Abs(KLData_3m.kdj.K - KLData_3m.kdj.J) < 15)
                            //    buy(-1, KLData_3m, isOverSellOrBuy.ToString() + "," + max_kj_between + ",DarkTest," + KLData_3m._dark_Test.Avg_dt);

                            isOverSellOrBuy = false;
                        }
                        Beforepoint_3m = 0;
                        Before_max_kj_between = max_kj_between;
                        max_kj_between = 0;
                        
                    }
                    else
                    {
                        Beforepoint_3m++;
                        if (Math.Abs(KLData_3m.kdj.J - KLData_3m.kdj.K) > max_kj_between)
                            max_kj_between = Math.Abs(KLData_3m.kdj.J - KLData_3m.kdj.K);
                    }




                }
                
                /*if((oversell_3m || overbuy_3m) && KLData_3m.kdj.J >10 && KLData_3m.kdj.J < 90)
                {
                    if (overbuy_3m)
                        buy(-1, KLData_3m, isOverSellOrBuy.ToString() + "," + Beforepoint_3m);
                    if (oversell_3m)
                        buy(1, KLData_3m, isOverSellOrBuy.ToString() + "," + Beforepoint_3m);

                    oversell_3m = false;
                    overbuy_3m = false;

                }*/



                if (KLData_1m.kdj.crossPoint.Y != 0)
                    if (KLData_1m.kdj.J > KLData_1m.kdj.K)
                    {
                        //if(KLData_3m.kdj.K > 20 && KLData_3m.kdj.K <80)

                        /* if (isOverSellOrBuy&& Beforepoint_1m > 5)*/
                        if (KLData_1m._dark_Test.Avg_dt_20 > 20) ;
                            //buy(1, KLData_1m, isOverSellOrBuy.ToString()+","+ Beforepoint_1m);

                        isOverSellOrBuy = false;
                        Beforepoint_1m = 0;
                    }
                    else
                    {
                        //if (KLData_3m.kdj.K > 20 && KLData_3m.kdj.K < 80)
                        //if (isOverSellOrBuy)
                        /*if (isOverSellOrBuy && Beforepoint_1m > 5)*/
                        if (KLData_1m._dark_Test.Avg_dt_20 > 20) ;
                           // buy(-1, KLData_1m, isOverSellOrBuy.ToString() + "," + Beforepoint_1m);
                        isOverSellOrBuy = false;
                        Beforepoint_1m = 0;
                    }
                else
                    Beforepoint_1m++;


                    /*if (KLData.Value.kdj.J < 0 || KLData.Value.kdj.J > 100)
                        isOverSellOrBuy = true;
                    if (KLData.Value.kdj.crossPoint.Y != 0)
                        if (KLData.Value.kdj.J > KLData.Value.kdj.K)
                        {
                            //if(KLData_3m.kdj.K > 20 && KLData_3m.kdj.K <80)

                            if (isOverSellOrBuy)
                                buy(1, KLData.Value);
                            isOverSellOrBuy = false;
                        }
                        else
                        {
                            //if (KLData_3m.kdj.K > 20 && KLData_3m.kdj.K < 80)
                            if (isOverSellOrBuy)
                                buy(-1, KLData.Value);
                            isOverSellOrBuy = false;
                        }
                        */

                    /*if (KLData.Value.kdj.crossPoint.Y != 0)
                        buy(KLData_3m.kdj.J > KLData_3m.kdj.K ? 1 : -1, KLData.Value);*/

            }
        }

        void settlement( KLData kLData)
        {
            for(int i = 0; i < tranList.Count;i++)
            {
                Tran t = tranList[i];
                if (t.isClose)
                    continue;
                /*if (false)
                    if (kLData.kdj.crossPoint.Y != 0)
                    {
                        t.skLData = kLData;
                        t.sellPrice = kLData.Close;

                        t.result = (t.orientation > 0 )?( t.sellPrice>t.buyPrice?"Win":"Lost"   ): (t.sellPrice < t.buyPrice ? "Win" : "Lost");

                        t.isClose = true;
                    }*/

                if(t.orientation > 0)
                {
                    if(kLData.Low < t.buyPrice - (Scale * cutOffPoint))
                    {
                        t.isClose = true;
                        lost++;
                        t.sellPrice = kLData.Low;
                        t.result = "Lost";
                        t.skLData = kLData;
                    }
                    else if (t.buyPrice + Scale * (1 + cutSellPoint) < kLData.High)
                    {
                        t.isClose = true;
                        win++;
                        t.sellPrice = kLData.High;
                        t.result = "Win";
                        t.skLData = kLData;
                    }
                }
                else if(t.orientation < 0)
                {
                    if (kLData.High > t.buyPrice + (Scale * cutOffPoint))
                    {
                        t.isClose = true;
                        lost++;
                        t.sellPrice = kLData.High;
                        t.result = "Lost";
                        t.skLData = kLData;
                    }
                    else if (t.buyPrice - Scale * (1 + cutSellPoint) > kLData.Low)
                    {
                        t.isClose = true;
                        win++;
                        t.sellPrice = kLData.High;
                        t.result = "Win";
                        t.skLData = kLData;
                    }
                }
                tranList[i] = t;
            }
        }
        void buy(int orientation, KLData kLData,string other = "")
        {

            Tran tran = new Tran();
            tran.orientation = orientation;
            tran.kLData = kLData;
            tran.isClose = false;
            //tran.buyPrice = orientation > 0 ? kLData.High : kLData.Low;
            tran.buyPrice = kLData.Close;
            tran.other = other;
            tran.isOuputed = false;
            tranList.Add(tran);

            //Debug.consol("\n[Byu "+ orientation+ "]\n");
        }
        struct Tran
        {
            public int orientation;
            public KLData kLData;
            public KLData skLData;
            public bool isClose;
            public double buyPrice;
            public double sellPrice;
            public string result;
            public string other;
            public bool isOuputed;
        }
        List<Tran> tranList;
        List<Tran> tranList_closed;

        const double Scale = 12;
        const double cutOffPoint = 2;
        const double cutSellPoint = 2;//3;
        double win = 0, lost = 0, draw = 0;
        double toDay_win = 0, toDay_lost = 0, toDay_draw = 0;
        public void dayend()
        {

            Debug.consol(" \n");
            for (int i = 0; i < tranList.Count; i++)
            {
                Tran t = tranList[i];
                if (t.isOuputed)
                    continue;
                Debug.consol((t.orientation >0?"[ +":"[ -")+ " ]  買入 "+t.buyPrice  + " " + t.kLData.sysTime.ToShortTimeString()
                    + " 賣出 "+t.sellPrice + " "+  t.skLData.sysTime.ToShortTimeString() + "  " + t.result + Math.Abs(t.buyPrice- t.sellPrice) + " ::" + t.other + "\n");
                if (!t.isClose)
                {
                    draw++;
                    t.isClose = true;
                }

                t.isOuputed = true;
                tranList[i] = t;
                


            }
            Debug.consol("Today win = " + cutSellPoint*( win- toDay_win) + ", lost = " + cutOffPoint*( lost- toDay_lost) + ".  " + (int)(100.0 * ((cutSellPoint*(win - toDay_win)) / ((cutSellPoint*(win - toDay_win)) + (cutOffPoint*(lost - toDay_lost))))) + "%  draw = " + (draw - toDay_draw) + "\n");
            toDay_win = win;
            toDay_lost = lost;
            toDay_draw = draw;
            tranList_closed.AddRange(tranList);
            tranList.Clear();
            //Debug.consol(" \n");
        }

        
        public void result_output()
        {
            win *= cutSellPoint;
            lost *= cutOffPoint;

            Debug.consol("Total win = "+ win + ", lost = "+ lost + ".  " + (100.0*(win/(win+lost))) + "%  draw = "+ draw + "\n");
            Debug.consol("Outputing result ...");

            //System.IO.File.WriteAllText(@"C:\Users\darkchan\Desktop\stockresult.csv", string.Empty);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\darkchan\Desktop\stockresult.csv", false))
            {

                for (int i = 0; i < tranList_closed.Count; i++)
                {
                    Tran t = tranList_closed[i];
                    //Debug.consolLine((t.orientation >0?"[ +":"[ -")+ " ]  買入 "+t.buyPrice  + " " + t.kLData.sysTime.ToShortTimeString()
                    //    + " 賣出 "+t.sellPrice + " "+  t.skLData.sysTime.ToShortTimeString() + "  " + t.result + Math.Abs(t.buyPrice- t.sellPrice));



                    

                    file.WriteLine(
                        
                        string.Join(",",

                        new string[] {
                            t.orientation.ToString(),
                            t.buyPrice.ToString(),
                            t.sellPrice.ToString(),
                            t.result,
                            t.isClose.ToString(),
                            t.other,
                            t.kLData.ToString(),
                            t.skLData.ToString()
                            })



                        );
                    Debug.consol("\rOutputing result " + (int)(100.0*((1.0*i)/ (1.0* tranList_closed.Count))) + "%");

                }
                

                

            }
            Debug.consol("\rOutputing result ... Done          \n");

        }



    }
}
