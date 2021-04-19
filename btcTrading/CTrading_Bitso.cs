using System;
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
using System.Security.Cryptography;
using System.Threading;



public class CTrading_Bitso
{
    public buyMoney buym = new buyMoney();
    public List<CMoneda_Historial> lHistrialmoney = new List<CMoneda_Historial>();
    public List<Payload_orden> lOrdenes_activas = new List<Payload_orden>();

    private readonly string BITSO_KEY;
    private readonly string BITSO_SECRET;
    private readonly string BITSO_API_URL;
    private readonly string BITSO_VERSION_PREFIX;

    public class buyMoney
    {
        public string book { get; set; }
        public string side { get; set; }
        public string major { get; set; }
        public string price { get; set; }
        public string type { get; set; }        
    }

    public class CRes_tradeIdResult
    {
        public string oid { get; set; }
    }

    public class CResTradingjson
    {
        public bool success { get; set; }
        public CRes_tradeIdResult payload { get; set; }
    }
    public class CMoney_Trading_oid
    {
        public string moneda { get; set; }
        public string oid { get; set; }
    }

    public class CMoneda_Historial
    {
        public string moneda { get; set; }
        public string valormax { get; set; }
        public string valormin { get; set; }
        public DateTime tiempomax { get; set; }
        public DateTime tiempomin { get; set; }
    }
    public class COpen_Orders
    {
        public string book { get; set; }
        public string marker { get; set; }
        public string sort { get; set; }
        public string limit { get; set; }
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

    //---- clase para usarlo en ordenes activas
    public class Payload_orden
    {
        public string original_value { get; set; }
        public string unfilled_amount { get; set; }
        public string original_amount { get; set; }
        public string book { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string price { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string oid { get; set; }
        public string status { get; set; }
    }

    public class C_Ordenes_activas
    {
        public bool success { get; set; }
        public List<Payload_orden> payload { get; set; }
    }

    //------   Fin clases -----

    private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    internal static long CurrentUnixTimeMillis()
    {
        //return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        //return (long)(Jan1st1970).TotalMilliseconds;
        //return Split(DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds(), ".", 2)(0);
        return (DateTime.UtcNow.Ticks - 621355968000000000) / 1000;
    }

    public CMoney_Trading_oid bitsoTrading(string sMoneda)
    {
        //bool bRegresa = false;

        //string APIKEY = "jGahwvgWpc";
        string APIKEY = "JkkXaYKKDo";
        //string API_SECRET = "f3ed245422cafaab020059a0f78e37fc";
        string API_SECRET = "578d14362dd2d68d05cb4dd0b2a92012";// Bitso_Trading
        string SIGNATURE = string.Empty;
        string AUTH_HEADER = string.Empty;
        string MESSAGE = string.Empty;
        string REQUESTPATH = "/v3/orders/";
        string sURL = @"https://api.bitso.com/v3/orders/";
        string DNONCE = Convert.ToInt64(new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds).ToString();
        //string DNONCE = (CurrentUnixTimeMillis()).ToString();
        string JSONPayload = JsonConvert.SerializeObject(buym);
        
//          string json = @"{
//               'book': 'xrp_mxn',
//               'side': 'buy',
//               'major': '1',
//               'price': '19',
//               'type': 'limit'}";
          //string JSONPayload = JsonConvert.SerializeObject(json);

        //Dataos obtenido de una clase que al momento de la instancia se pasan los parametros.
        MESSAGE = DNONCE + "POST" + REQUESTPATH + JSONPayload;

        ASCIIEncoding encoder = new ASCIIEncoding();
        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(API_SECRET));

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(MESSAGE));
        SIGNATURE = ToHexString(hm256Byte);
        
        AUTH_HEADER = "Bitso " + APIKEY + ":" + DNONCE + ":" + SIGNATURE;
        CResTradingjson resTrading = null;
        CMoney_Trading_oid moneyResult = new CMoney_Trading_oid();
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
            request.Method = "POST";
            
            request.Headers.Add("Authorization", AUTH_HEADER);
            request.ContentType = "application/json";
            //request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            //request.ProtocolVersion = HttpVersion.Version11;            

            /// ejemplo 2
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JSONPayload);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                resTrading = JsonConvert.DeserializeObject<CResTradingjson>(result);
                if (resTrading.success)
                {
                    moneyResult.moneda = sMoneda;
                    moneyResult.oid = resTrading.payload.oid;
                }
                //MessageBox.Show("Resultado del Treadin: \n" + result );
            }
            //aqui termina ejemplo 2
        }
        catch (WebException e)   
        {        
            using(WebResponse response = e.Response)   
            {  
                HttpWebResponse httpResponse = (HttpWebResponse) response;  
                //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);  
                using(Stream data = response.GetResponseStream())  
                using(var reader = new StreamReader(data))   
                {  
                    string text = reader.ReadToEnd();
                    MessageBox.Show(text + " : " + httpResponse.StatusCode);  
                    
                }  
            }  
       }
       catch (Exception ex) { MessageBox.Show("Ocurrio un error: " + ex.Message ); }

        return moneyResult;
    }

    /// <summary>
    /// consultaHistorialMonedas()
    /// Descripcion: metodo para consultar el archivo historial de monedas de maximos y minimos de un mes
    /// </summary>
    /// <returns>true/false regresa verdadero si termino todo el proceso correctamente</returns>
    public bool consultaHistorialMonedas( /*System.Windows.Forms.DataVisualization.Charting.Chart graficahist*/ )
    {
        bool bRegresa = false;
        string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
        string sMes = sMeses.ElementAt(DateTime.Now.Month);
        string sAnio = DateTime.Now.Year.ToString();
        
        //List<CMoneda_Historial> lHistrialmoney = new List<CMoneda_Historial>();
        lHistrialmoney.Clear();
        string sLine = string.Empty;
        //string sFile = @"C:\sys\tradingboots\historial\histomoney.json";
        string sFile = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\histomoneymes.json", sAnio,sMes);
        CMoneda_Historial mh = null;

        if (Directory.Exists( string.Format(@"C:\sys\tradingboots\historial\{0}\{1}", sAnio,sMes) ))
        {
            //if (File.Exists(@"C:\sys\tradingboots\historial\histomoney.json"))
            if (File.Exists(sFile ))
            {
                try
                {
                    StreamReader file = new StreamReader(sFile);

                    //primera linea 
                    sLine = file.ReadLine();
                    if (sLine != "")
                    {
                        mh = JsonConvert.DeserializeObject<CMoneda_Historial>(sLine);
                        lHistrialmoney.Add( mh );
                    }

                    while (sLine != null)
                    {
                        sLine = file.ReadLine();
                        if (sLine != null )
                        {
                            mh = JsonConvert.DeserializeObject<CMoneda_Historial>(sLine);
                            lHistrialmoney.Add(mh);
                        }
                        bRegresa = true;
                    }
                    file.Close();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ocurrio un Error al momento de leer datos historico de monedas\n" + ex.Message , "Error de lectura de archivo");
                }
            }//fin File.exist...
        }/// fin Directory.Exists..

        return bRegresa;
    }

    public bool consultaOrdenesActivas(string sMoneda)
    {
        bool bRegresa = false;
        string SIGNATURE = string.Empty;
        string AUTH_HEADER = string.Empty;
        string MESSAGE = string.Empty;
        string JSONPayload = string.Empty;
        string REQUESTPATH = string.Empty;

        //string APIKEY = "JkkXaYKKDo";
        //string API_SECRET = "578d14362dd2d68d05cb4dd0b2a92012";// Bitso_Trading
        
        string APIKEY = "KzWPedEtfW";//Api_ordenes
        string API_SECRET = "a93cc38e5ef5991d163e0c008f15620a";//Api_ordenes
        
        string DNONCE = Convert.ToInt64(new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds).ToString();
        //var DNONCE = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        
       // REQUESTPATH = @"/v3/open_orders/"; //v3/balance/  --v3/orders/
        string sURL = @"https://api.bitso.com/api/v3/open_orders" + BuildQueryString("book", "xrp_mxn", "marker", "", "sort", "desc", "limit", "25");  //  ?book=btc_mxn"; //?book=" + sMoneda.Trim();  <----

        REQUESTPATH = "/api/v3/open_orders" + BuildQueryString("book", "xrp_mxn", "marker", "", "sort", "desc", "limit", "25");
        MESSAGE = DNONCE + "GET" + REQUESTPATH + JSONPayload;

        ASCIIEncoding encoder = new ASCIIEncoding();
        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(API_SECRET));

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(MESSAGE));
        SIGNATURE = ToHexString(hm256Byte);

        AUTH_HEADER = "Bitso " + APIKEY + ":" + DNONCE + ":" + SIGNATURE;
        
        //CLoadStatusBitso statusBitso = null;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
            request.Method = "GET";

            request.Headers.Add("Authorization", AUTH_HEADER);
            request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.Accept = "application/json";

            if (!string.IsNullOrEmpty(JSONPayload))
            {
                using (var req = request.GetRequestStream())
                {
                    var bodyBytes = Encoding.UTF8.GetBytes(JSONPayload);
                    req.Write(bodyBytes, 0, bodyBytes.Length);
                }
            }
                        
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                //MessageBox.Show(json);
                C_Ordenes_activas ordenes=null;
                ordenes = JsonConvert.DeserializeObject<C_Ordenes_activas>(json);

                foreach (Payload_orden item in ordenes.payload)
                {
                    lOrdenes_activas.Add(item);
                } 
                bRegresa = true;
            }
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show("error al momento de obtener ordenes activas :" + ex.Message);
        }
        catch (WebException wx)
        {
            using (WebResponse response = wx.Response)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);  
                using (Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    MessageBox.Show(text + " : " + httpResponse.StatusCode);
                }
            }
        }
        catch (Exception ex) { MessageBox.Show("Ocurrio un error: " + ex.Message); }

        return bRegresa;
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

    public bool eliminaOrden(string sMoneda, string sOid)
    {
        bool bregresa = false;
        //
        string APIKEY = "JkkXaYKKDo";        
        string API_SECRET = "578d14362dd2d68d05cb4dd0b2a92012";// Bitso_Trading
        string SIGNATURE = string.Empty;
        string AUTH_HEADER = string.Empty;
        string MESSAGE = string.Empty;
        string REQUESTPATH = "";// "/v3/orders/";
        string sURL = string.Format( @"https://api.bitso.com/api/v3/orders/{0}",sOid );
        string DNONCE = Convert.ToInt64(new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds).ToString();
        string JSONPayload = ""; // JsonConvert.SerializeObject(buym);

        //Dataos obtenido de una clase que al momento de la instancia se pasan los parametros.
        MESSAGE = DNONCE +  REQUESTPATH + JSONPayload;

        ASCIIEncoding encoder = new ASCIIEncoding();
        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(API_SECRET));

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(MESSAGE));
        SIGNATURE = ToHexString(hm256Byte);

        AUTH_HEADER = "Bitso " + APIKEY + ":" + DNONCE + ":" + SIGNATURE;
        CResTradingjson resTrading = null;
       
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
            request.Method = "DELETE";

            request.Headers.Add("Authorization", AUTH_HEADER);
            request.ContentType = "application/json";            
            //request.Accept = "application/json";

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                resTrading = JsonConvert.DeserializeObject<CResTradingjson>(result);
                if (resTrading.success)
                {
                    Dictionary<bool, string> res = new Dictionary<bool, string>();                    
                    
                }
                //MessageBox.Show("Resultado del Treadin: \n" + result );
            }
            
        }
        catch (WebException e)
        {
            using (WebResponse response = e.Response)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);  
                using (Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    MessageBox.Show(text + " : " + httpResponse.StatusCode);

                }
            }
        }
        catch (Exception ex) { MessageBox.Show("Ocurrio un error: " + ex.Message); }


        return bregresa;
    }

    /// <summary>
    /// HMACSHA256, metodo para regresar datos de autenticacion para el api
    /// </summary>
    /// <param name="SIGNATURE">nonce + method + requestPath + body</param>
    /// <param name="BITSO_SECRET">clave de bitso que se da de alta en la paguina web de bitso</param>
    /// <returns>sResult, regresa los datos encriptados en hexadecimal</returns>
    private string HMACSHA256(string SIGNATURE ,string BITSO_SECRET)
    {
        string sResult = string.Empty;

        ASCIIEncoding encoder = new ASCIIEncoding();
        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(BITSO_SECRET));

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(SIGNATURE));
        sResult = ToHexString(hm256Byte);

        return sResult;
    }

    public string SendRequest(string url, string method, bool signRequest , string body )
        {
            var requestPath = BITSO_VERSION_PREFIX + url;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(BITSO_API_URL + requestPath);

            if (signRequest)
            {
                //Authorization: Bitso <key>:<nonce>:<signature>
                //key = The API Key you generated
                //nonce = An Integer that must be unique and increasing for each API call
                //signature = The signature is generated by creating a SHA256 HMAC using the Bitso API Secret on the concatenation of nonce + HTTP method + requestPath + JSON payload (no ’+’ signs in the concatenated string) and hex encode the output. 

                //var nonce = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                //var signature = BitsoUtils.ToHexString(BitsoUtils.HMACSHA256(nonce + method + requestPath + body, BITSO_SECRET));

                //httpWebRequest.Headers["Authorization"] = $"Bitso {BITSO_KEY}:{nonce}:{signature}";
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
                       // throw new BitsoException("No response was returned from Bitso.", "0");

                    using (var str = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(str))
                        {
                            response = reader.ReadToEnd();

                            if (res.StatusCode == HttpStatusCode.NotFound && response.StartsWith("<"))
                                MessageBox.Show("The requested resource was not found.", "-1");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // throw new BitsoException(ex.Message, "0");
            }

            if (string.IsNullOrEmpty(response) || response.StartsWith("<"))
                MessageBox.Show("A malformed response was returned from Bitso.", "-1");

           // var responseObj = JsonConvert.DeserializeObject<JObject>(response);

            //if (responseObj == null)
            //    MessageBox.Show("No response was returned from Bitso.", "0");

            //if (responseObj["success"].Value<bool>())
            //{
            //    if(method == "GET" && url == "balance") //This was hardcoded to mantain consistency in the response
            //        return responseObj["payload"]["balances"].ToString();

            //    return responseObj["payload"].ToString();
            //}

            //MessageBox.Show(responseObj["error"]["message"].ToString(), responseObj["error"]["code"].ToString());

            return "";
        }
}

