namespace Stock
{
    partial class chartView01
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, "0,0,0,0");
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 60F;
            chartArea1.Position.Width = 90F;
            chartArea1.Position.X = 3F;
            chartArea1.Position.Y = 3F;
            chartArea2.AlignWithChartArea = "ChartArea1";
            chartArea2.Name = "ChartArea2";
            chartArea2.Position.Auto = false;
            chartArea2.Position.Height = 20F;
            chartArea2.Position.Width = 90F;
            chartArea2.Position.X = 3F;
            chartArea2.Position.Y = 60F;
            chartArea3.AlignWithChartArea = "ChartArea1";
            chartArea3.Name = "ChartArea3";
            chartArea3.Position.Auto = false;
            chartArea3.Position.Height = 20F;
            chartArea3.Position.Width = 90F;
            chartArea3.Position.X = 3F;
            chartArea3.Position.Y = 80F;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.ChartAreas.Add(chartArea2);
            this.chart1.ChartAreas.Add(chartArea3);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.IsVisibleInLegend = false;
            series1.Legend = "Legend1";
            series1.LegendText = "800000";
            series1.Name = "Series1";
            series1.Points.Add(dataPoint1);
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series1.YValuesPerPoint = 4;
            series2.ChartArea = "ChartArea2";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series2.IsVisibleInLegend = false;
            series2.Legend = "Legend1";
            series2.Name = "K";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series3.ChartArea = "ChartArea2";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Color = System.Drawing.Color.Aqua;
            series3.IsVisibleInLegend = false;
            series3.Legend = "Legend1";
            series3.Name = "D";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series4.ChartArea = "ChartArea2";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Color = System.Drawing.Color.Fuchsia;
            series4.IsVisibleInLegend = false;
            series4.Legend = "Legend1";
            series4.Name = "J";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series5.ChartArea = "ChartArea2";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series5.Color = System.Drawing.Color.Red;
            series5.IsVisibleInLegend = false;
            series5.Legend = "Legend1";
            series5.Name = "CrossPoint";
            series5.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series6.ChartArea = "ChartArea3";
            series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series6.IsVisibleInLegend = false;
            series6.Legend = "Legend1";
            series6.Name = "K_3m";
            series6.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series7.ChartArea = "ChartArea3";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series7.Color = System.Drawing.Color.Aqua;
            series7.IsVisibleInLegend = false;
            series7.Legend = "Legend1";
            series7.Name = "D_3m";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series8.ChartArea = "ChartArea3";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series8.Color = System.Drawing.Color.Fuchsia;
            series8.IsVisibleInLegend = false;
            series8.Legend = "Legend1";
            series8.Name = "J_3m";
            series8.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            series9.ChartArea = "ChartArea3";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
            series9.Color = System.Drawing.Color.Red;
            series9.IsVisibleInLegend = false;
            series9.Legend = "Legend1";
            series9.Name = "CrossPoint_3m";
            series9.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Series.Add(series5);
            this.chart1.Series.Add(series6);
            this.chart1.Series.Add(series7);
            this.chart1.Series.Add(series8);
            this.chart1.Series.Add(series9);
            this.chart1.Size = new System.Drawing.Size(927, 561);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // chartView01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 579);
            this.Controls.Add(this.chart1);
            this.Name = "chartView01";
            this.Text = "chartView01";
            this.Load += new System.EventHandler(this.chartView01_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}