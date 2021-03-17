using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace btcTrading
{
    public partial class FrmLog : Form
    {
        public TextBox txtlog = new TextBox();

        public FrmLog()
        {
            InitializeComponent();
        }


        private void FrmLog_Load(object sender, EventArgs e)
        {
            txtlogerror.Text = txtlog.Text;
        }

        private void FrmLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else if (e.KeyCode == Keys.F8)
                txtlogerror.Clear();
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            txtlogerror.Text = "";
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            txtlog.Text = txtlogerror.Text;
        }

       
    }
}
