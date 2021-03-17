using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using btcTrading;
using Newtonsoft.Json; //Libreria importada para manejo de JSON.  carpeta de proyecyto\LIB
using System.IO;
using System.Net;
using System.Threading;
using System.Media;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using WMPLib;
using System.Linq;
using bitso;
using bitso.Entities;
using EntidadesGenerales;

namespace btcTrading
{
    public partial class FrmPrincipalbtc : Form
    {
        public delegate void delegadomonedas(string sMoneda, ListView lvObj);
        delegate void SetTextCallback2(string text, CMoneyPrice moneypp);

        public FrmPrincipalbtc()
        {
            InitializeComponent();
        }

        //Listas Globales para el proyecto
        ImageList imageListSmall = new ImageList(); //lista de imagenes a mostrar en el listview de monedas
        //List<CDataPrice> lMoneyData = new List<CDataPrice>(); //lista donde se captura los datos de la moneda usado en los hilos.
        List<CMoneyPrice> lMoneyData = new List<CMoneyPrice>(); //lista donde se captura los datos de la moneda usado en los hilos.
        private List<CBalance> lmiBalances = new List<CBalance>();//lista para obtener el balance que se tiene en la plataforma como bitso,etc..
        List<CDatosAlertas> lAlertas = new List<CDatosAlertas>(); //lista para tener las diferentes alertas a validar
        List<CMoneda_Movto> lMoneymvto = new List<CMoneda_Movto>();// lista para capturar el maximo,minimo y hora de la moneda.

        //public delegate CDataPrice midelegadoMonedas(string sMoneda, ListView lvObj); // no se usa
        //public delegate void midelegado1(string sMoneda, ListView lvObj,ref string sTserv);
        
        
                
        // ClASE Y LISTA PARA GUARDAR CONFIGURACION DEL PROYECTO        
        //CConfiguracion cf = new CConfiguracion(); //aun no se usa
        //List<CConfiguracion> lcf = new List<CConfiguracion>(); //aun no se usa
        List<CHistoricoMonedas_dia> lMoneyHis_day = new List<CHistoricoMonedas_dia>();

        ///Hilo para consultar las ordenes de compra/venta activas.
        Thread thrOrdenes;
        ThreadStart trdstartOrdenes;// = new ThreadStart(validaOrdenesActivas);

        private bool bGrafica = false, bPlay = false,bGraficaMes= false;
        WindowsMediaPlayer wplayer = new WindowsMediaPlayer();///variable global para manejo de reproductor multimedia del grid

        long lTimeAnt = 0;// variable global para ejecutar cada determinado segundos la actualizacion de la grafica.
        short iCantMoneyGrafica = 0; //variable para contabilizar la cantidad de monedas que tiene el combo monedas de las gaficas.
        string [,] sMoneyprecio = new string[15,2];
        List<stTendencia> ltendencia3 = new List<stTendencia>(); // lista global para validar la tendencia cada minuto y medio.
        List<stTendencia> ltendencia3_3 = new List<stTendencia>(); // lista global para validar la tendencia cada minuto y medio.
        bool bOrdenActiva = false; // variable para validar si hay una orden de compra/venta 
        DateTime dthora;

        //variables globales donde se mantiene los datos de api seleccionado del ultimo login
        private string sApi_key = string.Empty;
        private string sApi_pwd = string.Empty;
        string sPlataforma = string.Empty;
        List<CHistoricoMonedas_dia> lTendecia = new List<CHistoricoMonedas_dia>(); //agrega por moneda precio cada segundos para validar la tendenecia del precio.
        List<CMoneda_Movto> lultPrecioOrden = new List<CMoneda_Movto>();//agregar el ultimpo precio venta/compra de cada moneda en ordenes.
        //List<CTrading_Bitso.CMoneda_Historial> lstHistrialmoney = new List<CTrading_Bitso.CMoneda_Historial>();
        List<CMonedaHistorial> lstHistrialmoney = new List<CMonedaHistorial>();
        //List<FrmHistorialOrdenes.ChistorialOrdenes> lstHOrdenes = new List<FrmHistorialOrdenes.ChistorialOrdenes>();
        List<COrdenes> lstHOrdenes = new List<COrdenes>();
        private TextBox txtlogerror = new TextBox();
        private bool bTipoPrueba = false; //variable para validar si se prueba en servidor real o pruebas
        List<CMoneda4dias> lstmoneda4diasmax = new List<CMoneda4dias>();
                
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)768; //TLS 1.1



            List<string> lstmon = new List<string>();


            lstmon.Add("A");
            lstmon.Add("E");

            foreach (char s in string.Format("ABCDEFG"))
            {
                if ( lstmon.Exists(x=> x.ToString()==s.ToString())  )
                {
                    lstmon.Add(s.ToString());
                }
            }

            btnActualizamonhistorial.Enabled = false;
            bOrdenActiva = false;
            bTipoPrueba = true; // variable global para el tipo de ejecucion del servidor real o de pruebas.
            //string dir = Environment.CurrentDirectory + @"..\..\imagenes\";
            //MessageBox.Show(dir);

            //poner los años en combo año para la grafica
            cboaño.Items.Add(DateTime.Now.Year.ToString() );
            cboaño.Items.Add("2018");
            cboaño.Items.Add("2019");
            cboaño.Items.Add("2020");
            cboaño.SelectedIndex = 0;
            //guardarHistorialMonedasDia();
            

            //Pone en una medida en particular el dialogo, para esconder la grafica.
            this.Width = 720;// 679;
            this.Height = 763;
            btnGrafica.Text = "&Grafica >>";

            //Configuracion del hilo que ejecuta las ordenes activas
            //trdstartOrdenes = new ThreadStart(validaOrdenesActivas);
            //thrOrdenes = new Thread(trdstartOrdenes);
            //thrOrdenes.IsBackground = true;
            //thrOrdenes.Name = "thrOrdenes";
            
            //thrOrdenes.Start();

            //Agregando datos para el combo de meses para la grafica historial del mes.
            object[] oItem = { "ENE     1","FEB      2","MZO     3","ABR     4","MAY     5","JUN     6","JUL     7","AGO     8","SEP     9","OCT     10","NOV     11","DIC     12"};
            cbomesgh.Items.AddRange(oItem);             
            cbomesgh.SelectedIndex = DateTime.Now.Month -1;

            CheckForIllegalCrossThreadCalls = false; //para que los hilos secomporten como en clases de netframework 1.1 o 2.0
            imageListSmall.ColorDepth = ColorDepth.Depth32Bit;
            // Asignamos el objeto ImageList al control ListView.
            imageListSmall.Images.Add("btc", Image.FromFile(@"..\..\imagenes\btc.jpg") );
            imageListSmall.Images.Add("eth", Image.FromFile(@"..\..\imagenes\eth.png") );
            imageListSmall.Images.Add("xrp", Image.FromFile(@"..\..\imagenes\xrp.png"));
            imageListSmall.Images.Add("bch", Image.FromFile(@"..\..\imagenes\bch.jpg"));
            lvMonedas.SmallImageList = imageListSmall;
            //lvMonedas.LargeImageList = imageListSmall;

            //RootMoney datosmoneda;
            //MoneyData moneydata;// se inibio por la nuev aclase de bitso.
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://api.bitso.com/v3/available_books/");            
           /* HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://bitso.com/v3/ticker/");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))*/
            {
                //var json = reader.ReadToEnd();// se inibio 25jun19
               //datosmoneda = JsonConvert.DeserializeObject<RootMoney>(json);
                //moneydata = JsonConvert.DeserializeObject<MoneyData>(json);

                //List<Payload> sBook =datosmoneda.payload.FindAll(delegate(Payload bk)
                //{
                //    //return bk.book.Substring(0,3) == "btc";
                //    return bk.book != " ";
                //});
                configuraGridOrdenes();
                configuraGridMonedas();// grid para el detalle de todas las monedas
                ListViewItem item ; //= new ListViewItem(new string[] { "preuba 1", "preuba2", "prueba3" });
                
                Random rnd = new Random();
                int iRen = 0;// esto para el precio del arreglo de tendencia de precio.

                Bitso bitso = new Bitso(true);
                
                foreach( var ticker in bitso.GetTickers() )
                //for (Int32 index = 0; index < moneydata.payload.Count; index++)
                {
                    //cbomonedas.Items.Add(moneydata.payload[index].book);
                    cbomonedas.Items.Add(ticker.Book);
                    //Tomar en cuenta esto cada vez que se asigne una columna nueva al listview lvmonedas
                    //item = new ListViewItem(new string[] { moneydata.payload[index].book, "", "", "", "","" ,"" });
                    item = new ListViewItem(new string[] {ticker.Book, "", "", "", "", "", "","" });
                    //item.Name = moneydata.payload[index].book.ToString();
                    item.Name = ticker.Book.Trim();
                    item.ImageKey = item.Name.Substring(0, 3);
                    if (item.Name == "btc_mxn" || item.Name == "xrp_mxn" || item.Name == "bat_mxn")
                    {
                        /*item.SubItems[1].Text = string.Format("{0,14:N2}", Convert.ToDouble(moneydata.payload[index].ask));
                        item.SubItems[2].Text = string.Format("{0,14:N2}", Convert.ToDouble(moneydata.payload[index].high));
                        item.SubItems[3].Text = string.Format("{0,14:N2}", Convert.ToDouble(moneydata.payload[index].low));
                        item.SubItems[4].Text = string.Format("{0,14:N2}", Convert.ToDouble(moneydata.payload[index].last));
                          */
                        item.SubItems[1].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.Ask));
                        item.SubItems[2].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.PriceHigh));
                        item.SubItems[3].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.PriceLow));
                        item.SubItems[4].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.LastTradedPrice));
                    }
                    else
                    {
                        //item.SubItems[1].Text = moneydata.payload[index].ask;
                        //item.SubItems[2].Text = moneydata.payload[index].high;
                        //item.SubItems[3].Text = moneydata.payload[index].low;
                        //item.SubItems[4].Text = moneydata.payload[index].last;

                        item.SubItems[1].Text = ticker.Ask;
                        item.SubItems[2].Text = ticker.PriceHigh;
                        item.SubItems[3].Text = ticker.PriceLow;
                        item.SubItems[4].Text = ticker.LastTradedPrice;
                    }

                    lvMonedas.Items.Add(item);
                    alternaColor(1);

                    if (item.Name.Substring(item.Name.Length - 3, 3).Trim() == "mxn")
                    {
                        //Agregar linea de grafica de la moneda
                        Series serie = new Series(item.Name);
                        serie.BorderColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                        serie.Color = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                        serie.ChartType = SeriesChartType.Line;
                        serie.BorderWidth = 3;
                        serie.YValuesPerPoint = 32;
                        serie.XValueType = ChartValueType.Time;
                        chart1.Series.Add(serie);
                        //chart1.Titles.Add( "Movimientos de la moneda[" + item.Name + "]" );
                    }

                    dgcolMonedas.Items.Add(item.Name);
                    //dgcolDir.Text = "...";
                    //dgcolPlay.Text = "Play";
                    //dgcolPlay.HeaderText = "Play";                    
                    

                    //Agregar al combo de monedas a graficar solo las que sean en moneda mxn
                    //comparamos las ultimas 3 letras si trae ...mxn es que puede ser btc_mxn, xrp_mxn,etc.
                    if (item.Name.Substring(item.Name.Length - 3, 3) == "mxn")
                    {
                        cbomonedas_grafica.Items.Add(item.Name);
                        sMoneyprecio[iRen, 0] = item.Name;
                        sMoneyprecio[iRen, 1] = "0.0";
                        iRen += 1;
                    }
                   
                } 
               // lMonedas = datosmoneda.payload;                
                //lMoneyData = moneydata.payload; //agregar la nueva clase y adjuntar datos
                
                timer1.Enabled = true;
                timer2.Enabled = false;

                //Configuracion del Grafico de monedas
                //chart1.Series["Monedas"].XValueMember = "Precio";
                //chart1.Series["Monedas"].YValueMembers = "High,Low,Open,Close";
                ////chart1.Series["Monedas"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
                //chart1.Series["Monedas"].CustomProperties = "PriceDownColor= Red, PriceUpColor= Blue";
                //chart1.Series["Monedas"]["OpenCloseStyle"] = "Triangle";
                //chart1.Series["Monedas"]["ShowOpenclose"] = "Both";    
                //chart1.DataManipulator.IsStartFromFirst = true;

                //se inibhio 19 jun 2019 para reubicarlo 
                //dgcolPathFile.DefaultCellStyle.NullValue = " ";
                //dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //dataGridView1.Columns["dgcolMax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns["dgcolMin"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                //dtgMismonedas.DefaultCellStyle.NullValue = "";
                //dtgMismonedas.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                //dtgMismonedas.Columns["dg2colDisponible"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dtgMismonedas.Columns["dg2colenorden"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dtgMismonedas.Columns["dg2collockedmx"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dtgMismonedas.Columns["dg2coltotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dtgMismonedas.Columns["dg2colenviopendiente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dtgMismonedas.Columns["dg2coltotalmx"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                
                //toma el timepo para actualiar la grafica.
                //tTiempo = new DateTime().ToLocalTime();

                lTimeAnt = DateTime.Now.Ticks / 10000000;

                //Leer datos de Ordenes Realizadas o historial de moneda del dia en caso que se cerro el programa por alguna causa.
                bool bhistorial = false;
                if (leerHistorialOrdenesDia())
                    bhistorial = true;
                if (LeerHistorialMonedasDia())
                    bhistorial = true;
                if (bhistorial)
                {
                    if (cbomonedas_grafica.Items.Count >= 2)
                        cbomonedas_grafica.SelectedIndex = 2;
                    this.cbomonedas_grafica_SelectedIndexChanged(cbomonedas_grafica, null);

                    foreach (string sMonedacbo in cbomonedas_grafica.Items)
                    {
                        //#listado movimiento diario, maximo y minimo
                        List<CHistoricoMonedas_dia> l1 = lMoneyHis_day.FindAll(delegate(CHistoricoMonedas_dia hm)
                        {
                            return hm.moneda == sMonedacbo;
                        });
                        string[] stMax = l1.Max(x => x.precio + "|" + x.tiempo).Split('|');
                        string[] stMin = l1.Min(x => x.precio + "|" + x.tiempo).Split('|');
                        lMoneymvto.Add(new CMoneda_Movto { moneda = sMonedacbo, 
                                                            valorMax = double.Parse(stMax[0]), 
                                                            valorMin = double.Parse(stMin[0]), 
                                                            tiempoMax = DateTime.Parse(stMax[1]), 
                                                            tiempoMin = DateTime.Parse(stMin[1]),
                                                           valorClose = double.Parse(stMin[0]),
                                                           valorOpen = double.Parse(stMin[0])/* se agrego el valor min para cuando no se tiene dato..*/
                        });
                    }
                }// if (bhistorial)
            }//using (StreamReader reader = new StreamReader(stream))
          
        }

        private void configuraGridMonedas()
        {
            // Set the view to show details.
            lvMonedas.View = View.Details;
            lvMonedas.Columns.Add("Moneda", 80, HorizontalAlignment.Left);
            lvMonedas.Columns.Add("Precio ", 80, HorizontalAlignment.Right);
            lvMonedas.Columns.Add("Precio Max ", 80, HorizontalAlignment.Right);
            lvMonedas.Columns.Add("Precio Min ", 80, HorizontalAlignment.Right);
            lvMonedas.Columns.Add("ultimo precio", 80, HorizontalAlignment.Right);
            lvMonedas.Columns.Add("    Variacion", 120, HorizontalAlignment.Left);
            //lvMonedas.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.None);
            //lvMonedas.HorizontalAlignmen
            lvMonedas.Columns.Add("Tendencia1", 75, HorizontalAlignment.Right);
            lvMonedas.Columns.Add("Tendencia2", 75, HorizontalAlignment.Right);
            lvMonedas.FullRowSelect = true;
            lvMonedas.GridLines = true;
            lvMonedas.BackColor = Color.Gray;
            //tomar en cuenta en el metodo _load ..new ListViewItem , agregar un parametro mas.

        }
        private void configuraGridOrdenes()
        {
            // Set the view to show details.
            //lvmimonedas.View = View.Details;
            //lvmimonedas.Columns.Add("Moneda", 70, HorizontalAlignment.Left);
            //lvmimonedas.Columns.Add(" Disponible ", 90, HorizontalAlignment.Right);
            //lvmimonedas.Columns.Add(" En Orden ", 90, HorizontalAlignment.Right);
            //lvmimonedas.Columns.Add("Locked_MX", 70, HorizontalAlignment.Right);
            //lvmimonedas.Columns.Add("  Total    ", 90, HorizontalAlignment.Right);
            //lvmimonedas.Columns.Add("Envio Pendiente", 90, HorizontalAlignment.Center);
            //lvmimonedas.Columns.Add("Total MX", 70, HorizontalAlignment.Right);
            //lvmimonedas.Columns.Add("Elim Ordn.", 80, HorizontalAlignment.Center);
            //lvmimonedas.FullRowSelect = true;
            //lvmimonedas.GridLines = true;
            //lvmimonedas.BackColor = Color.Gray;

            dgcolPathFile.DefaultCellStyle.NullValue = " ";
            dtgtrading.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dtgtrading.Columns["dgcolMax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgtrading.Columns["dgcolMin"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dtgMismonedas.DefaultCellStyle.NullValue = "";
            dtgMismonedas.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dtgMismonedas.Columns["dg2colDisponible"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["dg2colenorden"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["dg2collockedmx"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["dg2coltotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["dg2colenviopendiente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["dg2coltotalmx"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dtgMismonedas.Columns["colid"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

        }
        private DataGridViewLinkColumn AddLinkColumn()
        {
            DataGridViewLinkColumn links = new DataGridViewLinkColumn();

            links.UseColumnTextForLinkValue = true;
            //links.HeaderText = ColumnName.ReportsTo.ToString();
           // links.DataPropertyName = ColumnName.ReportsTo.ToString();
            links.ActiveLinkColor = Color.White;
            links.LinkBehavior = LinkBehavior.SystemDefault;
            links.LinkColor = Color.Blue;
            links.TrackVisitedState = true;
            links.VisitedLinkColor = Color.YellowGreen;
            links.Text = "ejemplo";

            //DataGridView1.Columns.Add(links);
            return links;
        }
        private void alternaColor(int iOpcion)
        {
            if (iOpcion == 1)
            {
                for (int i = 0; i < lvMonedas.Items.Count; i++)
                {
                    if (lvMonedas.Items[i].Index % 2 == 0)
                        lvMonedas.Items[i].BackColor = Color.DarkGray;
                    else
                        lvMonedas.Items[i].BackColor = Color.Aquamarine;
                }
            }
            else
            {
                for (int i = 0; i < dtgMismonedas.Rows.Count-1; i++)
                {
                    if (dtgMismonedas.Rows[i].Index % 2 == 0)
                        dtgMismonedas.Rows[i].DefaultCellStyle.BackColor = Color.DarkGray;
                    else
                        dtgMismonedas.Rows[i].DefaultCellStyle.BackColor = Color.Aquamarine;
                }
            }
        }

        private void btnconsulta_Click(object sender, EventArgs e)
        {
            //getGridAlerts();// metodo para obtener las alertas configuradas para despues guardarlas como configuracion
             //CTrading_Bitso bt = new CTrading_Bitso();
             //bt.consultaOrdenesActivas("xrp_mxn");
            // bt.eliminaOrden("", "txI4MRY8R7moipM4");
            consultapreciomoneda("xrp_mxn");

            Bitso bitso = new Bitso("JkkXaYKKDo", "578d14362dd2d68d05cb4dd0b2a92012", true);
           // bitso.CancelOpenOrder("txI4MRY8R7moipM4");
             string sboks = string.Empty;
             //foreach (var ticker in bitso.GetTickers())
             //{
             //    sboks = string.Format("Book = {0}, Last = {1}, High = {2}, Low = {3}", ticker.Book,ticker.LastTradedPrice, ticker.PriceHigh, ticker.PriceLow);
             //    MessageBox.Show(sboks);                
             //}

             foreach (var orden in bitso.GetOpenOrders("", "", "desc", 25))
            {
                sboks = string.Format( "Book={0}, id={1}, precio={2}, amount={3}, estatus ={4}, venta/compra:{5}", orden.Book ,orden.Oid , orden.Price , orden.OriginalAmount, orden.Status,orden.Side );
                MessageBox.Show(sboks);
            }
                  
           

            //string sRuta = @"c:\logs\apiBitsoAlertas.json";
            //if (getGridAlerts())
            //{
            //    StreamWriter wr = new StreamWriter(sRuta);

            //    if (!Directory.Exists(@"c:\logs")) File.Create(@"c:\logs");
            //    //if (!Directory.Exists(@"c:\sys\util")) File.Create("c:\\sys\\util");

            //    //Crear el archivo con la informacion
            //    //using (StreamWriter sw = File.OpenWrite(sRuta) ) // CreateText(sRuta))
            //    {
            //        //lAlertas.Add(new CDatosAlertas() { file= "c:\ruta\ruta2\archivo.mp3",maximo=5.0, minimo=4.0, moneda="moneda1"} );
            //        //lAlertas.Add(new CDatosAlertas() { file = "c:\ruta\ruta2\archivo2.mp3", maximo = 25.0, minimo = 22.0, moneda = "moneda2" });

            //        var jsontxt = JsonConvert.SerializeObject(lAlertas);
            //        //File.Delete(@"c:\sys\util\apiBitsoAlertas.json");
            //        //File.WriteAllText(@"c:\sys\util\apiBitsoAlertas.json", jsontxt);
            //        wr.WriteLine(jsontxt);
            //        //sw.WriteLine(sIp.Trim());                
            //        wr.Close();
            //    }
            //}

         
            /*DataGridViewLinkColumn links = new DataGridViewLinkColumn();

            links.UseColumnTextForLinkValue = true;
            links.HeaderText = ColumnName.ReportsTo.ToString();
            links.DataPropertyName = ColumnName.ReportsTo.ToString();
            links.ActiveLinkColor = Color.White;
            links.LinkBehavior = LinkBehavior.SystemDefault;
            links.LinkColor = Color.Blue;
            links.TrackVisitedState = true;
            links.VisitedLinkColor = Color.YellowGreen;

            lvmimonedas.Items[0].SubItems[7]*/
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbomonedas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( cbomonedas.SelectedIndex >=0 )
            {
                btnActualizamonhistorial.Enabled= true;
                consultaHistorialMoney();
                consultavalor4dis(cbomonedas.SelectedItem.ToString());
            }
            else
            {
                btnActualizamonhistorial.Enabled= false;
            }
        }

        private void consultavalor4dis(string sMoneda)
        {

            if (lstmoneda4diasmax.Count > 0)
            {
                double dMax = lstmoneda4diasmax.FindAll(delegate(CMoneda4dias max) { return max.moneda == sMoneda; }).Max(x => x.preciomax);
                double dMin = lstmoneda4diasmax.FindAll(delegate(CMoneda4dias max) { return max.moneda == sMoneda; }).Min(x => x.preciomin);
                string sOrden = string.Empty;
                foreach (var mon in lstmoneda4diasmax.FindAll(delegate(CMoneda4dias m) { return m.moneda == cbomonedas.SelectedItem.ToString(); }).OrderBy(x => x.preciomax))
                {
                    sOrden += mon.preciomax.ToString() + ",";

                }
                MessageBox.Show(sOrden);

                txtmax4dias.Text = dMax.ToString();
                txtmin4dias.Text = dMin.ToString();
                txtporc4dias.Text = (((dMax - dMin) / dMin) * 100.00).ToString();
            }
            //string sDato = string.Empty;
            //string sDato2 = string.Empty;
            //foreach (var dia4 in lstmoneda4diasmax.FindAll(delegate(CMoneda4dias dias) { return dias.moneda == "xrp_mxn"; }))
            //{
            //    sDato += dia4.fecha.ToString() + " [" + dia4.preciomax.ToString() + "][" + dia4.moneda + "]," + Environment.NewLine;
            //    sDato2 += dia4.fecha.ToString() + " [" + dia4.preciomin.ToString() + "][" + dia4.moneda + "]," + Environment.NewLine;
            //}
            //MessageBox.Show(sDato + sDato2);
        }

        /// <summary>
        /// timer1_Tick(object sender, EventArgs e), metodo que se ejecuta cada N segundos para obtener los datos de la moneda en ese momento.
        /// </summary>
        /// <param name="sender">no se usa</param>
        /// <param name="e">no se usa</param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            lMoneyData.Clear();
            string sTServ= string.Empty;
            for (Int16 index = 0; index < lvMonedas.Items.Count; index++)
            {
                {
                    //CDatosMonedas dhilos = new CDatosMonedas(lvMonedas.Items[index].Text, lvMonedas);
                    Bitso dhilos = new Bitso(true);

                    //midelegado1 dm1 = new midelegado1(dhilos.consultaCoins2);  //funcionando 1                    
                    delegadomonedas dm1 = new delegadomonedas(dhilos.consultamonedas);  //funcionando 1                    
                    IAsyncResult ar = dm1.BeginInvoke(lvMonedas.Items[index].Text, lvMonedas,null, null); //este esta funcionando 2
                    //ar.AsyncWaitHandle.WaitOne(50);
                    
                    string sReturn = string.Empty;
                    //dm1.EndInvoke(ref sReturn, ar);
                     if (ar.IsCompleted)
                     {  
                        ar.AsyncWaitHandle.Close();
                        //dhilos.Mensaje -= new Bitso.MensajeErrorDelegate(cmh_Mensaje);
                        
                     }
                     //dhilos.Mensaje += new CDatosMonedas.MensajeErrorDelegate(cmh_Mensaje);                    
                     dhilos.Mensaje += new Bitso.MensajeErrorDelegate(cmh_Mensaje);
                }
            }
            
            

            ///Esto sirve para mostrar una alerta y mandar a enfrente la aplicación.            
            /*this.TopMost = true;
            this.WindowState = FormWindowState.Normal;
            this.Refresh();
            this.TopMost = false;*/
        }
        
        /// <summary>
        /// cmh_Mensaje, metodo que se ejecuta desde un delagado que obtiene datos de la moneda en ese momento
        /// </summary>
        /// <param name="p">Nombre de la moneda a consultar</param>
        /// <param name="moneyp">Estructura donde contiene los datos de la moneda consultada</param>
        private void cmh_Mensaje(string p, CMoneyPrice moneyp)
        {
            this.SetText2(p, moneyp);
        }

        private void SetText2(string text, CMoneyPrice moneyp)
        {
            if (this.txtlogerror.InvokeRequired && dtgMismonedas.InvokeRequired)
            {
                try
                {
                    if (txtlogerror.IsHandleCreated)
                    {
                        SetTextCallback2 d = new SetTextCallback2(SetText2);
                        this.Invoke(d, new object[] { text, moneyp });
                    }
                }
                catch (StackOverflowException so)
                {
                    MessageBox.Show(so.InnerException.ToString());
                    txtlogerror.AppendText(so.InnerException.ToString());
                }
                catch (IOException ie)
                {
                    txtlogerror.AppendText(ie.InnerException.ToString());
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
                        btnlog.ForeColor = Color.Black;
                    }
                }
                else
                {
                    lMoneyData.Add(moneyp);
                    if (validaTiempoGraficaUpdate())// cada # de segundos se ejectuda para actualiar la grafica.
                    {
                        actualizaGrafica(moneyp);//actualiza los datos de la grafia del dia
                        if (bGraficaMes)
                        {
                            if (txtplataforma.Text.Trim().ToUpper() == "BITSO")
                            {
                                Bitso cb = new Bitso(bTipoPrueba);
                                cb.consultaHistorialMonedas();
                                lstHistrialmoney.Clear();
                                foreach (var mh in cb.lHistrialmoney)
                                {
                                    lstHistrialmoney.Add(new CMonedaHistorial { moneda = mh.moneda, 
                                                                                valormin = mh.valormin, 
                                                                                valormax = mh.valormax, 
                                                                                tiempomin = mh.tiempomin, 
                                                                                tiempomax = mh.tiempomax  });
                                }
                            }
                            bGraficaMes = false;
                        }
                    }
                    validaAlertas(moneyp.book, double.Parse(moneyp.ask));
                    //tomar aqui en cuenta la fecha que trae 'moneyp' para mandar parametro y validar cambio de dia.
                    validaMovtoMoneda(moneyp.book, double.Parse(moneyp.ask));
                    //validaOrdenes(moneyp.book); // esto 2, se inive para validarlo en otro hilo: validaordenesactivas

                    //Dato para poner en el encabezado de la pantalla
                    this.Text = string.Format("Movimientos de monedas Digitales.              hora servidor[{0}]-[{1}]", txtplataforma.Text.Trim(), moneyp.created_at.ToShortTimeString() );
                }
            }
        }

        private void FrmPrincipalbtc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.F8)
                txtlogerror.Clear();

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            FrmLogin frmlogin = new FrmLogin();
            if( frmlogin.ShowDialog() == DialogResult.OK )
            {
                //Tomamos o se asigna los datos de la monedas que se tienen en inversion y se pasa al list de Balances.                lmiBalances = frmlogin.lBalances;
                lmiBalances = frmlogin.lBalances;
                actualizaListVewmiMonedas();
                sApi_key = frmlogin.sApi_key;
                sApi_pwd = frmlogin.sApi_pwd;
                sPlataforma = frmlogin.sPlataforma;
                txtplataforma.Text = sPlataforma;

                DateTime dt = DateTime.Now.AddDays(-4);

                foreach (var mh in frmlogin.lstHistrialmoney_login)
                {
                    lstHistrialmoney.Add(new CMonedaHistorial { moneda = mh.moneda, valormin = mh.valormin, valormax = mh.valormax, tiempomin = mh.tiempomin, tiempomax = mh.tiempomax });
                }
                   //validacion para traerce los ultimo 4 dias de valores de monedas
                lstmoneda4diasmax.Clear();
                foreach( var mon in lstHistrialmoney.FindAll( delegate (CMonedaHistorial hm){ return hm.tiempomax >= dt; } ) )
                {
                    lstmoneda4diasmax.Add(new CMoneda4dias { moneda = mon.moneda, preciomax = double.Parse(mon.valormax),preciomin = double.Parse(mon.valormin), fecha = mon.tiempomax });
                    
                }

                string sDato = string.Empty;
                string sDato2 = string.Empty;
                foreach( var dia4 in lstmoneda4diasmax.FindAll( delegate (CMoneda4dias dias) {return dias.moneda=="xrp_mxn"; } ))
                {
                    sDato += dia4.fecha.ToString() + " [" + dia4.preciomax.ToString() + "]["+ dia4.moneda+"]," + Environment.NewLine;
                    sDato2 += dia4.fecha.ToString() + " [" + dia4.preciomin.ToString() + "][" + dia4.moneda + "]," + Environment.NewLine;
                }
                //MessageBox.Show(sDato + sDato2);
                foreach (var val in lmiBalances)
                {
                    if (float.Parse(val.locked) > 0.0) // valor en orden, si se tiene una orden activa
                    {
                        if (thrOrdenes== null || !thrOrdenes.IsAlive)
                        {
                            trdstartOrdenes = new ThreadStart(validaOrdenesActivas);
                            thrOrdenes = new Thread(trdstartOrdenes);
                            thrOrdenes.IsBackground = true;
                            thrOrdenes.Name = "thrOrdenes";
                            thrOrdenes.Start();
                        }                        
                    }
                    timer2.Enabled = true;
                }                
            }
        }

        private void actualizaListVewmiMonedas()
        {
            dtgMismonedas.Rows.Clear();
            
            for (int i = 0; i < lmiBalances.Count; i++)
            {
                if (Convert.ToDouble( lmiBalances[i].total) > 0.0 )                
                {
                                        
                    //Codigo para agregar datos al grid de mis monedas
                    DataGridViewRow Item = new DataGridViewRow();
                    Item.CreateCells(dtgMismonedas);
                                                    
                    //Item.Cells[dg2colmoneda.Name].Value = lmiBalances[i].currency;
                    //Item.Cells["dg2colDisponible"].Value = lmiBalances[i].available;
                    //Item.Cells["dg2colenorden"].Value = lmiBalances[i].locked;
                    //Item.Cells["dg2collockedmx"].Value = "0.0"; //locked_mx
                    //Item.Cells["dg2coltotal"].Value = lmiBalances[i].total;
                    //Item.Cells["dg2colenviopendiente"].Value = lmiBalances[i].pending_withdrawal;
                    //Item.Cells["dg2coltotalmx"].Value = "0.0"; //Total mx
                     
                    Item.Cells[0].Value = lmiBalances[i].currency;
                    Item.Cells[1].Value = lmiBalances[i].available;
                    Item.Cells[2].Value = lmiBalances[i].locked;
                    Item.Cells[3].Value = "0.0"; //locked_mx
                    Item.Cells[4].Value = lmiBalances[i].total;
                    Item.Cells[5].Value = lmiBalances[i].pending_withdrawal;
                    Item.Cells[6].Value = "0.0"; //Total mx
                    Item.Cells[8].Value = "";  //colid                    
                    //DataGridViewLinkColumn((Item.Cells[7]).FormattedValue = new DataGridViewLinkColumn().UseColumnTextForLinkValue = true);

                    
                    if (Convert.ToDouble(lmiBalances[i].locked) > 0.1)
                    {    //monedas1                        
                        DataGridViewLinkCell links = new DataGridViewLinkCell();
                        
                        links.UseColumnTextForLinkValue = false; // si se quiere poner como tipo hipervinculo , ponerlo en true
                        //links.HeaderText = dg2colelimorden.HeaderText;
                        //links.DataPropertyName = dg2colelimorden.DataPropertyName;
                        links.ActiveLinkColor = Color.White;
                        links.LinkBehavior = LinkBehavior.SystemDefault;
                        links.LinkColor = Color.Blue;
                        links.TrackVisitedState = true;
                        links.VisitedLinkColor = Color.YellowGreen;                        
                        // string value = dtgMismonedas.SelectedCells[iRen].FormattedValue.ToString();
                        links.Value = ""; // "Canc. Ord.";
                        Item.Cells[7] = links;                        
                        bOrdenActiva = true;
                    }
                    else
                    {
                        Item.Cells[7].Value = "";                        
                    }
                    dtgMismonedas.Rows.Add(Item);                    
                }
            }
        }

        private void actualizagridordenes()
        {
            //int iRen = 0;
            //dtgMismonedas.Rows.Add(lmiBalances.Count);
            for (int i = 0; i < lstHOrdenes.Count; i++)
            {
                
                //Codigo para agregar datos al grid de mis monedas
                DataGridViewRow Item = new DataGridViewRow();
                Item.CreateCells(dtgMismonedas);

                Item.Cells[0].Value = lstHOrdenes[i].moneda; //nombre moneda
                Item.Cells[1].Value = lstHOrdenes[i].cantmonedas.ToString(); //Disponible
                Item.Cells[2].Value = lstHOrdenes[i].valor ;    //En orden
                Item.Cells[3].Value = lstHOrdenes[i].precio; //locked_mx        //total en mx en transaccion
                Item.Cells[4].Value = lstHOrdenes[i].cantmonedas.ToString();     //total en momendas en orden
                Item.Cells[5].Value = "0.0"; //Envio pendiente
                Item.Cells[6].Value = (lstHOrdenes[i].precio * lstHOrdenes[i].cantmonedas).ToString(); //Total mx         //Total del valor moneda y la orden lanzada
                Item.Cells[8].Value = lstHOrdenes[i].id;  //colid              //el id generado  por la orden.

            
                DataGridViewLinkCell links = new DataGridViewLinkCell();

                links.UseColumnTextForLinkValue = true;                        
                links.ActiveLinkColor = Color.White;
                links.LinkBehavior = LinkBehavior.SystemDefault;
                links.LinkColor = Color.Blue;
                links.TrackVisitedState = true;
                links.VisitedLinkColor = Color.YellowGreen;                        
                links.Value = "Canc. Ord.";
                Item.Cells[7] = links;
                bOrdenActiva = true;
                
                dtgMismonedas.Rows.Add(Item);
                //iRen += 1;              
            }
        }
        private void btor_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Se preciono btn del lvmismonedas" );
        }

        /// <summary>
        /// actualizaTotalMXmonedas : metodo para actualizar el valor MX dependiendo el valor de cada moneda en el grid de monedas en orden.
        /// se ejecuta en un hilo cada N segundos [timer1_Tick].
        /// </summary>
        private void actualizaTotalMXmonedas()
        {
            string sMoneda = string.Empty;
            double dPrecio = 0.0;
            double dTotMoneda = 0.0;
            double dEnorden=0.0;

            
            for (int index = 0; index < dtgMismonedas.Rows.Count; index++)
            {
                if (dtgMismonedas.Rows[index].Cells[0].Value != null)
                {
                    sMoneda = dtgMismonedas.Rows[index].Cells[0].Value.ToString();
                    dTotMoneda = Convert.ToDouble(dtgMismonedas.Rows[index].Cells[1].Value);
                    dEnorden = Convert.ToDouble(dtgMismonedas.Rows[index].Cells[2].Value);

                    for (int s = 0; s < lvMonedas.Items.Count; s++)
                    {
                        string sM = lvMonedas.Items[s].Name.Substring(0, 3);
                        string smd = lvMonedas.Items[s].Name.Substring(4, 3);
                        if ((sM == sMoneda) && (smd == "mxn") && dEnorden == 0 || sMoneda == lvMonedas.Items[s].Name.Trim() && dEnorden == 0)
                        {
                            if (lvMonedas.Items[s].SubItems[1].Text != null)
                            {
                                dPrecio = Convert.ToDouble(lvMonedas.Items[s].SubItems[1].Text.Trim());
                                dtgMismonedas.Rows[index].Cells[6].Value = string.Format("{0,12:#####.00#}", (dPrecio * dTotMoneda));
                            }
                        }
                    }//for
                }//if                
            }//for
        }

        ///private void actualizaGrafica(CDataPrice moneyp)
        private void actualizaGrafica(CMoneyPrice moneyp)
        {
            System.Windows.Forms.DataVisualization.Charting.DataPoint item = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
            DateTime dt1 = DateTime.Now;

            if (cbomonedas_grafica.SelectedIndex >= 0 && moneyp.book.Trim() == cbomonedas_grafica.SelectedItem.ToString().Trim() )
            {
                
                //item.SetValueY(new object[] { Convert.ToDouble(moneyp.ask) }); //Esto para cuando es de linea
                item.YValues = new double[]{ Convert.ToDouble(moneyp.ask)};
                if (dt1.TimeOfDay >= dthora.TimeOfDay)
                {
                    //item.XValue = dthora.ToOADate();
                    item.XValue = dt1.ToOADate();
                    dthora = dt1;
                }
                item.ToolTip = moneyp.ask + " - (" + DateTime.Now.ToString("G").ToString() + ")";
                chart1.Series[moneyp.book].Points.Add(item);
                chart1.Series[moneyp.book].LabelAngle = 90; //para poner los labels del eje X en vertical "la hora"
            }
            else if( cbomonedas_grafica.SelectedIndex < 0 && moneyp.book== "xrp_mxn")            
            {
                
                //item.SetValueY( new object[] {  Convert.ToDouble(moneyp.high), Convert.ToDouble(moneyp.low),Convert.ToDouble(moneyp.ask), Convert.ToDouble(moneyp.last) });
                //item.SetValueY(new object[] { Convert.ToDouble(moneyp.ask) }); //Esto para cuando es de linea
                item.YValues = new double[] {Convert.ToDouble(moneyp.ask)};
                if (dt1.TimeOfDay >= dthora.TimeOfDay)
                {
                    //item.XValue = dthora.ToOADate();
                    item.XValue = dt1.ToOADate();
                    dthora = dt1;
                }

                //chart1.Series["Monedas"].Points.Add(item);
                //chart1.Series["Monedas"].Points.AddXY(moneyp.book, moneyp.ask);
                //chart1.Series.Contains( moneyp.book);
                //chart1.Series.Add(moneyp.book).Points.AddXY(moneyp.book, moneyp.ask);

                /*chart1.Series["Monedas"].XValueMember = "Precio";
                chart1.Series["Monedas"].YValueMembers = "High,Low,Open,Close";
                chart1.Series["Monedas"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
                chart1.Series["Monedas"].CustomProperties = "PriceDownColor= Red, PriceUpColor= Blue";
                chart1.Series["Monedas"]["OpenCloseStyle"] = "Triangle";
                chart1.Series["Monedas"]["ShowOpenclose"] = "Both";
                chart1.DataManipulator.IsStartFromFirst = true;    */
                //item.SetValueXY(new DateTime(moneyp.created_at.Ticks), new object[] { Convert.ToDouble(moneyp.ask), Convert.ToDouble(moneyp.high), Convert.ToDouble(moneyp.low), Convert.ToDouble(moneyp.last) });
               
                //chart1.Series["Monedas"].Points.Add(item);
                //item.ToolTip = moneyp.book;

                item.ToolTip = moneyp.ask + " - (" + DateTime.Now.ToString("G").ToString() +")";
                chart1.Series[moneyp.book].Points.Add(item);
                chart1.Series[moneyp.book].LabelAngle = 90; //para poner los labels del eje X en vertical "la hora"

                //lTimeAnt = (DateTime.Now.Ticks) / 10000000;
                //validaAlertas(moneyp.book, moneyp.ask);
            }

            if (moneyp.book.Substring(moneyp.book.Length - 3, 3) == "mxn")
            {
                //en es esta lista almacena moneda,precio,tiempo cada N segundos en las 24 horas del dia.
                lMoneyHis_day.Add(new CHistoricoMonedas_dia { moneda = moneyp.book, precio = double.Parse(moneyp.ask), tiempo = DateTime.Parse( DateTime.Now.ToString("G")) });
                if (iCantMoneyGrafica < cbomonedas_grafica.Items.Count )
                {
                    iCantMoneyGrafica += 1;

                    if (iCantMoneyGrafica == cbomonedas_grafica.Items.Count)
                    {
                        lTimeAnt = (DateTime.Now.Ticks) / 10000000;
                        iCantMoneyGrafica = 0;// se reinicia para cada # de segundos se valide de nuevo la cantidad de monedas ..mxn y se actualice en la lista de monedas a graficar.
                    }
                }
            }

        }

        private void btnGrafica_Click(object sender, EventArgs e)
        {               
            if (!bGrafica)
            {
                this.Width = 1206; // 1176; // 1167;
                this.Height = 763; // 720;
                btnGrafica.Text = "&Grafica <<";
                bGrafica = true;
            }
            else
            {
                this.Width = 720;
                this.Height = 763; // 785;// 720;
                btnGrafica.Text = "&Grafica >>";
                bGrafica = false;
            }
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {               
            if (this.dtgtrading.Columns[e.ColumnIndex].Name == "dgcolDir")
            {
                /*dgcolPathFile.ValueType = typeof(string);
                DataGridViewCell celFile = dataGridView1.Rows[e.RowIndex].Cells["dgcolPathFile"];
                string stexto = string.Empty;
                if (celFile.Value is DBNull)
                {
                    stexto = celFile.Value.ToString();
                }
                
                 string sFile = (string)dataGridView1.Rows[e.RowIndex].Cells["dgcolPathFile"].Value.ToString();
                 double dMax = (double)dataGridView1.Rows[e.RowIndex].Cells["dgcolMax"].Value;
                 double dMin = (double)dataGridView1.Rows[e.RowIndex].Cells["dgcolMin"].Value;
                 string sMoneda = (string)dataGridView1.Rows[e.RowIndex].Cells["dgcolMoneda"].Value.ToString();
                 bool bchek = (bool)dataGridView1.Rows[e.RowIndex].Cells["dgcolchek"].Value;
                  */
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && this.dtgtrading.Columns[e.ColumnIndex].Name == "dgcolPlay" && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                DataGridViewButtonCell celBoton = this.dtgtrading.Rows[e.RowIndex].Cells["dgcolPlay"] as DataGridViewButtonCell;
                Icon icoAtomico = new Icon(Environment.CurrentDirectory + @"..\..\..\imagenes\play_ico.ico", 24, 24);

                //Icon icoAtomico = new Icon(@"E:\proyectos\btcTrading_26_jun\imagenes\play_ico.ico", 24, 24);
                //Icon icoAtomico = new Icon(@"C:\Proyectos\ARGENTINA\Utilerias\btcTrading_26_jun\imagenes\play_ico.ico", 24, 24);
                
                
                e.Graphics.DrawIcon(icoAtomico, e.CellBounds.Left +3/*+ 3*/, e.CellBounds.Top +3 /* + 3*/);

                this.dtgtrading.Rows[e.RowIndex].Height = icoAtomico.Height +8; // +10;
                this.dtgtrading.Columns[e.ColumnIndex].Width = icoAtomico.Width +10;// +10;

                e.Handled = true;
            }
            if (e.ColumnIndex >= 0 && this.dtgtrading.Columns[e.ColumnIndex].Name == "dgcolDir" && e.RowIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                
                DataGridViewButtonCell celBoton = this.dtgtrading.Rows[e.RowIndex].Cells["dgcolDir"] as DataGridViewButtonCell;
                Icon icoAtomico = new Icon(Environment.CurrentDirectory + @"..\..\..\imagenes\open_ico.ico", 24, 24);
                //Icon icoAtomico = new Icon(@"E:\proyectos\btcTrading_26_jun\imagenes\open_ico.ico", 24, 24);
                //Icon icoAtomico = new Icon(@"C:\Proyectos\ARGENTINA\Utilerias\btcTrading_26_jun\imagenes\open_ico.ico", 24, 24);

                e.Graphics.DrawIcon(icoAtomico, e.CellBounds.Left + 3/*+ 3*/, e.CellBounds.Top + 3 /* + 3*/);

                this.dtgtrading.Rows[e.RowIndex].Height = icoAtomico.Height + 8; // +10;
                this.dtgtrading.Columns[e.ColumnIndex].Width = icoAtomico.Width + 8;// +10;

                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (this.txtPorcentaje.Text == string.Empty)
            //{
            //    MessageBox.Show("Introducir valor para el porcentaje");
            //    this.txtPorcentaje.Focus();
            //}
            //else
            //{
            //    if (this.dataGridView1.Columns[e.ColumnIndex].Name == "dgcolDir")
            //    {
            //        //decimal nListPrice = (decimal)this.dataGridView1.Rows[e.RowIndex].Cells["ListPrice"].Value;
            //        //int nPorcentaje = int.Parse(this.txtPorcentaje.Text);
            //        //decimal nDescuento = (nListPrice * nPorcentaje) / 100;

            //        //MessageBox.Show(nDescuento.ToString("#,#.##"), "Descuento aplicable");
            //    }
            //}
            if (/*dataGridView1.CurrentCell == null || dataGridView1.CurrentCell.Value == null || */e.RowIndex == -1) 
                return;
            if (this.dtgtrading.Columns[e.ColumnIndex].Name == "dgcolDir")
            {                  

                OpenFileDialog oDlg = new OpenFileDialog();
                oDlg.InitialDirectory = Application.StartupPath;
                oDlg.RestoreDirectory = true;
                oDlg.Filter = "Archivos de Audio (*.mp3)|*.mp3|All files (*.*)|*.*";
                oDlg.CheckFileExists = true;
                oDlg.CheckPathExists = true;
                oDlg.Multiselect = false;

                if (oDlg.ShowDialog() == DialogResult.OK)
                {
                    dtgtrading.Rows[e.RowIndex].Cells["dgcolPathFile"].Value = oDlg.FileName;
                }                
            }
            else if (this.dtgtrading.Columns[e.ColumnIndex].Name == "dgcolPlay")
            {
                DataGridViewCell celFile = dtgtrading.Rows[e.RowIndex].Cells["dgcolPathFile"];
                /*DataGridViewCell celMax = dataGridView1.Rows[e.RowIndex].Cells["dgcolMax"];
                DataGridViewCell celMin = dataGridView1.Rows[e.RowIndex].Cells["dgcolMin"];
                DataGridViewCell celMoneda = dataGridView1.Rows[e.RowIndex].Cells["dgcolMonedas"];
                DataGridViewCell celChek = dataGridView1.Rows[e.RowIndex].Cells["dgcolchek"];*/
                //|| celMax.Value is DBNull || celMin.Value is DBNull || celMoneda.Value is DBNull || celChek.Value is DBNull
                if ((celFile.Value is DBNull) || (celFile.Value == null))
                {
                    MessageBox.Show("No hay Ningun Archivo de Audio a ejecutar!!");
                    return;
                }                

                if (bPlay)
                {
                    wplayer.controls.stop();
                    bPlay = false;
                }
                else
                {
                    wplayer.URL = celFile.Value.ToString();
                    //wplayer.controls.play();
                    bPlay = true;
                    //DataGridViewCellPaintingEventArgs ea = new DataGridViewCellPaintingEventArgs();                    
                    //TextBox tx = new TextBox();
                    //tx.Text = "stop";
                    //Object send = new object();
                    //((TextBox)send).Text = "stop";
                    //dataGridView1_CellPainting(send, ea);
                }
            }   
        }//

        private bool getGridAlerts()
        {
            bool bRegresa = false;
            lAlertas.Clear();
            for (int i = 0; i < dtgtrading.Rows.Count-1; i++)
            {
                DataGridViewCell celFile = dtgtrading.Rows[i].Cells["dgcolPathFile"];
                DataGridViewCell celMax = dtgtrading.Rows[i].Cells["dgcolMax"];
                DataGridViewCell celMin = dtgtrading.Rows[i].Cells["dgcolMin"];
                DataGridViewCell celMoneda = dtgtrading.Rows[i].Cells["dgcolMonedas"];
                DataGridViewCell celChekmax = dtgtrading.Rows[i].Cells["dgcolchekmax"];
                DataGridViewCell celChekmin = dtgtrading.Rows[i].Cells["dgcolchekmin"];
                DataGridViewCell celChekri = dtgtrading.Rows[i].Cells["dgcolchekreinvertir"];                
                DataGridViewCell celValor = dtgtrading.Rows[i].Cells["dgcolPorciento"];
                
                if ((celFile.Value is DBNull) || (celFile.Value == null) || celMax.Value is DBNull || celMin.Value is DBNull || celMoneda.Value is DBNull || celChekmax.Value is DBNull || celChekmin.Value is DBNull)
                {
                   // return;
                }
                else
                {
                    lAlertas.Add(new CDatosAlertas() { file = celFile.Value.ToString(), maximo = double.Parse(celMax.Value.ToString()), minimo = double.Parse(celMin.Value.ToString()), moneda = celMoneda.Value.ToString(), chkmax = bool.Parse(celChekmax.Value.ToString()),chkmin= bool.Parse(celChekmin.Value.ToString()),chkri= bool.Parse(celChekri.Value.ToString()) ,valor= double.Parse(celValor.Value.ToString())});
                    bRegresa = true;
                }
            }
            return bRegresa;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ( char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
                e.Handled =false;
        }
      
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if ((dtgtrading.Rows.Count - 1) < 1)
                {
                    dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolPathFile"].Value = "";
                    dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolMax"].Value = "0.0";
                    dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolMin"].Value = "0.0";
                    dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolMonedas"].Value = "";
                    dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolchek"].Value = "false";
                }
                else
                {
                    string sMoneda = string.Empty;
                    DataGridViewCell celMoneda = dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolMonedas"];
                    if ((celMoneda.Value is DBNull) || (celMoneda.Value == null))
                    {
                        if( !(dtgtrading.CurrentRow.Index == dtgtrading.Rows.Count -1) ) //No puede borrar el ultimo renglon nuevo bacio.
                            dtgtrading.Rows.RemoveAt(dtgtrading.CurrentRow.Index);
                    }
                    else
                    {
                        sMoneda = dtgtrading.Rows[dtgtrading.CurrentRow.Index].Cells["dgcolMonedas"].Value.ToString();
                        if (MessageBox.Show("Decea Eliminar la Alerta de esta moneda[" + sMoneda + "]..?", "Alertas..", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            dtgtrading.Rows.RemoveAt(dtgtrading.CurrentRow.Index);
                            if (bPlay)
                            {
                                wplayer.controls.stop();
                                bPlay = false;
                            }
                        }
                    }                  
                }                
            }
        }

        //metodo para darle formato a las celadas de precio ##.##
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                DataGridViewCell cell = dtgtrading.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Value = Convert.ToDecimal(cell.Value).ToString("N2");
                dtgtrading.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
            }
        }

        private bool validaTiempoGraficaUpdate()
        {           
            long l2 =(DateTime.Now.Ticks)/10000000;

            if ((l2 - lTimeAnt) > 35)
               return true;
            else
                return false;
        }

        private void validaAlertas(string sMoneda, double dPrecio)
        {
            bool bOrden = false;
            for (int i = 0; i < dtgtrading.Rows.Count-1; i++)
            {
                DataGridViewCell celMax = dtgtrading.Rows[i].Cells["dgcolMax"];
                DataGridViewCell celMin = dtgtrading.Rows[i].Cells["dgcolMin"];
                DataGridViewCell celMoneda = dtgtrading.Rows[i].Cells["dgcolMonedas"];
                DataGridViewCell celChekmin = dtgtrading.Rows[i].Cells["dgcolchekmin"];
                DataGridViewCell celChekmax = dtgtrading.Rows[i].Cells["dgcolchekmax"];
                DataGridViewCell celFile = dtgtrading.Rows[i].Cells["dgcolPathFile"];
                DataGridViewCell celPorciento = dtgtrading.Rows[i].Cells["dgcolPorciento"];
                DataGridViewCell celReinvertir = dtgtrading.Rows[i].Cells["dgcolchekreinvertir"];
                DataGridViewCell celEnOrden = dtgtrading.Rows[i].Cells["dgcolchekreinvertir"];

                if (celMax.Value == null)
                    celMax.Value = 0;
                if (celMin.Value == null)
                    celMin.Value = 0;
                if (celFile.Value == null)
                    celFile.Value = "";
                if (celPorciento.Value == null)
                    celPorciento.Value = 0;
                if( celChekmin.Value == null)
                    celChekmin.Value=false;
                if( celChekmax.Value == null)
                    celChekmax.Value=false;
                if (celReinvertir.Value == null)
                    celReinvertir.Value = false;
                bOrden = false;

                //CTrading_Bitso.CMoney_Trading_oid moneyTrading = null;
                OrderPlaced order = new OrderPlaced();//clase para captuarar datos de la orden a ejecutar.
                Trading_oid trading_id = new Trading_oid();

                /// Trading #1
                if ((celMoneda.Value != null) && (celMax.Value != null || celMin.Value != null) )
                {
                    //si el precio Actual es menor que lo que se configuro la alerta.
                    if (celMoneda.Value.ToString() == sMoneda && (double.Parse(celMax.Value.ToString()) < dPrecio))
                    {
                        if (bPlay) // aqui no se ocupa que entre
                        {
                            //wplayer.controls.stop();  
                            //bPlay = false;
                        }
                        else
                        {
                            if (celFile.Value != null && celFile.Value.ToString().Trim() !="" )
                            {
                                wplayer.URL = celFile.Value.ToString();
                                //wplayer.controls.play();
                                bPlay = true;
                            }

                            //1 si el precio es menor por lo menos en un lapso de 3 veces seguidas vender y tomendo en cuenta la hora.
                            //1.1si se mantiene el precio entre el valor menor tomar en cunta un % tam bien.
                            //2 - si es mayor tomaar en cuenta la tendencia si va subiendo dejar hata que la tendencia sea a bajar.
                            //2.1- si empiea a bajar tomar en cuenta minimo 3 tenencia bajistas y la hora antes de hacer el trading.
                            //3.- Tomar en cuenta el % de minimo o maximo para compra y venta sobre el precio actual.


                            if (bool.Parse( celChekmax.Value.ToString() ) )
                            {
                                if (bool.Parse(celReinvertir.Value.ToString()))
                                {
                                    List<stTendencia> lt3_3 = ltendencia3_3.FindAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });
                                    if (lt3_3.Count == 3)
                                    {
                                        int itrue = 0;
                                        string sTendencia = string.Empty;
                                        for (int t = 0; t < lt3_3.Count; t++)
                                        {
                                            //valida si hay mas verdaderos que falsos, si es asi el precio de la moneda esta subiendo, contrario baja.
                                            if (lt3_3[t].bSubeBaja)
                                                itrue += 1;
                                        }
                                        //int id2 = 0;
                                        for ( int id=0; id < lvMonedas.Items.Count; id++)
                                        {
                                            if (lvMonedas.Items[id].Text.Trim() == sMoneda)
                                            {
                                               sTendencia= lvMonedas.Items[id].SubItems[6].Text.Trim();  //tendencia1
                                               //id2 = id;
                                               id = lvMonedas.Items.Count;// para que se salga del for
                                            }
                                        }
                                        if (itrue >= 2 && sTendencia.Substring(0,1).Trim() == "↑" ) //"subir")// tiene tendencia a seguir subiendo, ahi que esperar para vende mas caro.
                                        {
                                            bOrden = false;
                                            //lvMonedas.Items[id2].SubItems[6].Text = "subir";//Tendencia2
                                        }
                                        else
                                        {// si hay mas falsos quiere decir que ha empezado a bajar por lo tanto ahi que vender.
                                            bOrden = true;
                                           // lvMonedas.Items[id2].SubItems[6].Text = "bajar";//Tendencia2
                                        }
                                        //ltendencia3_3.RemoveAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });//borra todos los elemento que encuentre.
                                    }//if (lt3_3.Count == 3)
                                }
                                else
                                {
                                    bOrden= true;
                                }
                            }
                            else
                            {
                                bOrden= false;
                            }

                            if(  bOrden ) //si se lanza  una orden de compara/venta, entra por aqui.
                            {
                                
                                for (int s = 0; s < dtgMismonedas.Rows.Count; s++)
                                {
                                    string sM = dtgMismonedas.Rows[s].Cells[0].Value.ToString().Substring(0, 3);//nombre moneda ultimas 3 caracteres
                                    double dDisponible = double.Parse(dtgMismonedas.Rows[s].Cells[1].Value.ToString() ); //disponibilidad
                                    
                                    if ( double.Parse( celPorciento.Value.ToString()) > 0.0 )//si es mayor , aqui solo se toma la cantidad que este en col. $Valor para lanzar la orden
                                    {
                                        dDisponible = double.Parse(celPorciento.Value.ToString());
                                    }
                                    string sMonedaT = string.Format("{0}_{1}", sM.Trim(), "mxn");
                                    // 10 dlls en btc= 0,0008759
                                    if ((sMoneda == sMonedaT && dDisponible > 10) || (sMoneda == sMonedaT && sM == "btc" && dDisponible > .0008759))
                                    {
                                        //CTrading_Bitso tb = new CTrading_Bitso();
                                        //tb.buym.book = sMoneda;
                                        order.book = sMoneda;
                                        //tb.buym.major = string.Format("{0,14:F8}",(double.Parse(lvmimonedas.Items[s].SubItems[1].Text)) ).Trim();
                                        //tb.buym.major = string.Format("{0,8:F6}", dDisponible.ToString().Trim() );
                                        order.major = string.Format("{0,8:F6}", dDisponible.ToString().Trim());
                                        if (sMoneda == "btc_mxn")
                                        {
                                            //tb.buym.price = (dPrecio ).ToString();
                                            order.price = (dPrecio).ToString(); 
                                        }
                                        else if (sMoneda == "xrp_mxn")
                                        {
                                            //tb.buym.major = string.Format("{0:#####.##}", dDisponible.ToString("N2") ).Trim();
                                            //tb.buym.major = string.Format("{0,5:N2}", dDisponible -0.1);
                                            //tb.buym.price = (dPrecio + .2).ToString();
                                            order.major = string.Format("{0,5:N2}", dDisponible - 0.1);
                                            if (bool.Parse(celReinvertir.Value.ToString()))
                                                order.price = dPrecio.ToString(); // (dPrecio + .2).ToString();
                                            else
                                            {
                                                for (Int32 v1 = 0; v1 < dtgtrading.Rows.Count; v1++)
                                                {
                                                    if (sMoneda == dtgtrading.Rows[v1].Cells[0].Value.ToString())
                                                    {
                                                        order.price = dtgtrading.Rows[v1].Cells[1].Value.ToString();//es el precio que pactamos en el grid como maximo
                                                        v1 = dtgtrading.Rows.Count + 1;//para que se salga del siclo
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //tb.buym.price = (dPrecio + .5).ToString();
                                            order.price= (dPrecio + .5).ToString();
                                        }

                                        //tb.buym.side = "sell";
                                        //tb.buym.type = "limit";

                                        order.side = "sell";
                                        order.type = "limit";
                                        //moneyTrading = tb.bitsoTrading(sMoneda);
                                         generaorden(order);
                                         //validar aqui , si regresa true la transaccion destildar para que no se ejecute de nuevo la venta.
                                        if (!bool.Parse(celReinvertir.Value.ToString()))
                                        {
                                            dtgtrading.Rows[i].Cells["dgcolchekmax"].Value = false;
                                            celChekmax.Value = false;
                                        }
                                        dtgtrading.Refresh();
                                        if (trading_id.oid != "" && trading_id.oid != null)
                                        { 
                                            //validacion para actualizar el maximo si sige subiendo, se toma en cuenta el ultimo maximo para el proximo trading
                                            if (double.Parse(celMax.Value.ToString()) < dPrecio && bool.Parse(celReinvertir.Value.ToString()))
                                            {
                                                celMax.Value = dPrecio;
                                                dtgtrading.Rows[i].Cells["dgcolMax"].Value = dPrecio; 
                                            }
                                            dtgMismonedas.Rows[s].Cells[3].Value = order.price;
                                        }
                                    }//if ((sMoneda == sMonedaT && dDisponible > 10) ||....
                                }//for
                            }//if bOrden
                           // }//if bool
                            
                        }//else if play
                    }
                    else if( celMoneda.Value.ToString() == sMoneda &&  dPrecio < double.Parse(celMin.Value.ToString()) )
                    {
                        if ( bool.Parse(celChekmin.Value.ToString() ))
                        {
                            //aqui inicia validaciones para ver si es viable seguir esperando a que baje para obtener mayor ganacia de una moneda antes de lanzar la orden de compra/vrnta
                            if (bool.Parse(celReinvertir.Value.ToString()))
                            {
                                List<stTendencia> lt3_3 = ltendencia3_3.FindAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });
                                if (lt3_3.Count == 3)
                                {
                                    int ifalse = 0;
                                    string sTendencia = string.Empty;
                                    for (int t = 0; t < lt3_3.Count; t++)
                                    {
                                        //valida si hay mas verdaderos que falsos, si es asi el precio de la moneda esta subiendo, contrario baja.
                                        if (!lt3_3[t].bSubeBaja)
                                            ifalse += 1;
                                    }
                                    //int id2 = 0;
                                    for (int id = 0; id < lvMonedas.Items.Count; id++)
                                    {
                                        if (lvMonedas.Items[id].Text.Trim() == sMoneda)
                                        {
                                            sTendencia = lvMonedas.Items[id].SubItems[6].Text.Trim(); //tendencia1
                                            //id2 = id;
                                            id = lvMonedas.Items.Count;// para que se salga.
                                        }
                                    }
                                    if (ifalse >= 2 && sTendencia.Substring(0, 1).Trim() == "↓") // "bajar")// tiene tendencia a seguir subiendo, ahi que esperar para vende mas caro.
                                    {
                                        bOrden = false;
                                        //lvMonedas.Items[id2].SubItems[6].Text = "bajar";//Tendencia2
                                    }
                                    else
                                    {// si hay mas falsos quiere decir que ha empezado a bajar por lo tanto ahi que vender.
                                        bOrden = true;
                                        //lvMonedas.Items[id2].SubItems[6].Text = "subir"; //Tendencia2
                                    }
                                    //ltendencia3_3.RemoveAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });//borra todos los elemento que encuentre.
                                }//if (lt3_3.Count == 3)
                            }
                            else
                            {
                                bOrden = true;
                            }
                        }
                        else
                        {
                            bOrden = false;
                        }

                        if(bOrden )
                        {
                            //for (int s = 0; s < lvmimonedas.Items.Count; s++)
                            for (int s = 0; s < dtgMismonedas.Rows.Count; s++)
                            {
                                //string sM = lvmimonedas.Items[s].Name.Substring(0, 3);
                                //double dDisponible = double.Parse(lvmimonedas.Items[s].SubItems[1].Text);
                                string sM = dtgMismonedas.Rows[s].Cells[0].Value.ToString().Substring(0, 3);
                                double dDisponible = double.Parse(dtgMismonedas.Rows[s].Cells[1].Value.ToString());

                                string sMonedaT = string.Format("{0}_{1}", sM.Trim(), "mxn");
                                //10 dolares en btc= 0,0008759
                                if (sM.Trim() == "mxn" && dDisponible > 20) // para comprar monedas se hace con moneda mexicana.
                                {
                                    //CTrading_Bitso tb = new CTrading_Bitso();  //se inhibe para usar clase generica 26jul2019
                                    //tb.buym.book = sMoneda;
                                    order.book = sMoneda;
                                    if ( double.Parse(celPorciento.Value.ToString() ) > 0.0)
                                    {
                                        dDisponible = double.Parse(celPorciento.Value.ToString() );
                                    }
                                    
                                   // tb.buym.major = string.Format("{0,2:N2}", double.Parse(lvmimonedas.Items[s].SubItems[1].Text.ToString()) );
                                    if (sMoneda == "btc_mxn")
                                    {
                                        //N es para numeros naturales ...string.Format("{0,14:N0}"                                        
                                        //tb.buym.major = string.Format("{0,14:F5}", (dDisponible / (dPrecio * 1.01))).Trim();
                                        //tb.buym.price = (dPrecio * .995).ToString();
                                        order.major = string.Format("{0,14:F5}", (dDisponible / (dPrecio * 1.01))).Trim();
                                        order.price = (dPrecio * .995).ToString();
                                    }
                                    else
                                    {
                                        
                                        //tb.buym.major = string.Format("{0,14:N2}", (dDisponible / (dPrecio + .07))).Trim();
                                        order.major = string.Format("{0,14:N2}", (dDisponible / (dPrecio + .07))).Trim();
                                        if (bool.Parse(celReinvertir.Value.ToString()))
                                            //tb.buym.price = dPrecio.ToString(); // (dPrecio + .2).ToString();
                                            order.price = dPrecio.ToString(); 
                                        else
                                        {
                                            for (Int32 v1 = 0; v1 < dtgtrading.Rows.Count; v1++)
                                            {
                                                if (sMoneda == dtgtrading.Rows[v1].Cells[0].Value.ToString())
                                                {
                                                    order.price = dtgtrading.Rows[v1].Cells[2].Value.ToString();//es el precio que pactamos en el grid como Minimo
                                                    v1 = dtgtrading.Rows.Count + 1;//para que se salga del siclo
                                                }
                                            }
                                        }
                                        //tb.buym.price = (dPrecio - .4).ToString();
                                    }
                                    /*tb.buym.side = "buy";
                                    tb.buym.type = "limit";*/
                                    order.side = "buy";
                                    order.type = "limit";
                                    generaorden(order); // tb.bitsoTrading(sMoneda);
                                    //validar aqui , si regresa true la transaccion destildar para que no se ejecute de nuevo la venta.
                                    if (!bool.Parse(celReinvertir.Value.ToString()))
                                    {
                                        dtgtrading.Rows[i].Cells["dgcolchekmin"].Value = false;
                                        celChekmin.Value = false;
                                    }
                                    dtgtrading.Refresh();
                                    if (trading_id.oid != "" && trading_id.oid != null)
                                    {
                                        dtgMismonedas.Rows[s].Cells[3].Value = order.price; // tb.buym.price;

                                        //validacion para actualizar el minimo si sige bajando, se toma en cuenta el ultimo minimo para el proximo trading
                                        if (double.Parse(celMin.Value.ToString()) > dPrecio && bool.Parse(celReinvertir.Value.ToString()))
                                        {
                                            celMin.Value = dPrecio;
                                            dtgtrading.Rows[i].Cells["dgcolMin"].Value = dPrecio;
                                        }
                                    }
                                }//if
                            }//for
                            
                        }//if check= true
                       
                    }
                }// fin validaciones de vacios.
                
            }
        }//fin metodo


        private bool generaorden(OrderPlaced orden)
        {
            string sPlataforma = txtplataforma.Text.Trim().ToUpper();
            bool bRegresa = false;

            switch (sPlataforma)
            {
                case "BITSO":
                    {
                        Bitso bitsoClient = new Bitso(sApi_key, sApi_pwd, true);
                        var trad =bitsoClient.PlaceOrder(orden);
                        if (trad.Oid != "" && trad.Oid != null)
                        {
                            foreach ( var bal in bitsoClient.GetAccountBalance() )
                            {
                                lmiBalances.Add(new CBalance { currency = bal.Currency, available = bal.Available, locked = bal.Locked, total = bal.Total, pending_deposit = "0.0", pending_withdrawal = "0.0" });
                            }                            
                            actualizaListVewmiMonedas();
                            //obtener los datos de precio de la moneda a vender                            
                            lultPrecioOrden.Add(new CMoneda_Movto { moneda = orden.book, valorMax = double.Parse(orden.price), tiempoMax = DateTime.Parse(DateTime.Now.ToString("G")) });

                            //validacion para actualizar el maximo si sige subiendo, se toma en cuenta el ultimo maximo para el proximo trading
                           /* if (double.Parse(celMax.Value.ToString()) < dPrecio && bool.Parse(celReinvertir.Value.ToString()))
                            {
                                celMax.Value = dPrecio;
                                dataGridView1.Rows[i].Cells["dgcolMax"].Value = dPrecio;
                            }*/
                            bOrdenActiva = true;//variable para validar si hay ordenes activas
                            //hilo para validar los movimientos de moneda en el grid de ordenes activas.
                            if (!thrOrdenes.IsAlive)
                            {
                                trdstartOrdenes = new ThreadStart(validaOrdenesActivas);
                                thrOrdenes = new Thread(trdstartOrdenes);
                                thrOrdenes.IsBackground = true;
                                thrOrdenes.Name = "thrOrdenes";
                                thrOrdenes.Start(); //hilo que valida las ordenes activas.
                            }
                            //dtgMismonedas.Rows[s].Cells[3].Value = tb.buym.price;
                            //dtgMismonedas.Rows[s].Cells[3].Value = orden.price;
                            //lstHOrdenes.Add(new FrmHistorialOrdenes.ChistorialOrdenes { moneda = sMoneda, precio = double.Parse(tb.buym.major), valor = double.Parse(tb.buym.price), hora = DateTime.Now, orden = "sell", status = "Open", id = moneyTrading.oid});
                            lstHOrdenes.Add(new COrdenes{ moneda = orden.book, precio = double.Parse(orden.major), valor = double.Parse(orden.price), hora = DateTime.Now, orden = "sell", status = "Open", id = trad.Oid });
                            bRegresa = true;
                        }
                    }
                    break;

            }
            return bRegresa;
        }
        private void validaMovtoMoneda(string sMoneda, double dValor)
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToString("G"));

            if (lMoneymvto.Exists( x=> x.moneda == sMoneda ))
            {                
                int index = lMoneymvto.FindIndex(delegate(CMoneda_Movto cm)
                {
                    if (cm.moneda == sMoneda)
                        return true;
                    else
                        return false;
                });

                if (sMoneda.Substring(sMoneda.Length - 3, 3) == "mxn")
                {
                    if (validaFechaMovtoMoneda(lMoneymvto[index], index))
                    {
                        lMoneymvto[index].valorMax = dValor;
                        lMoneymvto[index].tiempoMax = dt;
                        lMoneymvto[index].valorMin = dValor;
                        lMoneymvto[index].tiempoMin = dt;
                        //lMoneymvto[index].valorClose = double.Parse(consultapreciomoneda(sMoneda));//modificar aqui, poner el valor de cierre.
                        lMoneymvto[index].valorOpen = dValor;
                    }
                    validaTendenciaMoneda(sMoneda, dValor); //valida la tendencia de la moneda si sube o si tiende a bajar.
                }

                if (lMoneymvto[index].valorMax < dValor)
                {
                    lMoneymvto[index].valorMax = dValor;
                    lMoneymvto[index].tiempoMax= dt;
                }
                else if (lMoneymvto[index].valorMin > dValor)
                {
                    lMoneymvto[index].valorMin = dValor;
                    lMoneymvto[index].tiempoMin = dt;
                }
            
                //string sTexto = string.Format("moneda:{0}, valmax={1}, valmin={2},fechamin={3},fechamax={4}", lMoneymvto[index].moneda, lMoneymvto[index].valorMax, lMoneymvto[index].valorMin, lMoneymvto[index].tiempoMin, lMoneymvto[index].tiempoMax);
                //txtlogerror.AppendText(sTexto);
                //txtlogerror.AppendText("\n");
            }
            else
            {
                lMoneymvto.Add(new CMoneda_Movto { moneda = sMoneda, valorMax = dValor, valorMin = dValor, tiempoMax = dt, tiempoMin = dt });
            }
            //lMoneymvto
        }

        /// <summary>
        /// consultapreciomoneda , metodo para consultar el precio que tiene la moneda en el listview
        /// </summary>
        /// <param name="sMoneda">Nombre de la moneda a consultar</param>
        /// <returns>Regresa el precio de la moneda tipo string</returns>
        private string consultapreciomoneda(string sMoneda)
        {
            string sPrecio = "0.0";
            
            foreach (ListViewItem dato in lvMonedas.Items)
            {
                if (dato.SubItems[0].Text.ToString().Trim() == sMoneda.Trim())
                {
                    return dato.SubItems[1].Text.ToString();
                }
            }
            return sPrecio;
        } 

        /// <summary>
        /// validaFechaMovtoMoneda(CMoneda_Movto cm,int index)
        /// metodo para validar el cambio de dia para guardar el valor minimo y maximo de cada moneda en el archivo por mes
        /// </summary>
        /// <param name="cm">parametro de tipo clase donde almacena valores de la moneda del dia</param>
        /// <param name="index">parametro donde trae el indice donde trae el valor de la moneda que esta validando </param>
        /// <returns>Regresa valor verdadero si todo termino correctamente</returns>
        private bool validaFechaMovtoMoneda(CMoneda_Movto cm,int index)
        {
            bool bContinua = false;
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();
            string sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneymes.json", sAnio, sMes);
            
            if ( ( DateTime.Today.Day > cm.tiempoMin.Day) || (DateTime.Today.Month != cm.tiempoMin.Month))
            {
               // MessageBox.Show("la moneda["+cm.moneda +"]  es mayor:" + idia.ToString() + " que el dia:" + iDiam.ToString());
                bContinua = true;
                bGraficaMes = true;//para que recarge del archivo el nuevo dia guardado y se muestre en grafica mes.

                //esto es para validar cuando este en el ultimo dia del mes que lo cargado en la clase no lo guarde en el otro mes.
                //ejemplo si el mes trae 30 dias , el dia 30 lo guarde en el mes que corresponde no en el siguiente.
                if ((DateTime.Today.Month != cm.tiempoMin.Month) && Math.Abs((cm.tiempoMin.Day - DateTime.Today.Day)) > 1)
                {
                    sMes = sMeses.ElementAt(DateTime.Now.Month-1);
                    sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneymes.json", sAnio, sMes);
                }
            }
            
            //string sRuta = @"C:\sys\tradingboots\historial\histomoney.json";
            if (bContinua)
            {
                //StreamWriter wr = new StreamWriter(sRuta);
                cm.valorClose =  double.Parse(consultapreciomoneda(cm.moneda));// guarda el ultim pecio de la moneda.

                //if (!Directory.Exists(@"C:\sys\tradingboots\historial"))
                if (!Directory.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\",sAnio,sMes)) )
                    Directory.CreateDirectory(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\", sAnio, sMes));
                //Crear el archivo con la informacion

                var jsontxt = JsonConvert.SerializeObject(cm) + Environment.NewLine;
                if (!File.Exists(sRuta))
                {
                    File.WriteAllText(sRuta, jsontxt);
                }
                else
                {
                    File.AppendAllText(sRuta, jsontxt);
                }

                //using (FileStream fs = File.OpenWrite(sRuta)) // CreateText(sRuta))
                //{
                //    //var jsontxt = JsonConvert.SerializeObject(lMoneymvto[index]);
                //    var jsontxt = JsonConvert.SerializeObject(cm);
                    
                //    //wr.WriteLine(jsontxt);
                //    //wr.Write(jsontxt);
                //    //wr.Write("\n");

                //    Byte[] info = new UTF8Encoding(true).GetBytes(jsontxt);
                //    fs.Write(info, 0, info.Length);
                    
                //    //sw.Write(jsontxt);
                //    //sw.Write("\n");
                //    //sw.Close();
                //    fs.Close();
                //}

                lMoneyHis_day.Clear();
                LeerHistorialMonedasDia(); // recarga el historial de monedas precio maximo/minimo por dia, para grafica mensual.
                //this.cbomonedas_grafica_SelectedIndexChanged(cbomonedas_grafica,  null);
                actualizaGraficasMes_Dia();
//                borrarHistorial_ordenes_monedasDia();//borra los archivos generados de historial de monedasDia o ordenes del Dia Realizadas.
            }
            return bContinua;
        }

        private void btnActualizamonhistorial_Click(object sender, EventArgs e)
        {
            consultaHistorialMoney();
        }

        private bool consultaHistorialMoney()
        {
            bool bRegresa = false;

            string sMoneda = string.Empty, sTexto = string.Empty;
            if (cbomonedas.SelectedIndex >= 0)
            {
                sMoneda = cbomonedas.SelectedItem.ToString().Trim();
                if (sMoneda != "")
                {
                    if (lMoneymvto.Exists(x => x.moneda == sMoneda))
                    {
                        int index = lMoneymvto.FindIndex(delegate(CMoneda_Movto cm)
                        {
                            if (cm.moneda == sMoneda)
                                return true;
                            else
                                return false;
                        });
                        txtmonhistMax.Text = lMoneymvto[index].valorMax.ToString();
                        txtmonhistMin.Text = lMoneymvto[index].valorMin.ToString();
                        txtfechahoramax.Text = lMoneymvto[index].tiempoMax.ToLongTimeString();
                        txtFechaHoraMin.Text = lMoneymvto[index].tiempoMin.ToLongTimeString();
                        float fValordif=  (float.Parse(txtmonhistMax.Text) - float.Parse(txtmonhistMin.Text));
                        string svalor= string.Empty;
                        if( sMoneda.Substring( sMoneda.Trim().Length-3,3)=="mxn")
                            svalor = string.Format("Valor Dia - dif[ ${0:#####0.000} ]",fValordif );
                        else
                            svalor = string.Format("Valor Dia - dif[ {0:######0.00000000} ]",fValordif);
                        gboxvalordia.Text = svalor;// "Valor Dia - dif[" + fValordif.ToString() + "]";
                        bRegresa = true;
                    }
                    else
                    {
                        MessageBox.Show("La moneda no esta capturada para su historial");
                    }
                }
            }

            return bRegresa;
        }

        private void cbomonedas_grafica_SelectedIndexChanged(object sender, EventArgs e)
        {
            actualizaGraficasMes_Dia();
        }

        private void actualizaGraficasMes_Dia()
        {
            if (cbomonedas_grafica.SelectedIndex >= 0)
            {
                chart1.Series[0].Points.Clear();
                chart1.Series.Clear();
                
                chart1.Titles.Clear();
                double dMax = 0.0;
                double dMin = 0.0;

                int iMes = int.Parse(cbomesgh.SelectedItem.ToString().Substring(cbomesgh.SelectedItem.ToString().Length - 2, 2));
                //System.Windows.Forms.DataVisualization.Charting.DataPoint item = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                //item.SetValueY( new object[] {  Convert.ToDouble(moneyp.high), Convert.ToDouble(moneyp.low),Convert.ToDouble(moneyp.ask), Convert.ToDouble(moneyp.last) });
                //Agregar linea de grafica de la moneda

                Random rnd = new Random();
                Series serie = new Series(cbomonedas_grafica.SelectedItem.ToString().Trim());
                //serie.BorderColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                //serie.Color = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));

                serie.BorderColor = Color.Blue;
                serie.Color = Color.GreenYellow;
                serie.ChartType = SeriesChartType.Line;
                serie.BorderWidth = 3;
                serie.YValuesPerPoint = 32;
                serie.XValueType = ChartValueType.Time;
                serie.LabelAngle = 90;

                chart1.Series.Add(serie);
                chart1.Titles.Add("Moneda: " + cbomonedas_grafica.SelectedItem.ToString().Trim());
                chart1.Palette = ChartColorPalette.SeaGreen; //color a la grafica
                //chart1.DataManipulator.IsStartFromFirst = true;

                //chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                //chart1.ChartAreas["ChartArea1"].Area3DStyle.Inclination = 25;
                //chart1.ChartAreas["ChartArea1"].Area3DStyle.Rotation = 45;

                // datos historicos para llenar el grafico
                if (lMoneyHis_day.Exists(x => x.moneda == cbomonedas_grafica.SelectedItem.ToString().Trim()))
                {
                    List<CHistoricoMonedas_dia> sBook = lMoneyHis_day.FindAll(delegate(CHistoricoMonedas_dia bk)
                    {
                        return bk.moneda == cbomonedas_grafica.SelectedItem.ToString().Trim();
                    });

                    if (sBook.Count > 0)
                    {

                        for (int i = 0; i < sBook.Count; i++)
                        {
                            System.Windows.Forms.DataVisualization.Charting.DataPoint item = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                            //item.SetValueY(new object[] { Convert.ToDouble(sBook[i].precio) }); //Esto para cuando es de linea
                            item.ToolTip = sBook[i].precio + " - (" + sBook[i].tiempo.ToString("G").ToString() + ")";
                            item.XValue = sBook[i].tiempo.ToOADate();
                            item.YValues = new double[] { Convert.ToDouble(sBook[i].precio) };
                            chart1.Series[sBook[i].moneda].Points.Add(item);
                            chart1.Series[sBook[i].moneda].LabelAngle = 90;
                            item = null;
                            if (dMax < sBook[i].precio)
                                dMax = sBook[i].precio;
                            else if (dMin < sBook[i].precio)
                                dMin = sBook[i].precio;
                        }
                        //chart1.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(Convert.ToInt32(dMax * 1.20));
                        //chart1.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(Convert.ToInt32(dMin * .85));
                        chart1.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(dMax * 1.20);
                        chart1.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(dMin * .85);

                        chart1.ChartAreas[0].InnerPlotPosition.X = 10;
                        chart1.ChartAreas[0].InnerPlotPosition.Y = 5;
                        chart1.ChartAreas[0].InnerPlotPosition.Width = 90;
                        chart1.ChartAreas[0].InnerPlotPosition.Height = 80;
                        //chart1.Series[0].IsValueShownAsLabel = true; //muesta valores en la linea de la grafica.

                        //chart1.ChartAreas[0].Position = New ElementPosition(0, 0, 80, 100);
                        //chart1.Update();
                        chart1.ChartAreas.ResumeUpdates();

                    }//end if sbook.count

                }//end if lmoneyHis_day.exists.. 


                //confguraacion para la grafica de historial de monedas por dia max/min
                charthistorial.Series[0].Points.Clear();
                if (charthistorial.Series.Count >= 2)
                {
                    charthistorial.Series[1].Points.Clear();
                }

               // while (charthistorial.Series) { charthistorial.Series.RemoveAt(0); }
//                charthistorial.ChartAreas
                charthistorial.Series.Clear();
                charthistorial.Titles.Clear();
                //--------------- Serie 1 precio maximo
                //Series serieh = new Series(cbomonedas_grafica.SelectedItem.ToString().Trim()+"_max");
                Series serieh = new Series("precio max");
                serieh.BorderColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                serieh.Color = Color.Blue;
                serieh.ChartType = SeriesChartType.Line;
                serieh.BorderWidth = 3;
                serieh.YValuesPerPoint = 32;
                serieh.LabelAngle = 90;
                //serieh.Legend = "Precio Max";
                //serie.XValueType = "";
                ///---------------- serie 2 precio minimo
                Series serieh2 = new Series("Precio min");
                serieh2.BorderColor = Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255));
                serieh2.Color = Color.GreenYellow;
                serieh2.ChartType = SeriesChartType.Line;
                serieh2.BorderWidth = 3;
                serieh2.YValuesPerPoint = 32;
                serieh2.LabelAngle = 90;

                // serieh.Legend = "Precio Min";
                //---------------------------------------------
                charthistorial.Series.Add(serieh);
                charthistorial.Series.Add(serieh2);
                charthistorial.Titles.Add("Moneda: " + cbomonedas_grafica.SelectedItem.ToString().Trim());
                charthistorial.DataManipulator.IsStartFromFirst = true;

                /*charthistorial.ChartAreas[0].AxisX.Minimum=1; //configuracion de minimo y max de dias a mostrar
                charthistorial.ChartAreas[0].AxisX.Maximum = 31;*/
                charthistorial.ChartAreas[0].AxisX.Title = "Dias";
                charthistorial.ChartAreas[0].AxisX.Interval = 1;
                charthistorial.ChartAreas[0].AxisX.IsMarginVisible = false;
                charthistorial.Palette = ChartColorPalette.SeaGreen; //color a grafica

                dMax = dMin = 0.0;

                // datos historicos para llenar el grafico
                if (lstHistrialmoney.Exists(x => x.moneda == cbomonedas_grafica.SelectedItem.ToString().Trim()))
                {
                    List<CMonedaHistorial> sBookh = lstHistrialmoney.FindAll(delegate(CMonedaHistorial bk)
                    {
                        //return bk.book.Substring(0,3) == "btc";
                        return bk.moneda == cbomonedas_grafica.SelectedItem.ToString().Trim() && bk.tiempomax.Month == iMes;
                    });

                    if (sBookh.Count > 0)
                    {
                        //int[] iDias = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31 };
                        for (int i = 0; i < sBookh.Count; i++)
                        {
                            System.Windows.Forms.DataVisualization.Charting.DataPoint itemh = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                            //itemh.SetValueY(new object[] { Convert.ToDouble(sBookh[i].valormax) }); //Esto para cuando es de linea
                            itemh.ToolTip = sBookh[i].valormax + " - (" + sBookh[i].tiempomax.ToString("G").ToString() + ")";
                            itemh.XValue = double.Parse(sBookh[i].tiempomax.Day.ToString());
                            itemh.YValues = new double[] { Convert.ToDouble(sBookh[i].valormax) };

                            //--------------
                            System.Windows.Forms.DataVisualization.Charting.DataPoint itemh2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint();
                            //itemh2.SetValueY(new object[] { Convert.ToDouble(sBookh[i].valormin) }); //Esto para cuando es de linea
                            itemh2.ToolTip = sBookh[i].valormin + " - (" + sBookh[i].tiempomin.ToString("G").ToString() + ")";
                            itemh2.XValue = Convert.ToDouble(sBookh[i].tiempomin.Day.ToString());
                            itemh2.YValues = new double[] { Convert.ToDouble(sBookh[i].valormin) };

                            //charthistorial.Series[sBookh[i].moneda].Points.Add(itemh);
                            //charthistorial.Series[sBookh[i].moneda].Points.Add(itemh2);
                            charthistorial.Series[0].Points.Add(itemh);
                            charthistorial.Series[1].Points.Add(itemh2);

                            if (dMax < double.Parse(sBookh[i].valormax))
                                dMax = double.Parse(sBookh[i].valormax);
                            if (dMin > double.Parse(sBookh[i].valormin))
                                dMin = double.Parse(sBookh[i].valormin);
                            //charthistorial.Series[0].IsValueShownAsLabel = true; //muesta valores en la linea de la grafica.
                            //charthistorial.Series[1].IsValueShownAsLabel = true;

                            itemh = null;
                            itemh2 = null;

                        }

                        //charthistorial.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(Convert.ToInt32(dMax * 1.10));
                        //charthistorial.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(Convert.ToInt32(dMin * .95));
                        charthistorial.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(dMax * 1.10);
                        charthistorial.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(dMin * .95);
                        charthistorial.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.FixedCount;
                        //---chartarea0
                        charthistorial.ChartAreas[0].InnerPlotPosition.X = 12;
                        charthistorial.ChartAreas[0].InnerPlotPosition.Y = 5;
                        charthistorial.ChartAreas[0].InnerPlotPosition.Width = 90;
                        charthistorial.ChartAreas[0].InnerPlotPosition.Height = 80;

                        charthistorial.Update();
                        charthistorial.ChartAreas.ResumeUpdates();

                    }//end if sbook.count

                }//end if lmoneyHis_day.exists..
                //-----  finaliza para el historial de grafica max/min por dia al mes.

            }//end if cbomonedas_grafica
        }

        void validaTendenciaMoneda(string sMoneda, double dValor)
        {
            int itrue = 0;
            List<CHistoricoMonedas_dia> lBook = lTendecia.FindAll(delegate(CHistoricoMonedas_dia bk)
            {
                return bk.moneda == sMoneda;
            });

            if(sMoneda.Substring( sMoneda.Trim().Length-3,3) == "mxn")
            {

                // 18 veces equivalen a 1 min. y medio.  (90 seg / 5seg) = 18
                if (lBook.Count < 18) //cada 5 seg. se esta agregando un valor, al min y medio se valida si el precio va a la alza o baja.
                {
                    lTendecia.Add(new CHistoricoMonedas_dia { moneda = sMoneda, precio = dValor, tiempo = DateTime.Parse(DateTime.Now.ToString("G")) });
                }
                else
                {
                    //txtlogerror.AppendText("moneda: " + sMoneda + " cantidad:" + ldTendencia[sMoneda].ToString());
                    //txtlogerror.AppendText("\n");
                    double dPTotal = 0.0;
                    for (int i = 0; i < (sMoneyprecio.Length / 2) - 1; i++)
                    {
                        if (sMoneyprecio[i, 0].Trim() == sMoneda)
                        {
                            for (int d = 0; d < lBook.Count; d++)
                            {
                                dPTotal += lBook[d].precio;
                            }
                            dPTotal = (dPTotal / lBook.Count);
                            stTendencia st = new stTendencia();

                            if (dPTotal > double.Parse(sMoneyprecio[i, 1]))
                            {
                                //string sTexto = string.Format("moneda[{0}] , precioM[ {1}], Status: {2}", sMoneda, dPTotal.ToString(), "subir");
                                //txtlogerror.AppendText(sTexto);
                                //txtlogerror.AppendText("\n");
                                st.sMoneda = sMoneda;
                                st.bSubeBaja = true;
                                ltendencia3.Add(st);
                            }
                            else
                            {
                               // string sTexto = string.Format("moneda[{0}] , precioM[ {1}], Status: {2}", sMoneda, dPTotal.ToString(), "bajar");
                                //txtlogerror.AppendText(sTexto);
                                //txtlogerror.AppendText("\n");
                                st.sMoneda = sMoneda;
                                st.bSubeBaja = false;
                                ltendencia3.Add(st);
                            }
                            sMoneyprecio[i, 1] = dPTotal.ToString();
                            i = sMoneyprecio.Length;
                        }//if (sMoneyprecio[i, 0].Trim() == sMoneda)
                    }//end for sMoneyprecio.Length

                    List<stTendencia> lt3 = ltendencia3.FindAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });
                    if (lt3.Count >= 3) //validacion tendencia 2
                    {
                        for (int i = 0; i < lt3.Count; i++)
                        {
                            //valida si hay mas verdaderos que falsos, si es asi el precio de la moneda esta subiendo, contrario baja.
                            if (lt3[i].bSubeBaja)
                                itrue += 1;
                        }
                        string sTexto = string.Empty;
                        if (itrue >= 2)
                        {
                            actualizaTendenciaGrid(sMoneda, "↑"); //"subir");//sube
                            //sTexto = string.Format("moneda[{0}] , precioM[ {1}], Status: {2} , hora:{3}", sMoneda, dPTotal.ToString(), "sube",DateTime.Now.ToString("G") );
                            ltendencia3_3.Add(new stTendencia { bSubeBaja = true, sMoneda = sMoneda });//valida 3 veces la tendencia lt3
                        }
                        else
                        {
                            actualizaTendenciaGrid(sMoneda,  "↓" ); //"bajar");//baja
                            ltendencia3_3.Add(new stTendencia { bSubeBaja = false, sMoneda = sMoneda }); //valida 3 veces la tendencia lt3
                            //sTexto = string.Format("moneda[{0}] , precioM[ {1}], Status: {2}, hora:{3}", sMoneda, dPTotal.ToString(), "bajar",DateTime.Now.ToString("G") );
                        }
                        if (lt3[0].sMoneda == "btc_mxn" || lt3[0].sMoneda == "xrp_mxn" || lt3[0].sMoneda == "bat_mxn")
                        {
                            //txtlogerror.AppendText(sTexto);
                            //txtlogerror.AppendText("\n");
                            //valida 3 veces la tendencia de la monedas en sus 3 x min y medio. Esto para ver si esperar o vender/comprar
                            List<stTendencia> lt3_3 = ltendencia3_3.FindAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });
                            if (lt3_3.Count >= 3)
                            {
                                itrue = 0;
                                for (int t = 0; t < lt3.Count; t++)
                                {
                                    //valida si hay mas verdaderos que falsos, si es asi el precio de la moneda esta subiendo, contrario baja.
                                    if (lt3[t].bSubeBaja)
                                        itrue += 1;
                                }
                                if (itrue >= 2)
                                {
                                    //txtlogerror.AppendText("La tendencia de la moneda[" + lt3_3[0].sMoneda + "] es subir.." + DateTime.Now.ToString("G") );
                                    sTexto = "↑"; // "subir";
                                }
                                else
                                {
                                    //txtlogerror.AppendText("La tendecnia de la moneda[" + lt3_3[0].sMoneda + "] es bajar.." + DateTime.Now.ToString("G"));
                                    sTexto = "↓";// "bajar";
                                }
                                for (int id = 0; id < lvMonedas.Items.Count; id++)
                                {
                                    if (lvMonedas.Items[id].Text.Trim() == sMoneda)
                                    {
                                        string sMensaje = string.Format("{0}-[{1}]", sTexto,DateTime.Now.ToString("HH:mm:ss"));
                                        lvMonedas.Items[id].SubItems[7].Text = sMensaje;
                                        id = lvMonedas.Items.Count;// para que se salga.
                                    }
                                }
                                //-- se inhibe este metodo ya que en el hilo se manda llamar cada N segundos.
                                validaAlertas(sMoneda, dValor);// valida si es que tiene para vender o comprar si es viable esperar o lanzar la orden.
                                //txtlogerror.AppendText("\n");
                                ltendencia3_3.RemoveAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });//borra todos los elemento que encuentre.
                            }
                           
                        }
                        ltendencia3.RemoveAll(delegate(stTendencia bk) { return bk.sMoneda == sMoneda; });//borra todos los elemento que encuentre.
                    }
                    
                    lTendecia.RemoveAll(delegate(CHistoricoMonedas_dia bk) { return bk.moneda == sMoneda; });//borra todos los elementos que coincidan.
                }//if (lBook.Count < 3)
            }//if(sMoneda.Substring( sMoneda.Trim().Length-3,3) == "mxn")
        }

        void actualizaTendenciaGrid(string sMoneda, string sTendencia)
        {//Tendenciagrid
            for (int i = 0; i < lvMonedas.Items.Count ; i++)
            {
                if (lvMonedas.Items[i].Text.Trim() == sMoneda)
                {
                    string sTexto = string.Format("{0}-[{1}]", sTendencia, DateTime.Now.ToString("HH:mm:ss"));
                    lvMonedas.Items[i].SubItems[6].Text = sTexto; //Tendencia1
                }
            }
        }

        //poner este metodo en otro hilo para que se ejecute a destiempo al hilo principal.
        void validaOrdenes(string sMoneda)
        {
            bool bConsulta = false;
            for (int s = 0; s < dtgMismonedas.Rows.Count; s++)
            {
                if (dtgMismonedas.Rows[s].Cells[2].Value != null)// valor del dato en orden
                {
                    //columna donde esta el total de ordenes activas
                    double dOrden = double.Parse(dtgMismonedas.Rows[s].Cells[2].Value.ToString());
                    if (dOrden > 0.0)
                    {
                        if (!bConsulta)
                        {
                            bConsulta = true;
                        }
                        
                        double dLocked = 0.0;
                        for (int i = 0; i < lmiBalances.Count; i++)
                        {
                            if (Convert.ToDouble(lmiBalances[i].locked) > 0.0)
                            {
                                dLocked = double.Parse(lmiBalances[i].locked);
                            }
                        }
                        //con esto sabemos si ya se compro o vendio la moenda lanzada como propueta, si se finalizo el proceso se finaliza el hilo
                        if (dLocked <= 0.0 && bOrdenActiva)
                        {
                            actualizaListVewmiMonedas();
                            //s = lvmimonedas.Items.Count + 1;
                            s = dtgMismonedas.Rows.Count + 1;
                            bOrdenActiva = false;
                        }
                        else
                        {
                            //aqui poner validacion de la orden activa si baja de precio o sube lanzar una cancelacion, para no perder
                            //actualizagridordenes();
                        }
                    }//if orden 
                }//end != null
            }
            if (bConsulta)
            {
                lstHOrdenes.Clear();
                Bitso bitsoclient = new Bitso(sApi_key, sApi_pwd, bTipoPrueba);
                foreach (var ord in bitsoclient.GetOpenOrders("", "", "desc", 10))
                {
                    lstHOrdenes.Add(new COrdenes { moneda = ord.Book, side= ord.Side ,valor = double.Parse(ord.OriginalValue), precio = double.Parse(ord.Price), cantmonedas= double.Parse( ord.OriginalAmount), orden = ord.Type, status = ord.Status, id = ord.Oid, hora = DateTime.Parse(ord.CreatedAt) });
                }
                lmiBalances.Clear();
                foreach (var bal in bitsoclient.GetAccountBalance())
                {
                    if( double.Parse(bal.Total) >0.0 && bal.Currency.Trim()!="" )
                        lmiBalances.Add(new CBalance { currency = bal.Currency, available = bal.Available, locked = bal.Locked, total = bal.Total, pending_deposit = "0.0", pending_withdrawal = "0.0" });
                }
                
                actualizaListVewmiMonedas();
                actualizagridordenes();
            }
        }

        // metodo que se ejecuta en un hilo para validar solo las ordenes activas, selanza desde el metodo:
        //y una vez ejecutada orden compra/venta empieza a ejecutarce como un hilo separado hasta que se finalice la operacion.
        public void validaOrdenesActivas()
        {
            while (bOrdenActiva)
            //while ( txtplataforma.Text.Trim()=="" )
            {
                validaOrdenes("");
                Thread.Sleep(5000);
                
            }
            thrOrdenes.Abort();
        }

        /// <summary>
        /// metodo para validar el precio en que se lanzo la orden y el precio actual de la moneda, si es mayor o menor de la venta/compra, cancelar.
        /// </summary>
        void validaOrdenActivaPrecio(/*string sMoneda,double dPrecio,string sBuyCel*/)
        {
            string sMoneda = string.Empty;
            string sBuyCel = string.Empty;
            foreach (var ord in lstHOrdenes)
            {
                sMoneda = ord.moneda.Trim().ToUpper();
                sBuyCel = ord.side.Trim().ToUpper();
                for (Int32 index = 0; index < lvMonedas.Items.Count; index++)
                {
                    if (lvMonedas.Items[index].Text.Trim() == sMoneda)
                    {
                        if (sBuyCel == "BUY") //orden de compra
                        {
                            if (double.Parse(lvMonedas.Items[index].SubItems[0].Text) >= ord.precio)
                            {
                                //llamar la cancelacion de la orden de esta moneda[sMoneda]
                                cancelaOrden(ord.id);
                                index = lvMonedas.Items.Count + 1;
                            }
                        }
                        else
                        {
                            //Si el valor del precio de la orden que se lanzo es menor o igual al precio actual
                            if (double.Parse(lvMonedas.Items[index].SubItems[0].Text) <= ord.precio)
                            {
                                //llamar la cancelacion de la orden de esta moneda[sMoneda]
                                cancelaOrden(ord.id);
                                index = lvMonedas.Items.Count + 1;
                            }
                        }
                    }
                }
            }
        }

        private void chboxMes3d_CheckedChanged(object sender, EventArgs e)
        {
            if (chboxMes3d.Checked)
            {
                charthistorial.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                //charthistorial.ChartAreas["ChartArea1"].Area3DStyle.Inclination = 45;
                charthistorial.ChartAreas["ChartArea1"].Area3DStyle.Rotation = 25;
            }
            else
            {
                charthistorial.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;
            }
            charthistorial.Update();
        }

        private void cbomesgh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (charthistorial.ChartAreas.Count >= 0)
            {
                //this.cbomonedas_grafica_SelectedIndexChanged();
                //this.cbomonedas_grafica_SelectedIndexChanged(null, null);

                if (cboaño.SelectedIndex >= 0)
                {
                    actualizaGraficasMes_Dia();
                }
                else
                {
                    MessageBox.Show("Seleccione año a consultar!!");
                    cboaño.Focus();
                }
            }
        }

        private void btnHistorialOrdenes_Click(object sender, EventArgs e)
        {
            FrmHistorialOrdenes frmOrdenes = new FrmHistorialOrdenes();
            frmOrdenes.lstHOrdenes.Clear();
            frmOrdenes.lstHOrdenes = lstHOrdenes;
            
            //foreach(var ord in lstHOrdenes)
            //{
            //    frmOrdenes.lstHOrdenes.Add( new COrdenes { moneda = ord.moneda, valor = double.Parse( ord.OriginalValue), precio = double.Parse(ord.Price), orden = ord.Type, status = ord.Status, id = ord.Oid, hora = DateTime.Parse( ord.CreatedAt) });
            //}
            frmOrdenes.ShowDialog();
        }

        private void guardarHistorialMonedasDia()
        {
            string[] sMeses= {"","ene","feb","mzo","abr","may","jun","jul","ago","sep","oct","nov","dic"};
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();
            string sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneydia.json", sAnio, sMes);
            string sDir = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio, sMes);
            
            //StreamWriter wr = new StreamWriter(sRuta);

            if (!Directory.Exists(sDir))
            {
                if (!Directory.Exists(@"C:\sys") )
                    Directory.CreateDirectory(@"C:\sys");
                if (!Directory.Exists(@"C:\sys\tradingboots"))
                    Directory.CreateDirectory(@"C:\sys\tradingboots");
                if (!Directory.Exists(@"C:\sys\tradingboots\historial"))
                    Directory.CreateDirectory(@"C:\sys\tradingboots\historial");
                if (!Directory.Exists(string.Format( @"C:\sys\tradingboots\historial\{0}",sAnio)) )
                    Directory.CreateDirectory(string.Format(@"C:\sys\tradingboots\historial\{0}",sAnio) );
                Directory.CreateDirectory(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio,sMes ) );
            }

            //Crear el archivo con la información
            
            if (lMoneyHis_day.Count > 0)
            {
                List<string> listMonedas = new List<string>();

                foreach (CHistoricoMonedas_dia item in lMoneyHis_day.OrderBy(x=>x.moneda).ThenBy(x1=>x1.tiempo) )
                {
                    try
                    {
                        //Revizar si no esta repetido
                        //if ( lstmon.Exists(x=> x.ToString()==s.ToString())  )
                        if(!listMonedas.Exists(x => x==item.moneda))
                        {
                            listMonedas.Add(item.moneda);
                        }
                        var jsontxt = JsonConvert.SerializeObject(item) +Environment.NewLine;
                        if (!File.Exists(sRuta))
                        {
                            File.WriteAllText(sRuta, jsontxt);
                        }
                        else
                        {
                            File.AppendAllText(sRuta, jsontxt);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurio un Error al momento de Guardar historial de monedas\n" + ex.Message);
                    }
                }
            }
        }

        private bool LeerHistorialMonedasDia()
        {
            bool bRegresa = false;            
            lMoneyHis_day.Clear();
            string sLine = string.Empty;
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();
            string sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneydia.json", sAnio, sMes);
            
            //string sFile = @"C:\sys\tradingboots\historial\histomoneydia.json";
            string sFile = sRuta;

            CHistoricoMonedas_dia mhd = new CHistoricoMonedas_dia();

            //if (Directory.Exists(@"C:\sys\tradingboots\historial"))
            if (Directory.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio,sMes) ))
            {
                //if (File.Exists(@"C:\sys\tradingboots\historial\histomoneydia.json"))
                if (File.Exists(sRuta))
                {
                    try
                    {
                        StreamReader file = new StreamReader(sFile);

                            //primera linea 
                        sLine = file.ReadLine();
                        if (sLine.Trim() != "" && sLine.Trim() != null)
                        {
                            if (sLine != "")
                            {
                                mhd = JsonConvert.DeserializeObject<CHistoricoMonedas_dia>(sLine);
                                lMoneyHis_day.Add(mhd);
                            }

                            while (sLine != null)
                            {
                                sLine = file.ReadLine();
                                if (sLine != null)
                                {
                                    mhd = JsonConvert.DeserializeObject<CHistoricoMonedas_dia>(sLine);
                                    lMoneyHis_day.Add(mhd);
                                }
                                bRegresa = true;
                            }
                            file.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ocurrio un Error al momento de leer datos historico de monedas\n" + ex.Message, "Error de lectura de archivo");
                    }
                }
                else{
                   // MessageBox.Show("El archivo: " + sFile + " ,no existe, verifique!!");
                }//fin File.exist...
            }/// fin Directory.Exists..

            return bRegresa;

        }

        /// <summary>
        /// guardarHistorialOrdenes ..[ #ordeng]
        /// </summary>
        private void guardarHistorialOrdenes()
        {
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();
            string sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histoOrdenesDia.json",sAnio,sMes);

            //StreamWriter wr = new StreamWriter(sRuta);

            if (!Directory.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}",sAnio,sMes) ))
                File.Create(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}",sAnio,sMes) );

            //Crear el archivo con la información
            //FrmHistorialOrdenes.ChistorialOrdenes chordenes = new FrmHistorialOrdenes.ChistorialOrdenes();
            if (lstHOrdenes.Count > 0)
            {
                //if ((DateTime.Today.Day > cm.tiempoMin.Day) || (DateTime.Today.Month != cm.tiempoMin.Month))
                //{
                //    //esto es para validar cuando este en el ultimo dia del mes que lo cargado en la clase no lo guarde en el otro mes.
                //    //ejemplo si el mes trae 30 dias , el dia 30 lo guarde en el mes que corresponde no en el siguiente.
                //    if ((DateTime.Today.Month != cm.tiempoMin.Month) && Math.Abs((cm.tiempoMin.Day - DateTime.Today.Day)) > 1)
                //    {
                //        sMes = sMeses.ElementAt(DateTime.Now.Month - 1);
                //        sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histoOrdenesDia.json", sAnio, sMes);
                //    }
                //}
                

                //foreach (FrmHistorialOrdenes.ChistorialOrdenes chordenes in lstHOrdenes)
                foreach (COrdenes chordenes in lstHOrdenes)
                {
                    //chordenes.
                    try
                    {
                        var jsontxt = JsonConvert.SerializeObject(chordenes) + Environment.NewLine;
                        if (!File.Exists(sRuta))
                        {
                            File.WriteAllText(sRuta, jsontxt);
                        }
                        else
                        {
                            File.AppendAllText(sRuta, jsontxt);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurio un Error al momento de Guardar historial de Ordenes\n" + ex.Message);
                    }
                }
            }
        }
        private bool leerHistorialOrdenesDia()
        {
            bool bRegresa = false;
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();
                        
            lstHOrdenes.Clear();
            string sLine = string.Empty;
            string sFile = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histoOrdenesDia.json", sAnio, sMes);
            //FrmHistorialOrdenes.ChistorialOrdenes mho = null;
            COrdenes mho = new COrdenes();

            if (Directory.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio, sMes) ))
            {
                if (File.Exists(sFile))
                {
                    try
                    {
                        StreamReader file = new StreamReader(sFile);

                        //primera linea 
                        sLine = file.ReadLine();
                        if (sLine.Trim() != "" && sLine.Trim() != null)
                        {
                            if (sLine != "")
                            {
                                //mho = JsonConvert.DeserializeObject<FrmHistorialOrdenes.ChistorialOrdenes>(sLine);
                                mho = JsonConvert.DeserializeObject<COrdenes>(sLine);
                                lstHOrdenes.Add(mho);
                            }

                            while (sLine != null)
                            {
                                sLine = file.ReadLine();
                                if (sLine != null)
                                {
                                    //mho = JsonConvert.DeserializeObject<FrmHistorialOrdenes.ChistorialOrdenes>(sLine);
                                    mho = JsonConvert.DeserializeObject<COrdenes>(sLine);
                                    lstHOrdenes.Add(mho);
                                }
                                bRegresa = true;
                            }
                            file.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ocurrio un Error al momento de leer datos historico de Ordenes\n" + ex.Message, "Error de lectura de archivo");
                    }
                }//fin File.exist...
            }/// fin Directory.Exists..

            return bRegresa;
        }

        /// <summary>
        /// borrarHistorial_ordenes_monedasDia();
        /// Metodo para borrar los historiales de movimientos de monedas del dia, el de ordenes se deja por mes
        /// </summary>
        private void borrarHistorial_ordenes_monedasDia()
        {
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();

            

            if (File.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histoOrdenesDia.json",sAnio,sMes)))
            {
                File.Delete(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histoOrdenesDia.json", sAnio, sMes) );
            }
            if (File.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneydia.json",sAnio,sMes)) )
            {
                File.Delete(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneydia.json", sAnio, sMes));
            }
        }

        private void FrmPrincipalbtc_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            borrarHistorial_ordenes_monedasDia();
            guardarHistorialMonedasDia();
            guardarHistorialOrdenes();

            if (txtplataforma.Text.Trim().ToUpper() == "BITSO")
            {
                if (sApi_pwd.Trim() != "")
                {
                    CAplicationLogin bitso = new CAplicationLogin();
                    bitso.guardarDatosApi_Bitso(sApi_pwd,sApi_pwd,"");
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void dtgMismonedas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string vendorName = "",sMoneda = string.Empty, sLocked= string.Empty;
            if (e.ColumnIndex == 7)
            {
                vendorName = dtgMismonedas.Rows[e.RowIndex].Cells[8].Value.ToString();
                sMoneda = dtgMismonedas.Rows[e.RowIndex].Cells[0].Value.ToString();
                sLocked = dtgMismonedas.Rows[e.RowIndex].Cells[2].Value.ToString();

                if ( MessageBox.Show("Deceas Cancelar la orden con los sig. datos..? " + Environment.NewLine + sMoneda + Environment.NewLine + sLocked, "Verifique..",MessageBoxButtons.YesNo) == DialogResult.Yes )
                {
                    //CTrading_Bitso bt = new CTrading_Bitso();
                    //bt.consultaOrdenesActivas("xrp_mxn");
                    cancelaOrden(vendorName);
                }
            }
        }

        private void cancelaOrden(string sIdMoneda)
        {
            string sPlataforma= txtplataforma.Text.Trim().ToUpper();
            switch (sPlataforma)
            {

                case "BITSO":
                    {
                        Bitso bitso = new Bitso(sApi_key, sApi_pwd, bTipoPrueba);
                        string[] val= bitso.CancelOpenOrder(sIdMoneda);
                        MessageBox.Show(val.ToString());
                    }
                    break;
            }
        }

        private void charthistorial_Click(object sender, EventArgs e)
        {

        }

        private void btnlog_Click(object sender, EventArgs e)
        {
            dtgMismonedas.Update();

            FrmLog dlglog = new FrmLog();
            dlglog.txtlog.Text = txtlogerror.Text;
            dlglog.ShowDialog();
            txtlogerror.Text = dlglog.txtlog.Text;
            if (dlglog.txtlog.Text.Trim() == "")
                btnlog.ForeColor = Color.RoyalBlue;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            actualizaTotalMXmonedas(); // aqui esto
        }

        private void cboaño_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
