using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Bitso_Entitys;
using EntidadesGenerales;

namespace CoinTrader
{
    public partial class Form1 : Form
    {
        public List<CMonedaHistorial> lstHistrialmoney = new List<CMonedaHistorial>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            configuragrafica();
        }

        private void configuragrafica()
        {
            //Configuracion del Grafico de monedas
            //Series serie = new Series("Monedas");

            leedatos();
            actualizagrafica();

            //chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;
            //chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 0;

            //chart1.Series["Mes"].XValueMember = "Dia";            
            //chart1.Series["Mes"].YValueMembers = "High,Low,Open,Close";
            //chart1.Series["Mes"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            //chart1.Series["Mes"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart1.Series["Mes"].CustomProperties = "PriceDownColor= Red, PriceUpColor= Blue";
            //chart1.Series["Mes"]["OpenCloseStyle"] = "Triangle";
            //chart1.Series["Mes"]["ShowOpenclose"] = "Both";
            //chart1.DataManipulator.IsStartFromFirst = true;
            //System.Windows.Forms.DataVisualization.Charting.DataPoint item = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
            ////item.ToolTip = "5.7";
            //item.XValue = DateTime.Now.Day;//sBook[i].tiempo.ToOADate();
            //item.YValues = new double[] {7.4,4.5,5.20,6.40 };
            //chart1.Series["Mes"].Points.Add(item);
            //System.Windows.Forms.DataVisualization.Charting.DataPoint item2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
            //item2.XValue = DateTime.Now.AddDays(1).Day;//sBook[i].tiempo.ToOADate();
            //item2.YValues = new double[] { 8.4, 5.5, 6.20, 8.0 };
            //chart1.Series["Mes"].Points.Add(item2);

        }

        private void leedatos()
        {
            Bitso bitso = new Bitso("JkkXaYKKDo", "578d14362dd2d68d05cb4dd0b2a92012", true);
            bitso.consultaHistorialMonedas();
            foreach (var mh in bitso.lHistrialmoney)
            {
                this.lstHistrialmoney.Add(new CMonedaHistorial { moneda = mh.moneda,
                                                                valormin = mh.valormin,
                                                                valormax = mh.valormax,
                                                                tiempomin = mh.tiempomin,
                                                                tiempomax = mh.tiempomax,
                                                                valorclose= mh.valorclose,
                                                                valoropen= mh.valoropen});
            }
        }

        private void actualizagrafica()
        {
            //Configuracion del Grafico de monedas
            //Series serie = new Series("Monedas");
            
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineWidth = 0;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineWidth = 0;

            chart1.Series["Mes"].XValueMember = "Dia";
            chart1.Series["Mes"].YValueMembers = "High,Low,Open,Close";
            chart1.Series["Mes"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            chart1.Series["Mes"].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series["Mes"].CustomProperties = "PriceDownColor= Red, PriceUpColor= Blue";
            chart1.Series["Mes"]["OpenCloseStyle"] = "Triangle";
            chart1.Series["Mes"]["ShowOpenclose"] = "Both";
            chart1.DataManipulator.IsStartFromFirst = true;

            
            foreach (var his in lstHistrialmoney)
            {

                if (his.moneda == "xrp_mxn")
                {
                    
                    {
                        string sTip = string.Format("max={0}\nmin={1}\nOpen={2}\nClose={3}", 0, 0, double.Parse(his.valormax), double.Parse(his.valormin));
                        
                        System.Windows.Forms.DataVisualization.Charting.DataPoint item = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                        item.ToolTip = sTip;
                        item.XValue = his.tiempomax.Day;// DateTime.Now.Day;//sBook[i].tiempo.ToOADate();
                                                               //item.YValues = new double[] { 7.4, 4.5, 5.20, 6.40 };
                        item.YValues = new double[] { 8.0,4.0,double.Parse(his.valormax), double.Parse(his.valormin) };
                        chart1.Series["Mes"].Points.Add(item);

                        //System.Windows.Forms.DataVisualization.Charting.DataPoint item2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                        //item2.XValue = DateTime.Now.AddDays(1).Day;//sBook[i].tiempo.ToOADate();
                        //item2.YValues = new double[] { 8.4, 5.5, 6.20, 8.0 };
                        //chart1.Series["Mes"].Points.Add(item2);
                        //item = null;
                    }
                }
            }
            chart1.Update();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode== Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
