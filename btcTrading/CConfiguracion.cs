using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace btcTrading
//{
    
    class CConfiguracion
    {
        //void CConfiguracion() { }

        private string subeSound;
        private string bajaSound;

        public string SubeSound
        {
          get { return subeSound; }
          set { subeSound = value; }
        }       

        public string BajaSound
        {
            get { return bajaSound; }
            set { bajaSound = value; }
        } 
    }

    public class OrderPlaced
    {
        public string book { get; set; }
        public string side { get; set; }
        public string major { get; set; }
        public string price { get; set; }
        public string type { get; set; }
    }
    public class Trading_oid
    {
        public string moneda { get; set; }
        public string oid { get; set; }
    }
//}
