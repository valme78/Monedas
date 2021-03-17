using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json; //Libreria importada para manejo de JSON.  carpeta de proyecyto\LIB
using System.IO;
using System.Windows.Forms;
using System.Threading;
using btcTrading;
//namespace btcTrading
//{
    //class CDatosMoneda
    //{
    //}
/*
 * paguina para generar clase de consulta de json
 * http://json2csharp.com/#
 * */

public delegate void midelegado1(string sMoneda, ListView lvObj,ref string sTserv);


    public class Payload
    {
        public string book { get; set; }
        public string minimum_price { get; set; }
        public string maximum_price { get; set; }
        public string minimum_amount { get; set; }
        public string maximum_amount { get; set; }
        public string minimum_value { get; set; }
        public string maximum_value { get; set; }
    }

    public class RootMoney
    {
        public bool success { get; set; }
        public List<Payload> payload { get; set; }
    }

    //public class CDataPrice
    //{
    //    public string high { get; set; }
    //    public string last { get; set; }
    //    public string timestamp { get; set; }
    //    public string volume { get; set; }
    //    public string vwap { get; set; }
    //    public string low { get; set; }
    //    public string ask { get; set; }
    //    public string bid { get; set; }
    //}

    public class CDataPrice
    {
        public string high { get; set; }
        public string last { get; set; }
        public DateTime created_at { get; set; }
        public string book { get; set; }
        public string volume { get; set; }
        public string vwap { get; set; }
        public string low { get; set; }
        public string ask { get; set; }
        public string bid { get; set; }
    }    

    public class MoneyData
    {
        public bool success { get; set; }
        public List<CDataPrice> payload { get; set; }
    }
    public class MoneyData2
    {
        public bool success { get; set; }
        public CDataPrice payload { get; set; }
    }
    
  // -----------------   clases para la consula del balance -----
    public class CBalancesItem
    {         
        public string currency { get; set; }        
        public string available { get; set; }
        public string locked { get; set; }
        public string total { get; set; }
        public string pending_deposit { get; set; }
        public string pending_withdrawal { get; set; }
    }

    public class CUserBalance
    {
        public bool success { get; set; }
        public List<CBalancesItem> balances { get; set; }
    }

 //---- FIN clase consulta balance

    public struct stTendencia
    {
        public string sMoneda;
        public bool bSubeBaja;
    }

    public class CDatosAlertas
    {
        public string moneda { get; set; }
        public double maximo { get; set; }
        public double minimo { get; set; }
        public string file { get; set; }
        public bool chkmin { get; set; }
        public bool chkmax { get; set; }
        public bool chkri { get; set; }
        public double valor { get; set; }
    }

    public class CMoneda_Movto
    {
        public string moneda { get; set; }
        public double valorMax { get; set; }
        public double valorMin { get; set; }
        public DateTime tiempoMax { get; set; }
        public DateTime tiempoMin { get; set; }
        public double valorOpen { get; set; }
        public double valorClose { get; set; }
    }

    public class CHistoricoMonedas_dia
    {
        public string moneda { get; set; }
        public double precio { get; set; }        
        public DateTime tiempo { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
    }
    
    public class CDatosMonedas
    {
        private string moneda;
        private ListView lvg;        

        public string sMensajeError="";
        public delegate void MensajeErrorDelegate(string p, CDataPrice moneyp);
        public event MensajeErrorDelegate Mensaje;

        public void MostrarMensaje(string sMensaje, CDataPrice moneyp)
        {
            if (sMensaje != "" || Convert.ToDouble( moneyp.ask )> 0.0)
                OnMensaje(sMensaje,moneyp);
        }
        protected void OnMensaje(string p, CDataPrice moneyp)
        {
            if (Mensaje != null)
                Mensaje(p,moneyp);
        }

        protected string getTime()
        {
            return string.Format( "{0} :{1}", DateTime.Now.Day.ToString(), DateTime.Now.ToShortTimeString() );
        }


        public CDatosMonedas(string sMoneda, ListView lv)
        {
            this.moneda = sMoneda;
            this.lvg = lv;
        }
        public CDatosMonedas() { }

        public CDataPrice consultaCoins(string sMoneda, ListView lvObj)        
        {
            string sTexto = string.Empty;
            CDataPrice dPrice = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://bitso.com/api/v2/ticker?book=" + sMoneda);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    dPrice = JsonConvert.DeserializeObject<CDataPrice>(json);
                    
                    lvObj.Items[sMoneda].SubItems[1].Text = dPrice.ask;
                    lvObj.Items[sMoneda].SubItems[2].Text = dPrice.high;
                    lvObj.Items[sMoneda].SubItems[3].Text = dPrice.low;
                    lvObj.Items[sMoneda].SubItems[4].Text = dPrice.last;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ocurrio un error al Consultar monedas[1]: " + ex.Message); }

            return dPrice;
        }

        public void consultaCoins2(string sMoneda, ListView lvObj,ref string sTServ)        
        {
            string sTexto = string.Empty;
            CDataPrice dPrice = null;
            MoneyData2 md = null;
            
            try
            {
                string sURL = @"https://bitso.com/api/v3/ticker?book=" + sMoneda;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
                request.Timeout = 4000;
               
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    //dPrice = JsonConvert.DeserializeObject<CDataPrice>(json);
                    md = JsonConvert.DeserializeObject<MoneyData2>(json);
                    /*"volume": "22.31349615", "high": "5750.00","last": "5633.98","low": "5450.00","vwap": "5393.45","ask": "5632.24",
                    "bid": "5520.01","timestamp": "1447348096*/
                    dPrice = md.payload;
                    if (sMoneda == "btc_mxn" || sMoneda == "xrp_mxn")
                    {
                        lvObj.Items[sMoneda].SubItems[1].Text = string.Format("{0,14:N2}", Convert.ToDouble(dPrice.ask));
                        lvObj.Items[sMoneda].SubItems[2].Text = string.Format("{0,14:N2}", Convert.ToDouble(dPrice.high));
                        lvObj.Items[sMoneda].SubItems[3].Text = string.Format("{0,14:N2}", Convert.ToDouble(dPrice.low));
                        lvObj.Items[sMoneda].SubItems[4].Text = string.Format("{0,14:N2}", Convert.ToDouble(dPrice.last));
                    }
                    else
                    {
                        lvObj.Items[sMoneda].SubItems[1].Text = dPrice.ask;
                        lvObj.Items[sMoneda].SubItems[2].Text = dPrice.high;
                        lvObj.Items[sMoneda].SubItems[3].Text = dPrice.low;
                        lvObj.Items[sMoneda].SubItems[4].Text = dPrice.last;
                    }
                    dPrice.book = sMoneda;
                    MostrarMensaje("", dPrice);
                    request = null;
                    response.Close();
                    sTServ= dPrice.created_at.ToShortTimeString();
                }
            }
            catch (WebException ex)
            {
                //string sMsge = string.Empty;
                
                //using (WebResponse responsex = ex.Response)
                //{
                //    HttpWebResponse httpResponsex2 = (HttpWebResponse)responsex;
                //    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);  
                    
                //    using (Stream dataex = responsex.GetResponseStream())
                //    using (var readerex = new StreamReader(dataex))
                //    {
                //        sMsge = readerex.ReadToEnd() + "Error:" + httpResponsex2.StatusCode.ToString();
                //        //MessageBox.Show(text + " : " + httpResponse.StatusCode);
                //        dataex.Close();
                //        dataex.Dispose();
                //    }
                    
                //    httpResponsex2 = null;
                //}
                sMensajeError = string.Format("stauts: {0}, Error:{1}, moneda[{2}][#2] ,d |h[{3}]\n", ex.Status.ToString(), ex.Message, sMoneda,getTime());
                MostrarMensaje(sMensajeError,dPrice); 
               // MostrarMensaje(sMsge, dPrice); 
               
            }
            catch (Exception ex) { 
                //MessageBox.Show("Ocurrio un error: " + ex.Message); 
                sMensajeError = string.Format("Error:{0} ,Moneda[{1}][#2], d |h[{2}]\n", ex.Message,sMoneda, getTime());
                MostrarMensaje(sMensajeError,dPrice);                
                //MessageBox.Show(sMensajeError);
            }
        }

        public void ejecutaHilos(ListView lv, ref string sTserv)
        {
            for (Int16 index = 0; index < lv.Items.Count; index++)
            {
                {
                    //CDatosMonedas dhilos = new CDatosMonedas(lv.Items[index].Text, lv);
                    //midelegado1 dm1 = new midelegado1(dhilos.consultaCoins2);  //funcionando 1
                    //dm1.Invoke(lv.Items[index].Text, lv, ref sTserv); //este esta funcionando 2

                    //midelegadoMonedas dm = new midelegadoMonedas( dhilos.consultaCoins);


                    //Thread hilo = new Thread( new ThreadStart ( dm )).Start();
                    //hilo.Start(lvMonedas.Items[index].Text);
                    //lMoneyData.Add( dm.Invoke(lvMonedas.Items[index].Text, lvMonedas));
                    

                    //new Thread(new ThreadStart(dhilos.consultaCoins2 )).Start();
                    
                    /*ThreadStart starter = delegate { dhilos.consultaCoins2(lv.Items[index].Text, lv); };
                    starter.Invoke();*/
                }
            }
        }
    }

//}
