using Stock.Stock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stock
{
    public partial class chartView01 : Form
    {
        Quote quote;
       // public static chartView01 i;
        public chartView01()
        {
            //i = this;
            InitializeComponent();
            
            quote = new Quote();
            //quote.init("999010",6);
            //this.Text = "999010";
            quote.OnUpdate += () => {

                //chart1.Series["Series1"].Points.Clear();


                this.Invoke(new Action(() => { update(); }));


            };
            quote.init("800000",1);
            this.Text = "800000";
            

            //quote.updateHistory(new DateTime(2017, 2, 10), new DateTime(2017, 2, 10));


            Debug.LogInfo("init chartView01 Done");

        }

        public void update()
        {

            /*int totalHeight = chart1.Height;
            Console.Write("chart1.ChartAreas = " + chart1.ChartAreas.Count);
            Console.Write("totalHeight = " + totalHeight);
            chart1.ChartAreas[0].Position.Y = 10;
            chart1.ChartAreas[0].Position.Height = totalHeight - 60;
            chart1.ChartAreas["ChartArea2"].Position.Y = totalHeight-60;
            chart1.ChartAreas["ChartArea2"].Position.Height = 30;
            chart1.ChartAreas["ChartArea3"].Position.Y = totalHeight-30;
            chart1.ChartAreas["ChartArea3"].Position.Height = 30;*/

            


            var points = chart1.Series["Series1"].Points;


            points.Clear();
            chart1.Series["K"].Points.Clear();
            chart1.Series["D"].Points.Clear();
            chart1.Series["J"].Points.Clear();

            chart1.Series["CrossPoint"].Points.Clear();
            chart1.Series["K_3m"].Points.Clear();
            chart1.Series["K_3m"].Points.Clear();
            chart1.Series["K_3m"].Points.Clear();



            double max = Double.MinValue;
            double min = Double.MaxValue;
            if(quote.KLDataArr.Count >=190)
            for(int i = quote.KLDataArr.Count-180; i< quote.KLDataArr.Count;i++)
            {
                    var k = 
                    quote.KLDataArr.ElementAt(i);
                    int index = points.AddXY(k.Key/*.ToShortTimeString()*/, new object[] { k.Value.Low, k.Value.High , k.Value.Close, k.Value.Open});
                    points[index].Color = k.Value.Close<k.Value.Open? Color.DarkRed: Color.DarkGreen;
                    if (k.Value.Close == k.Value.Open)
                    {
                        if(i >0)
                        {
                            var k_1 = quote.KLDataArr.ElementAt(i-1);
                            if(k_1.Value.Close == k.Value.Close)
                            {
                                if (index > 0)
                                    points[index].Color = points[index-1].Color;
                            }
                            else
                            {
                                points[index].Color = k_1.Value.Close > k.Value.Close ? Color.DarkRed : Color.DarkGreen;
                            }

                        }
                    }

                    if (k.Value.Low < min)
                        min = k.Value.Low;

                    if (k.Value.High > max)
                        max = k.Value.High;

                    //KDJ
                    chart1.Series["K"].Points.AddXY(k.Key/*.ToShortTimeString()*/, k.Value.kdj.K);
                    chart1.Series["D"].Points.AddXY(k.Key, k.Value.kdj.D);
                    chart1.Series["J"].Points.AddXY(k.Key, k.Value.kdj.J);
                    if (k.Value.kdj.crossPoint.Y != 0)
                        chart1.Series["CrossPoint"].Points.AddXY(k.Key/*.ToShortTimeString()*/, k.Value.kdj.crossPoint.X);


                    int step = k.Key.Minute % 3;
                    if(step == 0)
                    {
                        DateTime target_time = k.Key.AddMinutes(-step);
                        if(quote.KLDataArr_3min.ContainsKey(target_time))
                        {
                            var kdj_3m = quote.KLDataArr_3min[target_time].kdj;
                            chart1.Series["K_3m"].Points.AddXY(k.Key/*.ToShortTimeString()*/, kdj_3m.K);
                            chart1.Series["D_3m"].Points.AddXY(k.Key/*.ToShortTimeString()*/, kdj_3m.D);
                            chart1.Series["J_3m"].Points.AddXY(k.Key/*.ToShortTimeString()*/, kdj_3m.J);
                            if (kdj_3m.crossPoint.Y != 0)
                                chart1.Series["CrossPoint_3m"].Points.AddXY(k.Key/*.ToShortTimeString()*/, kdj_3m.crossPoint.X);
                        }
                        else
                        {
                            Debug.LogError("Missing 3min K line Data " + target_time.ToString());
                        }

                    }


                }


            /*foreach (var k in quote.KLDataArr)
            {
               
                chart1.Series["Series1"].Points.AddXY(k.Key.ToShortTimeString(), new object[] { k.Value.High, k.Value.Open, k.Value.Close,  k.Value.Low });
                if (k.Value.Low < min)
                    min = k.Value.Low;

                if (k.Value.High > max)
                    max = k.Value.High;

            }*/
            chart1.ChartAreas[0].AxisY.Maximum = max;
            chart1.ChartAreas[0].AxisY.Minimum = min;
         
        }


        private void chartView01_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
