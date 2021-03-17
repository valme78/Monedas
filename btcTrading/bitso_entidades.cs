using Newtonsoft.Json;
using System;

namespace bitso.Entities
{
    /// <summary>
    /// BookInfo : clase para obtener toda la información consultada de las monedas activas.
    /// </summary>
    public class BookInfo
    {
        [JsonProperty("book")]
        public string Book { get; set; }

        [JsonProperty("minimum_amount")]
        public string MinimumAmount { get; set; }

        [JsonProperty("maximum_amount")]
        public string MaximumAmount { get; set; }

        [JsonProperty("minimum_price")]
        public string MinimumPrice { get; set; }

        [JsonProperty("maximum_price")]
        public string MaximumPrice { get; set; }

        [JsonProperty("minimum_value")]
        public string MinimumValue { get; set; }

        [JsonProperty("maximum_value")]
        public string MaximumValue { get; set; }

        public decimal MinimumAmountAsDecimal { get { return Convert.ToDecimal(MinimumAmount); } }
        public decimal MaximumAmountAsDecimal { get { return Convert.ToDecimal(MaximumAmount); } }
        public decimal MinimumPriceAsDecimal { get { return Convert.ToDecimal(MinimumPrice); } }
        public decimal MaximumPriceAsDecimal { get { return Convert.ToDecimal(MaximumPrice); } }
        public decimal MinimumValueAsDecimal { get { return Convert.ToDecimal(MinimumValue); } }
        public decimal MaximumValueAsDecimal { get { return Convert.ToDecimal(MaximumValue); } }

    }

    public class Ticker
    {
        [JsonProperty("book")]
        public string Book { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("high")]
        public string PriceHigh { get; set; }

        [JsonProperty("last")]
        public string LastTradedPrice { get; set; }

        [JsonProperty("low")]
        public string PriceLow { get; set; }

        [JsonProperty("vwap")]
        public string VolumeWeightedAvgPrice { get; set; }

        [JsonProperty("ask")]
        public string Ask { get; set; }

        [JsonProperty("bid")]
        public string Bid { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        public decimal VolumeAsDecimal { get { return Convert.ToDecimal(Volume); } }
        public decimal PriceHighAsDecimal { get { return Convert.ToDecimal(PriceHigh); } }
        public decimal LastTradedPriceAsDecimal { get { return Convert.ToDecimal(LastTradedPrice); } }
        public decimal PriceLowAsDecimal { get { return Convert.ToDecimal(PriceLow); } }
        public decimal VolumeWeightedAvgPriceAsDecimal { get { return Convert.ToDecimal(VolumeWeightedAvgPrice); } }
        public decimal AskAsDecimal { get { return Convert.ToDecimal(Ask); } }
        public decimal BidAsDecimal { get { return Convert.ToDecimal(Bid); } }
    }

    /// <summary>
    /// Clase para obtener la información de las monedas que tiene el usuario de bitso.
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// currency: variable donde contiene el nombre de la moneda. Ej [xrp_mxn, btc_mxn..]
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
        /// <summary>
        /// total: variable donde contiene el total valor precio que equivale la cantidad por el precio que se lano la combra/venta.
        /// </summary>
        [JsonProperty("total")]
        public string Total { get; set; }
        /// <summary>
        /// locked: variable donde contiene la cantidad en precio de la moenda que se puso en una orden.
        /// </summary>
        [JsonProperty("locked")]
        public string Locked { get; set; }
        /// <summary>
        /// available: variable donde contiene la cantidad en monedas que se esta intentando comprar/vender.
        /// </summary>
        [JsonProperty("available")]
        public string Available { get; set; }

        public decimal TotalAsDecimal { get { return Convert.ToDecimal(Total); } }
        public decimal LockedAsDecimal { get { return Convert.ToDecimal(Locked); } }
        public decimal AvailableAsDecimal { get { return Convert.ToDecimal(Available); } }
    }
    /// <summary>
    /// OpenOrder : Clase para uso de ordenes activas de bitso.
    /// </summary>
    public class OpenOrder
    {
        [JsonProperty("original_value")]
        public string OriginalValue { get; set; }

        [JsonProperty("unfilled_amount")]
        public string UnfilledAmount { get; set; }

        [JsonProperty("original_amount")]
        public string OriginalAmount { get; set; }

        /// <summary>
        /// book: nombre de la moneda digital de bitso.
        /// </summary>
        [JsonProperty("book")]
        public string Book { get; set; }
        /// <summary>
        /// created_at: Fecha y hora en la que fue creada la orden.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        /// <summary>
        /// price : valor en el cual se comporo la moneda que se lanzo en orden de compa/venta.
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; }
        /// <summary>
        /// side : tipo de trading [buy/cell].
        /// </summary>
        [JsonProperty("side")]
        public string Side { get; set; }
        /// <summary>
        /// type: typo de orden [limit, mercado..]
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// oid: es el valor id de la orden ejecutada.
        /// </summary>
        [JsonProperty("oid")]
        public string Oid { get; set; }
        /// <summary>
        /// status: es el estatus en la que esta la orden, open,close, pendente..
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }


        public decimal OriginalValueAsDecimal { get { return Convert.ToDecimal(OriginalValue); } }
        public decimal UnfilledAmountAsDecimal { get { return Convert.ToDecimal(UnfilledAmount); } }
        public decimal OriginalAmountAsDecimal { get { return Convert.ToDecimal(OriginalAmount); } }
        public decimal PriceAsDecimal { get { return Convert.ToDecimal(Price); } }

    }

    public class UserTrade
    {
        // <summary>
        /// book: nombre de la moenda digital de bitso.
        /// </summary>
        [JsonProperty("book")]
        public string Book { get; set; }
        // <summary>
        /// major: valor mayor que tiene la moneda a la hora de lanzar la orden.
        /// </summary>
        [JsonProperty("major")]
        public string Major { get; set; }

        [JsonProperty("minor")]
        public string Minor { get; set; }
        /// <summary>
        /// price: valor en lo cual se compro/vendio la moneda.
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; }
        /// <summary>
        /// side: tipo de trading [buy/cell]
        /// </summary>
        [JsonProperty("side")]
        public string Side { get; set; }
        /// <summary>
        /// fees_currency : nombre de la moneda en la cual sera cobrado el % del Trading
        /// </summary>
        [JsonProperty("fees_currency")]
        public string FeesCurrency { get; set; }
        /// <summary>
        /// fees_amount: valor % del trading cobrado por la operacion en el valor del nombre moneda[ fees_currency].
        /// </summary>
        [JsonProperty("fees_amount")]
        public string FeesAmount { get; set; }

        [JsonProperty("tid")]
        public long Tid { get; set; }

        [JsonProperty("oid")]
        public string Oid { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }


        public decimal MajorAsDecimal { get { return Convert.ToDecimal(Major); } }
        public decimal MinorAsDecimal { get { return Convert.ToDecimal(Minor); } }
        public decimal PriceAsDecimal { get { return Convert.ToDecimal(Price); } }
        public decimal FeesAmountAsDecimal { get { return Convert.ToDecimal(FeesAmount); } }

    }

    public class CMoneda_Historial
    {
        public string moneda { get; set; }
        public string valormax { get; set; }
        public string valormin { get; set; }
        public DateTime tiempomax { get; set; }
        public DateTime tiempomin { get; set; }
    }
}
