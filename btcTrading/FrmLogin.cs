using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EntidadesGenerales;

namespace btcTrading
{
    public partial class FrmLogin : Form
    {
        CAplicationLogin login= new CAplicationLogin();
        public List<CBalance> lBalances = new List<CBalance>();

        //variable global para el pwd de cifrado de datos.
        private string sPwdPlataforma = string.Empty;
        private bool bPasswordActivo = false;


        //valirables Globales publicas para pasar al dialogo principal
        public string sApi_key = string.Empty;
        public string sApi_pwd = string.Empty;
        public string sPlataforma = string.Empty;
        public List<CMonedaHistorial> lstHistrialmoney_login = new List<CMonedaHistorial>();
        
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            login = new CAplicationLogin();
            sPlataforma = cboplataforma.SelectedItem.ToString();
            
            //if (validaControles())
            {
                //if (login.bitsoLogin())
                {
                    //lBalances = login.lBalances;

                    if (cboplataforma.SelectedItem.ToString().Trim() == "Bitso" )
                    {
                        //login.guardarDatosApi_Bitso(txtapikey.Text.Trim(), txtapipwd.Text.Trim(), txtPwd.Text.Trim());
                        Bitso bitsoclient = new Bitso("JkkXaYKKDo", "578d14362dd2d68d05cb4dd0b2a92012", true);
                        if (bitsoclient.bitsoLogin())
                        {
                            //lBalances = bitsoclient.lBalance;
                            //lBalances.Add(
                            foreach (var val in bitsoclient.lBalance)
                            {
                                if( float.Parse(val.Total) > 0.0 )
                                    lBalances.Add(new CBalance { currency = val.Currency, available = val.Available, locked = val.Locked, total = val.Total, pending_deposit = "0.0", pending_withdrawal = "0.0" });                                
                            }
                        }
                        
                        //CTrading_Bitso bt = new CTrading_Bitso();
                        bitsoclient.consultaHistorialMonedas();
                        foreach( var mh in bitsoclient.lHistrialmoney)
                        {
                            this.lstHistrialmoney_login.Add(new CMonedaHistorial { moneda = mh.moneda, valormin = mh.valormin, valormax = mh.valormax, tiempomin = mh.tiempomin, tiempomax = mh.tiempomax });
                        }
                    }
                    if (txtapikey.Text.Trim() == "")
                    {
                        sApi_key = "JkkXaYKKDo";
                        sApi_pwd = "578d14362dd2d68d05cb4dd0b2a92012";
                    }
                    else
                    {
                        sApi_key = txtapikey.Text.Trim();
                        sApi_pwd = txtapipwd.Text.Trim();
                    }

                    

                    this.Close();
                    DialogResult = DialogResult.OK;
                }
            }
           
        }

        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            
        }

        private void cboplataforma_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPlataforma = string.Empty;
            if (cboplataforma.SelectedIndex >= 0)
            {
                sPlataforma = cboplataforma.SelectedItem.ToString();
                txtPwd.ReadOnly = leerconfiguaracionPlataforma(sPlataforma);
                btnLogin.Enabled = true;
            }
            else
            {
                txtPwd.ReadOnly = true;
                btnLogin.Enabled = false;
            }
        }
       
        private bool leerconfiguaracionPlataforma(string sPlataforma)
        {
            bool bRegresa = false;
            string sApikey = string.Empty, sApipwd = string.Empty, spwd = string.Empty;
            if (sPlataforma.Trim() == "Bitso")
            {
                login = new CAplicationLogin("", "", "", "", "");
                if (login.leeDatosApi_Bitso(ref sApikey, ref sApipwd, ref spwd))
                {
                    bRegresa = true;
                }
            }
            return bRegresa;
        }

        private void txtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (validaControles())
                {
                    if (txtPwd.Text.Trim().Length >= 5)
                    {
                        if (sPwdPlataforma != "")
                        {
                            login = new CAplicationLogin("", "", "", "", "");
                            string sPwdDec = login.Base64Decode(sPwdPlataforma);
                            if (txtPwd.Text.Trim() == sPwdDec)
                            {
                                txtapikey.Text = login.Base64Decode(txtapikey.Text);
                                txtapipwd.Text = login.Base64Decode(txtapipwd.Text);
                                bPasswordActivo = true;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("El Password debe ser mayor o igual a 5 caracteres");
                    }                    
                }// fin cboplataforma
            }
        }

        private bool validaControles()
        {
            bool bRegresa = false;
            if (cboplataforma.SelectedIndex < 0)
            {
                MessageBox.Show("Debe seleccionar una Plataforma");
            }
            else
            {
                if (txtapikey.Text.Trim() != "")
                {
                    MessageBox.Show("Deves ingresar un 'api key' para la plataforma Seleccionada");
                }
                else
                {
                    if (txtapipwd.Text.Trim() != "")
                    {
                        MessageBox.Show("Deves ingresaar un 'Api Password' para la plataforma Seleccionada");
                    }
                    else
                        bRegresa = true;
                }
            }

            return bRegresa;
        }
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            txtPwd.ReadOnly = true;
            btnLogin.Enabled = false;
        }       
    }
}
