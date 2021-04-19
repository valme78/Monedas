using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Bitso_Entitys;
using EntidadesGenerales;

namespace CoinTrader
{
    public partial class Form1 : Form
    {
        public List<CMonedaHistorial> lstHistrialmoney = new List<CMonedaHistorial>(); 

        private List<string> lstMonedas = new List<string>(); //lista para guardar el nombre de las monedas activas.
        public delegate void delegadomonedas(string sMoneda, DataGridView lvObj, Int32 index);
        delegate void SetTextCallback2(string text, CMoneyPrice moneypp);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            CheckForIllegalCrossThreadCalls = false; //para que los hilos secomporten como en clases de netframework 1.1 o 2.0

            configuragrafica();

            Bitso bit = new Bitso(true);
            bit.consultamonedasBitso(dgvMonedas, ref lstMonedas);
            tHilogral.Enabled = true;
            tHiloOrdenes.Enabled = false;


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

            //chart1.Series["Mes"].Legend = "xrp_mxn";
            //chart1.Series["Mes"].Name = "XRP_MXN";
            //chart1.Titles.Add("Historial del Mes..");


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
                                                        //item.YValues = new double[] { 8.0,4.0,double.Parse(his.valormax), double.Parse(his.valormin) };
                                                        //"High,Low,Open,Close"
                        item.YValues = new double[] { double.Parse(his.valormax), double.Parse(his.valormin),double.Parse(his.valoropen), double.Parse(his.valorclose) };
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

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void dgvMonedas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tHilogral_Tick(object sender, EventArgs e)
        {

            for( Int32 index=0; index < dgvMonedas.Rows.Count; index++)
            {
                Bitso bitso = new Bitso(true);
                delegadomonedas dm1 = new delegadomonedas(bitso.consultamoneda);  //
                IAsyncResult ar = dm1.BeginInvoke(dgvMonedas.Rows[index].Cells["dgcolmoneda"].Value.ToString().Trim(), dgvMonedas, index,null, null); //este esta funcionando 2
                                                                                                       //ar.AsyncWaitHandle.WaitOne(50);

                string sReturn = string.Empty;
                //dm1.EndInvoke(ref sReturn, ar);
                if (ar.IsCompleted)
                {
                    ar.AsyncWaitHandle.Close();
                    //dhilos.Mensaje -= new Bitso.MensajeErrorDelegate(cmh_Mensaje);

                }
                //dhilos.Mensaje += new CDatosMonedas.MensajeErrorDelegate(cmh_Mensaje);                    
                bitso.Mensaje += new Bitso.MensajeErrorDelegate(cmh_Mensaje);
            }
        }//finaliza timer general

        /// <summary>
        /// cmh_Mensaje, metodo que se ejecuta desde un delagado que obtiene datos de la moneda en ese momento
        /// </summary>
        /// <param name="p">Nombre de la moneda a consultar</param>
        /// <param name="moneyp">Estructura donde contiene los datos de la moneda consultada</param>
        private void cmh_Mensaje(string p, CMoneyPrice moneyp)
        {
            this.SetText(p, moneyp);
        }

        private void SetText(string text, CMoneyPrice moneyp)
        {
            if ( dgvMonedas.InvokeRequired)
            {
                try
                {
                    if (dgvMonedas.IsHandleCreated)
                    {
                        SetTextCallback2 d = new SetTextCallback2(SetText);
                        this.Invoke(d, new object[] { text, moneyp });
                    }
                }
                catch (StackOverflowException so)
                {
                    MessageBox.Show(so.InnerException.ToString());
                    txtlogerror.AppendText(so.InnerException.ToString());
                }                
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                   txtlogerror.AppendText(ex.Message);
                }
            }
            else
            {
                if (text.Trim().Length > 0)
                {
                    if (!text.Trim().Contains("Referencia a objeto no establecida como instancia de un objeto") &&
                        !text.Trim().Contains("Se excedió el tiempo de espera de la operación"))
                    {
                        txtlogerror.AppendText(text);
                     }
                }
                else
                {
                    
                }
            }
        }
    }
}
