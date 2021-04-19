using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EntidadesGenerales;

namespace btcTrading
{
    public partial class FrmHistorialOrdenes : Form
    {
        public FrmHistorialOrdenes()
        {
            InitializeComponent();
        }
        public class ChistorialOrdenes
        {
            public string moneda { get; set; }
            public double valor { get; set; }
            public double precio { get; set; }
            public DateTime hora { get; set; }
            public string orden { get; set; }
            public string status { get; set; }
            public string id { get; set; }
        }

        public class CStatusOrden
        {
            public string moneda { get; set; }
            public DateTime dTime { get; set; }
            public string status { get; set; }
        }

        //public List<ChistorialOrdenes> lstHOrdenes = new List<ChistorialOrdenes>();
        public List<COrdenes> lstHOrdenes = new List<COrdenes>();
        public List<CStatusOrden> lstStatusOrden = new List<CStatusOrden>();

        private void FrmHistorialOrdenes_Load(object sender, EventArgs e)
        {
            dgvhOrdenes.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvhOrdenes.Columns["colprecio"].DefaultCellStyle.Format = "c";
            dgvhOrdenes.Columns["colprecio"].DefaultCellStyle.Alignment= DataGridViewContentAlignment.MiddleRight;
            dgvhOrdenes.Columns["colvalor"].DefaultCellStyle.Format = "c";
            dgvhOrdenes.Columns["colvalor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dgvhOrdenes.Columns("colprecio").DefaultCellStyle.Format = "c";

            /*DataGridViewCell celFile = dataGridView1.Rows[i].Cells["dgcolPathFile"];
                DataGridViewCell celMax = dataGridView1.Rows[i].Cells["dgcolMax"];
                DataGridViewCell celMin = dataGridView1.Rows[i].Cells["dgcolMin"];
                DataGridViewCell celMoneda = dataGridView1.Rows[i].Cells["dgcolMonedas"];
                DataGridViewCell celChek = dataGridView1.Rows[i].Cells["dgcolchek"];
            */

            if (lstHOrdenes.Count > 0)
            {
                dgvhOrdenes.Rows.Add(lstHOrdenes.Count);
                for (int i = 0; i < lstHOrdenes.Count; i++)
                {
                    dgvhOrdenes.Rows[i].Cells["colMoneda"].Value = lstHOrdenes[i].moneda;
                    dgvhOrdenes.Rows[i].Cells["colprecio"].Value = lstHOrdenes[i].precio;
                    dgvhOrdenes.Rows[i].Cells["colvalor"].Value = lstHOrdenes[i].valor;
                    dgvhOrdenes.Rows[i].Cells["coltiempo"].Value = lstHOrdenes[i].hora.ToString("dd/MM/yy [hh:mm tt]");
                    dgvhOrdenes.Rows[i].Cells["colorden"].Value = lstHOrdenes[i].orden;
                    dgvhOrdenes.Rows[i].Cells["colstatus"].Value = lstHOrdenes[i].status;
                    dgvhOrdenes.Rows[i].Cells["colid"].Value = lstHOrdenes[i].id;
                }

            }
            else
            {
                MessageBox.Show("No existen Historial de Ordenes para este Día");
                this.Close();
            }
        }

        private void FrmHistorialOrdenes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}
