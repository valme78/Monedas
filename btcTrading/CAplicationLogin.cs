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
using EntidadesGenerales;

public class CAplicationLogin
{
    public string api_name { get; private set; }
    public string api_key { get; private set; }
    public string api_secret { get; private set; }
    public int api_num_user { get; private set; }
    private string user;
    private string password;
    private CDatos_bitso cb = new CDatos_bitso();

    public List<CBalance> lBalances = new List<CBalance>();
    public List<CMonedaHistorial> lHistrialmoney = new List<CMonedaHistorial>();
    

    private class CDatos_bitso
    {
       public string url= "https://api.bitso.com/v3/balance/";
       public string api_key;
       public string api_secret;
       /*private TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks);
       private double ds = ts.TotalMilliseconds;*/
       public string dnoce =  Convert.ToInt64( new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds ).ToString();
       public string HTTPmethod = "GET";
       public string RequestPath = "v3/balance";
       public string JSONPayload = "";
        
    }
    internal static long CurrentUnixTimeMillis()
    {
        //return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        //return (long)(Jan1st1970).TotalMilliseconds;
        //return Split(DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds(), ".", 2)(0);
        return (DateTime.UtcNow.Ticks - 621355968000000000) / 1000;
    }

   public  CAplicationLogin(string api_name, string api_key, string api_secret,string user, string pwd)
    {
        //this.api_key = api_key;
        this.api_key = "jGahwvgWpc";
        //this.api_name = api_name;
        this.api_name = "bitso_key_78";
        //this.api_secret = api_secret;
        this.api_secret = "f3ed245422cafaab020059a0f78e37fc"; //33
        this.user = user;
        this.password = pwd;
        cb.api_key = this.api_key;
        cb.api_secret = this.api_secret;       
    }

   public CAplicationLogin() { }

   private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
   //internal static long CurrentUnixTimeMillis(){ return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds; }
  /* internal static sbyte[] GetBytes(this string self)
   {
       return GetSBytesForEncoding(System.Text.Encoding.UTF8, self);
   }
   internal static sbyte[] GetBytes(this string self, System.Text.Encoding encoding)
   {
       return GetSBytesForEncoding(encoding, self);
   }
   internal static sbyte[] GetBytes(this string self, string encoding)
   {
       return GetSBytesForEncoding(System.Text.Encoding.GetEncoding(encoding), self);
   }
    */
   private static sbyte[] GetSBytesForEncoding(System.Text.Encoding encoding, string s)
   {
       sbyte[] sbytes = new sbyte[encoding.GetByteCount(s)];
       encoding.GetBytes(s, 0, s.Length, (byte[])(object)sbytes, 0);
       return sbytes;
   } 
   internal static byte[] GetBytes(string self)
   {
       System.Text.Encoding encoding = System.Text.Encoding.UTF8;
       byte[] sbytes = new byte[encoding.GetByteCount(self)];
       encoding.GetBytes(self, 0, self.Length, (byte[])(object)sbytes, 0);

       return sbytes;//GetSBytesForEncoding(System.Text.Encoding.UTF8, self);
       
   }

   private static char[] getCharArray(string sKey)
   {
       char[] cChars = new char[sKey.Length];

       for (int i = 0; i < sKey.Length; i++)
       {
           cChars[i] = (char)sKey[i];
       }

       return cChars;
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

    /*
      
URL=https://api.bitso.com/v3/balance/
API_KEY="BITSO_KEY"
API_SECRET="BITSO_SECRET"
DNONCE=$(date +%s)
HTTPmethod=GET
JSONPayload=""
RequestPath="/v3/balance/"
SIGNATURE=$(echo -n $DNONCE$HTTPmethod$RequestPath$JSONPayload | openssl dgst -hex -sha256 -hmac $API_SECRET )
AUTH_HEADER="Bitso $API_KEY:$DNONCE:$SIGNATURE"
http GET $URL Authorization:"$AUTH_HEADER"
      */
    public bool bitsoLogin()
    {
        bool bRegresa = false;
        string sQuery = string.Empty;
        string SIGNATURE = string.Empty;
        string AUTH_HEADER = string.Empty;
        string MESSAGE = string.Empty;

        cb.RequestPath = "/v3/balance/";
        cb.url = "https://api.bitso.com/v3/balance/";
        //cb.url = "https://bitso.com/v3/balance/";

          //Dataos obtenido de una clase que al momento de la instancia se pasan los parametros.
        MESSAGE = cb.dnoce + cb.HTTPmethod + cb.RequestPath + cb.JSONPayload;        

        ASCIIEncoding encoder = new ASCIIEncoding();
        //Byte[] code = encoder.GetBytes(cb.api_secret); // se obtiene de los parametros de la clase.

        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(cb.api_secret)); //---code
        //Byte[] hashMe = encoder.GetBytes(MESSAGE);//

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(MESSAGE)); //--hashMe
        SIGNATURE = ToHexString(hm256Byte); //Aqui tengo mi sospecha que no se este generando como se deben.

       // SIGNATURE = "c96a3973409a5c122d0eb00ca27679edbe4faafb460564b626b76c5431f74150";
        AUTH_HEADER = "Bitso " + cb.api_key + ":" + cb.dnoce + ":" + SIGNATURE;        
        CLoadMoney balanceItem = null;

        try
        {                                   
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cb.url);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(" https://api.bitso.com/v3/account_status/");
            request.Method = cb.HTTPmethod;
            
            request.Headers.Add("Authorization", AUTH_HEADER);
            request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                balanceItem = JsonConvert.DeserializeObject<CLoadMoney>(json);

                //List<CBalance> lBalance =balanceItem.payload.balances.FindAll(delegate(CBalance bk)
                //{
                //    return bk.currency.ToString() != "";
                //});
                foreach (CBalance item in balanceItem.payload.balances)
                {
                   lBalances.Add(item);
                }
                bRegresa = true;
                //bitsoUserData();
            }
        }
        catch (Exception ex) { MessageBox.Show("Ocurrio un error: " + ex.Message); }

        return bRegresa;
    }

    /// <summary>
    /// Codificar
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns>texto en binario</returns>
    public string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Decodificar texto binario
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns>regresa texto en formato ascii o utf8</returns>
    public string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public bool leeDatosApi_Bitso(ref string sApiKey, ref string sApipwd,ref string sPwd)
    {
        string sLine = string.Empty;
        bool bRegresa = false;
        //string[] sMeses = { "", "ene", "feb", "mzo", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic" };
        //string sMes = sMeses.ElementAt(DateTime.Now.Month);
        //string sAnio = DateTime.Now.Year.ToString();
        //string sRuta = string.Format(@"C:\sys\tradingboots\historial\{0}\{1}\apibitso.txt", sAnio, sMes);

        try
        {
            if (Directory.Exists("C:\\sys\\tradingboots"))
            {
                if (File.Exists("C:\\sys\\tradingboots\\apiBitso.txt"))
                {
                    StreamReader file = new StreamReader(@"c:\sys\tradingboots\apibitso.txt");

                    //primera linea apikey
                    sApiKey = file.ReadLine();
                    sApipwd = file.ReadLine();
                    sPwd = file.ReadLine();
                    file.Close();
                }
            }
            if (sApiKey.Trim() != "" && sApipwd.Trim() != "")
                bRegresa = true;
        }
        catch ( Exception ex )
        {
            MessageBox.Show(ex.Message, "Error al Abrir archivo configuración");
        }

        return bRegresa;
        
    }
    
    public bool guardarDatosApi_Bitso(string sApiKey, string sApipwd, string sPwd)
    {
        Boolean bRegresa = false;
        string sRuta = @"c:\sys\tradingboots\apiBitso.txt";

        try
        {
            if (!Directory.Exists(@"c:\sys"))
            {
                File.Create(@"c:\sys");
            }
            if (!Directory.Exists(@"c:\sys\tradingboots"))
            {
                File.Create("c:\\sys\\tradingboots");
            }

            //StreamWriter wr = new StreamWriter(sRuta);
            //Crear el archivo con la informacion
            using (StreamWriter sw = File.CreateText(sRuta))
            {
                sw.WriteLine(Base64Encode(sApiKey));
                sw.WriteLine(Base64Encode(sApipwd));
                sw.WriteLine(Base64Encode(sPwd));

                bRegresa = true;
                sw.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ocurrio un Error al momento de guardar datos de api\n" + ex.Message ,"Ocurrio un error!!");
        }

        return bRegresa;
    }

    public bool bitsoUserData()
    {
        bool bRegresa = false;
        
        string SIGNATURE = string.Empty;
        string AUTH_HEADER = string.Empty;
        string MESSAGE = string.Empty;
        string REQUESTPATH = "/v3/account_status/";
        string sURL = @"https://api.bitso.com/v3/account_status/";
        //string sNONCE = (CurrentUnixTimeMillis()).ToString();
        string sNONCE = Convert.ToInt64(new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds).ToString();

        //Dataos obtenido de una clase que al momento de la instancia se pasan los parametros.
        //MESSAGE = cb.dnoce + "GET" + REQUESTPATH + cb.JSONPayload;
        MESSAGE = sNONCE + "GET" + REQUESTPATH + cb.JSONPayload;

        ASCIIEncoding encoder = new ASCIIEncoding();
        HMACSHA256 hmSha256 = new HMACSHA256(encoder.GetBytes(cb.api_secret));

        byte[] hm256Byte = hmSha256.ComputeHash(encoder.GetBytes(MESSAGE));
        SIGNATURE = ToHexString(hm256Byte);  

        AUTH_HEADER = "Bitso " + cb.api_key + ":" + sNONCE + ":" + SIGNATURE;
        CLoadStatusBitso statusBitso = null;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);            
            request.Method = "GET";

            request.Headers.Add("Authorization", AUTH_HEADER);
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                statusBitso = JsonConvert.DeserializeObject<CLoadStatusBitso>(json);
                                
                /*foreach (CBalance item in statusBitso.payload)
                {
                    lBalances.Add(item);
                } */
                bRegresa = true;
            }
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
}

