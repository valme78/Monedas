﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json; //Libreria importada para manejo de JSON.  carpeta de proyecyto\LIB
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Threading;
using bitso.Entities;
using EntidadesGenerales;


namespace btcTrading
{
    public class Bitso
    {
        private readonly string BITSO_KEY;
        private readonly string BITSO_SECRET;
        private readonly string BITSO_API_URL;
        private readonly string BITSO_VERSION_PREFIX;
        public List<Balance> lBalance = new List<Balance>();
        public List<CMoneda_Historial> lHistrialmoney = new List<CMoneda_Historial>();

        // Delegados para el manejo de mensajes de error y mostrarlos en el dialogo de Logs.
        public string sMensajeError = "";
        public delegate void MensajeErrorDelegate(string p, CMoneyPrice moneyp);
        public event MensajeErrorDelegate Mensaje;
        public delegate void delegadomonedas(string sMoneda, ListView lvObj, ref string sTserv);


        //public void MostrarMensaje(string sMensaje, CDataPrice moneyp)
        public void MostrarMensaje(string sMensaje, CMoneyPrice moneyp)            
        {
            if (sMensaje != "" || Convert.ToDouble(moneyp.ask) > 0.0)
                OnMensaje(sMensaje, moneyp);
        }
        //protected void OnMensaje(string p, CDataPrice moneyp)
        protected void OnMensaje(string p, CMoneyPrice moneyp)
        {
            if (Mensaje != null)
                Mensaje(p, moneyp);
        }
        // fin mensajes delegados

        [JsonProperty("details")]
        public JObject Details { get; set; }
    

        public Bitso(string key, string secret, bool production )
        {
            BITSO_KEY = key;
            BITSO_SECRET = secret;
            BITSO_API_URL = production ? "https://api.bitso.com" : "https://api-dev.bitso.com";
            //BITSO_VERSION_PREFIX = "/api/v3/";
            BITSO_VERSION_PREFIX = "/v3/";
        }

        public Bitso(bool production) 
        { 
            BITSO_API_URL = production ? "https://api.bitso.com" : "https://api-dev.bitso.com";
//            BITSO_VERSION_PREFIX = "/api/v3/";
            BITSO_VERSION_PREFIX = "/v3/";
        }


        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        /// <summary>
        /// HMACSHA256, metodo para regresar datos de autenticacion para el api
        /// </summary>
        /// <param name="SIGNATURE">nonce + method + requestPath + body</param>
        /// <param name="BITSO_SECRET">clave de bitso que se da de alta en la paguina web de bitso</param>
        /// <returns>sResult, regresa los datos encriptados en hexadecimal</returns>
        private string HMACSHA256(string SIGNATURE, string BITSO_SECRET)
        {
            string sResult = string.Empty;

            ASCIIEncoding encoder = new ASCIIEncoding();
            HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(BITSO_SECRET));

            byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(SIGNATURE));
            sResult = ToHexString(hm256Byte);

            return sResult;
        }

        public string SendRequest(string url, string method, bool signRequest, string body)
        {
            string sMensajeError = string.Empty;
            var requestPath = BITSO_VERSION_PREFIX + url;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BITSO_API_URL + requestPath);

            if (signRequest)
            {
                //Authorization: Bitso <key>:<nonce>:<signature>
                //key = The API Key you generated
                //nonce = An Integer that must be unique and increasing for each API call
                //signature = The signature is generated by creating a SHA256 HMAC using the Bitso API Secret on the concatenation of nonce + HTTP method + requestPath + JSON payload (no ’+’ signs in the concatenated string) and hex encode the output. 

                var nonce = Convert.ToInt64(new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds).ToString();
                var signature = HMACSHA256(nonce + method + requestPath + body, BITSO_SECRET);

                var AUTH_HEADER = "Bitso " + BITSO_KEY + ":" + nonce + ":" + signature;
                httpWebRequest.Headers["Authorization"] = AUTH_HEADER;
            }

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;

            var response = string.Empty;

            if (!string.IsNullOrEmpty(body))
            {
                using (var req = httpWebRequest.GetRequestStream())
                {
                    var bodyBytes = Encoding.UTF8.GetBytes(body);
                    req.Write(bodyBytes, 0, bodyBytes.Length);
                }
            }

            try
            {
                using (var res = httpWebRequest.GetResponse())
                {
                    using (var str = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(str))
                        {
                            response = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                using (HttpWebResponse res = (HttpWebResponse)ex.Response)
                {
                    if (res == null)
                    {
                        //MessageBox.Show("No hay respuesta desde Bitso.", "0");
                        sMensajeError= "No hay respuesta desde Bitso..0";
                        MostrarMensaje(sMensajeError,new CMoneyPrice() );
                    }

                        using (var str = res.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(str))
                            {
                                response = reader.ReadToEnd();

                                if (res.StatusCode == HttpStatusCode.NotFound && response.StartsWith("<"))
                                {
                                    //MessageBox.Show("No se encontró el recurso solicitado.", "-1");
                                    sMensajeError= "No se encontró el recurso solicitado..-1";
                                    MostrarMensaje(sMensajeError, new CMoneyPrice());
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "0");
                sMensajeError = string.Format("stauts: {0}, d |h[{1}]\n",  ex.Message, getTime());
                MostrarMensaje(sMensajeError, new CMoneyPrice()); 
            }

            if (string.IsNullOrEmpty(response) || response.StartsWith("<"))
            {
                //MessageBox.Show("Una respuesta mal formada fue devuelta desde Bitso..", "-1");
                sMensajeError= "Una respuesta mal formada fue devuelta desde Bitso..-1";
                MostrarMensaje(sMensajeError, new CMoneyPrice());
            }

             var responseObj = JsonConvert.DeserializeObject<JObject>(response);

            if (responseObj == null)
            {
                sMensajeError = "[0] No se devolvió ninguna respuesta de Bitso.";
                //MessageBox.Show("No se devolvió ninguna respuesta de Bitso.", "0");
                MostrarMensaje(sMensajeError, new CMoneyPrice());
            }

            if (responseObj["success"].Value<bool>())
            {
                if (method == "GET" && url == "balance") //This was hardcoded to mantain consistency in the response
                    return responseObj["payload"]["balances"].ToString();

                return responseObj["payload"].ToString();
            }

            sMensajeError = string.Format(responseObj["error"]["message"].ToString(), responseObj["error"]["code"].ToString());
            MostrarMensaje(sMensajeError, new CMoneyPrice());
            //MessageBox.Show(responseObj["error"]["message"].ToString(), responseObj["error"]["code"].ToString());

            return "";
        }


        //https://bitso.com/api_info#available-books
        public BookInfo[] GetAvailableBooks()
        {
            var rawResponse = SendRequest("available_books", "GET", false,null);
            return JsonConvert.DeserializeObject<BookInfo[]>(rawResponse);
        }

        //https://bitso.com/api_info#tickers
        public Ticker[] GetTickers()
        {
            //var rawResponse = SendRequest("ticker", "GET", false,null);
            var rawResponse = SendRequest("ticker", "POST", false, null);
            return JsonConvert.DeserializeObject<Ticker[]>(rawResponse);
        }

        //https://bitso.com/api_info#ticker
        public Ticker GetTicker(string book)
        {
            var rawResponse = SendRequest("ticker?book=" + book, "GET", false,null);
            return JsonConvert.DeserializeObject<Ticker>(rawResponse);
        }

        //https://bitso.com/developers#user-trades
        public UserTrade[] GetUserTrades(string book , string marker , string sort , int limit)
        {
            var rawResponse = SendRequest("user_trades" + BuildQueryString("book", book, "marker", marker, "sort", sort, "limit", limit.ToString()), "GET",true, null);
            return JsonConvert.DeserializeObject<UserTrade[]>(rawResponse);
        }

        public string[] CancelOpenOrder(string oid)
        {
            var rawResponse = SendRequest("orders/"+ oid, "DELETE",true,null);
            return JsonConvert.DeserializeObject<string[]>(rawResponse);
        }

        //https://bitso.com/api_info#account-balance
        public Balance[] GetAccountBalance()
        {
            var rawResponse = SendRequest("balance", "GET",true,null);
            return JsonConvert.DeserializeObject<Balance[]>(rawResponse);
        }

         //https://bitso.com/api_info#open-orders
        public OpenOrder[] GetOpenOrders(string book, string marker , string sort , int limit)
        {
            var rawResponse = SendRequest("open_orders" + BuildQueryString("book", book, "marker", marker, "sort", sort, "limit", limit.ToString()), "GET",true,null);
            return JsonConvert.DeserializeObject<OpenOrder[]>(rawResponse);
        }

        //https://bitso.com/api_info#cancel_order
        public string[] CancelAllOpenOrders()
        {
            var rawResponse = SendRequest("orders/all", "DELETE", true,null);
            return JsonConvert.DeserializeObject<string[]>(rawResponse);
        }

        //https://bitso.com/developers#lookup-orders
        public OpenOrder LookupOrder(string oid)
        {
            var rawResponse = SendRequest("orders/" + oid, "GET",true,null);
            var orders = JsonConvert.DeserializeObject<OpenOrder[]>(rawResponse);
            if (orders != null && orders.Length > 0) return orders[0];
            return null;
        }

        //https://bitso.com/api_info#place-an-order
        //public OpenOrder PlaceOrder(string book, string side, string type, decimal price, decimal minorAmount , decimal majorAmount )
        public OpenOrder PlaceOrder(OrderPlaced orden )
        {             
            string sjson = JsonConvert.SerializeObject(orden);
            var rawResponse = SendRequest("orders", "POST", true, sjson);
               /* "{{\"book\":\"{"+ book+ "}\"," +
                "\"side\":\"{"+side+"}\"," +
                "\"type\":\"{"+type+"}\"," +
                "\"minor\":\"{"+minorAmount.Value+"}\"," : "") +
                "\"major\":\"{"+majorAmount.Value+"}\"," : "") +
                "\"price\":\"{"+price+"}\"}}");*/

            return JsonConvert.DeserializeObject<OpenOrder>(rawResponse);
        }



        public static string BuildQueryString(Dictionary<string, string> keyValues)
        {
            var queryBuilder = new StringBuilder();
            if (keyValues == null) return string.Empty;
            var index = 0;
            foreach (var key in keyValues.Keys)
            {
                if (index > 0)
                    queryBuilder.Append("&");
                queryBuilder.AppendFormat("{0}={1}", key, keyValues[key]);
            }

            var query = queryBuilder.ToString();

            return !string.IsNullOrEmpty(query) ? ("?" + query) : string.Empty;
        }

        public static string BuildQueryString(params string[] keyValues)
        {
            var queryBuilder = new StringBuilder();
            var isKey = true;
            if (keyValues == null) return string.Empty;
            var index = 0;
            foreach (var keyOrValue in keyValues)
            {
                if (index > 0 && isKey)
                    queryBuilder.Append("&");

                queryBuilder.AppendFormat(isKey ? "{0}=" : "{0}", keyOrValue);

                isKey = !isKey;
                index++;
            }

            var query = queryBuilder.ToString();
            return !string.IsNullOrEmpty(query) ? ("?" + query) : string.Empty;
        }
        protected string getTime()
        {
            return string.Format("{0} :{1}", DateTime.Now.Day.ToString(), DateTime.Now.ToShortTimeString());
        }

        public void consultamonedas(string sMoneda, ListView lvObj)
        {
            CMoneyPrice mp = new CMoneyPrice();
            double dporcentaje=0.0, dvar=0.0;
            string sTexto = string.Empty;
            try
            {                 
                var ticker =GetTicker(sMoneda);                
                if (sMoneda == "btc_mxn" || sMoneda == "xrp_mxn" || sMoneda =="bat_mxn")
                {
                    lvObj.Items[sMoneda].SubItems[1].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.Ask));
                    lvObj.Items[sMoneda].SubItems[2].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.PriceHigh));
                    lvObj.Items[sMoneda].SubItems[3].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.PriceLow));
                    lvObj.Items[sMoneda].SubItems[4].Text = string.Format("{0,14:N2}", Convert.ToDouble(ticker.LastTradedPrice));
                    dvar =  Convert.ToDouble(ticker.LastTradedPrice) - Convert.ToDouble(ticker.PriceHigh);
                    dporcentaje = ((dvar / Convert.ToDouble(ticker.PriceLow)) * 100.00);
                    if (dporcentaje < 0.0 )
                        sTexto = string.Format("{0,14:N2} ({1,2:N2}%){2}", dvar, dporcentaje, "↓" );
                    else
                        sTexto = string.Format("{0,14:N2} ({1,2:N2}%){2}", dvar, dporcentaje,"↑");
                    lvObj.Items[sMoneda].SubItems[5].Text = sTexto;
                                       
                }
                else
                {
                    lvObj.Items[sMoneda].SubItems[1].Text = ticker.Ask;
                    lvObj.Items[sMoneda].SubItems[2].Text = ticker.PriceHigh;
                    lvObj.Items[sMoneda].SubItems[3].Text = ticker.PriceLow;
                    lvObj.Items[sMoneda].SubItems[4].Text = ticker.LastTradedPrice;
                }
                //dPrice.book = sMoneda;
                mp.ask = ticker.Ask;
                mp.bid = ticker.Bid;
                mp.book = ticker.Book;
                mp.created_at = DateTime.Parse( ticker.CreatedAt);
                mp.high = ticker.PriceHigh;
                mp.last = ticker.LastTradedPrice;
                mp.low = ticker.PriceLow;
                mp.volume = ticker.Volume;
                mp.vwap = ticker.VolumeWeightedAvgPrice;

                //sTServ = DateTime.Parse(ticker.CreatedAt).ToShortTimeString(); // dPrice.created_at.ToShortTimeString();
                MostrarMensaje("", mp);
                
            }
            catch (WebException ex)
            {
                var sMensajeError = string.Format("stauts: {0}, Error:{1}, moneda[{2}][#2] ,d |h[{3}]\n", ex.Status.ToString(), ex.Message, sMoneda,"000" /*getTime()*/);
                MostrarMensaje(sMensajeError, mp);
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Ocurrio un error: [" + sMoneda +" ] :" + ex.Message); 
                sMensajeError = string.Format("Error:{0} ,Moneda[{1}][#2], d |h[{2}]\n", ex.Message, sMoneda, getTime());
                MostrarMensaje(sMensajeError, mp);
                //MessageBox.Show(sMensajeError);
            }
        }//fin

        public bool bitsoLogin()
        {
            bool bRegresa = false;
           
            try
            {
                //List<CBalance> lBalance =balanceItem.payload.balances.FindAll(delegate(CBalance bk)
                //{
                //    return bk.currency.ToString() != "";
                //});
                foreach (Balance item in GetAccountBalance() )
                {
                    lBalance.Add(item);
                    bRegresa= true;
                }                    
                //bitsoUserData();
                
            }
            catch (Exception ex) { MessageBox.Show("Ocurrio un error: " + ex.Message); }

            return bRegresa;
        }

        /// <summary>
        /// consultaHistorialMonedas()
        /// Descripcion: metodo para consultar el archivo historial de monedas de maximos y minimos de un mes
        /// </summary>
        /// <returns>true/false regresa verdadero si termino todo el proceso correctamente</returns>
        public bool consultaHistorialMonedas( )
        {
            bool bRegresa = false;
            string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
            string sMes = sMeses.ElementAt(DateTime.Now.Month);
            string sAnio = DateTime.Now.Year.ToString();

            //List<CMoneda_Historial> lHistrialmoney = new List<CMoneda_Historial>();
            lHistrialmoney.Clear();
            string sLine = string.Empty;
            //string sFile = @"C:\sys\tradingboots\historial\histomoney.json";
            string sFile = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneymes.json", sAnio, sMes);
            CMoneda_Historial mh = null;

            if (Directory.Exists(string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio, sMes)))
            {
                //if (File.Exists(@"C:\sys\tradingboots\historial\histomoney.json"))
                if (File.Exists(sFile))
                {
                    try
                    {
                        StreamReader file = new StreamReader(sFile);

                        //primera linea 
                        sLine = file.ReadLine();
                        if (sLine != "")
                        {
                            mh = JsonConvert.DeserializeObject<CMoneda_Historial>(sLine);                             
                            lHistrialmoney.Add(mh);
                        }

                        while (sLine != null)
                        {
                            sLine = file.ReadLine();
                            if (sLine != null)
                            {
                                mh = JsonConvert.DeserializeObject<CMoneda_Historial>(sLine);
                                //valida de que no se repita el dato de la moneda "moneda-dia"
                                if( !lHistrialmoney.Exists( x=>x.moneda== mh.moneda && x.tiempomax== mh.tiempomax ) )
                                    lHistrialmoney.Add(mh);
                            }
                            bRegresa = true;
                        }
                        file.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ocurrio un Error al momento de leer datos historico de monedas\n" + ex.Message, "Error de lectura de archivo");
                    }
                }//fin File.exist...
            }/// fin Directory.Exists..

            return bRegresa;
        }
        
    }
}

