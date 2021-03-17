using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntidadesGenerales
{

    public class CBalance
    {
        /// <summary>
        /// currency: variable donde contiene el nombre de la moneda. Ej [xrp_mxn, btc_mxn..]
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// available: variable donde contiene la cantidad en monedas que se esta intentando comprar/vender.
        /// </summary>
        public string available { get; set; }
        /// <summary>
        /// locked: variable donde contiene la cantidad en precio de la moenda que se puso en una orden.
        /// </summary>
        public string locked { get; set; }
        /// <summary>
        /// total: variable donde contiene el total valor precio que equivale la cantidad por el precio que se lano la combra/venta.
        /// </summary>
        public string total { get; set; }
        public string pending_deposit { get; set; }
        public string pending_withdrawal { get; set; }
    }

    /// <summary>
    /// Cordenes : clase para almacenar datos de las ordenes lanzadas.
    /// </summary>
    public class COrdenes
    {
        /// <summary>
        /// moneda: nombre de la moneda en orden
        /// </summary>
        public string moneda { get; set; }
        /// <summary>
        /// valor: valor o cantidad en moneda que se lanzo la orden
        /// </summary>
        public double valor { get; set; }
        /// <summary>
        /// precio: precio en lo cual se compro/vendio la moneda seleccionada.
        /// </summary>
        public double precio { get; set; }
        public DateTime hora { get; set; }
        public string orden { get; set; }
        public string status { get; set; }
        public double cantmonedas { get; set; }
        public string id { get; set; }
        public string side { get; set; }
    }
    public class CPayload
    {
        public IList<CBalance> balances { get; set; }
    }

    public class CLoadMoney
    {
        public bool success { get; set; }
        public CPayload payload { get; set; }
    }

    public class CStatusUser
    {
        public string client_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string status { get; set; }
        public string daily_limit { get; set; }
        public string monthly_limit { get; set; }
        public string daily_remaining { get; set; }
        public string monthly_remaining { get; set; }
        public string cellphone_number { get; set; }
        public string cellphone_number_stored { get; set; }
        public string email_stored { get; set; }
        public string official_id { get; set; }
        public string proof_of_residency { get; set; }
        public string signed_contract { get; set; }
        public string origin_of_funds { get; set; }
    }

    public class CLoadStatusBitso
    {
        public bool success { get; set; }
        public CStatusUser payload { get; set; }
    }

    public class CMonedaHistorial
    {
        public string moneda { get; set; }
        public string valormax { get; set; }
        public string valormin { get; set; }
        public DateTime tiempomax { get; set; }
        public DateTime tiempomin { get; set; }
        public string valorclose { get; set; }
        public string valoropen { get; set; }
    }
    /// <summary>
    /// CMoneyPrice: clase para obtener los datos recientes de movimientos de cada moneda. se actualiza cada N segundos.
    /// </summary>
    public class CMoneyPrice
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

    /// <summary>
    /// CMoneda4dias: clase para capturar 4 dias de los movimientos de las monedas para tomar encuenta para lanzar una orden
    /// </summary>
    public class CMoneda4dias
    {
        public string moneda { get; set; }
        public double preciomin { get; set; }
        public double preciomax { get; set; }
        public DateTime fecha { get; set; }
    }
}

