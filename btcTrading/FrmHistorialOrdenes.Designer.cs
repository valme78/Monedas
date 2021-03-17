namespace btcTrading
{
    partial class FrmHistorialOrdenes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvhOrdenes = new System.Windows.Forms.DataGridView();
            this.colMoneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colprecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colvalor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coltiempo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colorden = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colstatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvhOrdenes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvhOrdenes
            // 
            this.dgvhOrdenes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvhOrdenes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMoneda,
            this.colprecio,
            this.colvalor,
            this.coltiempo,
            this.colorden,
            this.colstatus,
            this.colid});
            this.dgvhOrdenes.Location = new System.Drawing.Point(1, 25);
            this.dgvhOrdenes.Name = "dgvhOrdenes";
            this.dgvhOrdenes.RowHeadersVisible = false;
            this.dgvhOrdenes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvhOrdenes.Size = new System.Drawing.Size(583, 464);
            this.dgvhOrdenes.TabIndex = 0;
            // 
            // colMoneda
            // 
            this.colMoneda.HeaderText = "Moneda";
            this.colMoneda.MaxInputLength = 120;
            this.colMoneda.Name = "colMoneda";
            this.colMoneda.ReadOnly = true;
            this.colMoneda.Width = 60;
            // 
            // colprecio
            // 
            this.colprecio.HeaderText = "Precio";
            this.colprecio.MaxInputLength = 14;
            this.colprecio.Name = "colprecio";
            this.colprecio.ReadOnly = true;
            this.colprecio.ToolTipText = "Cantidad de valor que se adquirio o vendio en moneda.";
            this.colprecio.Width = 60;
            // 
            // colvalor
            // 
            this.colvalor.HeaderText = "Valor";
            this.colvalor.MaxInputLength = 14;
            this.colvalor.Name = "colvalor";
            this.colvalor.ReadOnly = true;
            this.colvalor.ToolTipText = "Valor $ a lo que se adquirio la orden[precio]";
            this.colvalor.Width = 60;
            // 
            // coltiempo
            // 
            this.coltiempo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.coltiempo.HeaderText = "Fecha/ Hora Orden";
            this.coltiempo.MaxInputLength = 20;
            this.coltiempo.Name = "coltiempo";
            this.coltiempo.ReadOnly = true;
            this.coltiempo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.coltiempo.ToolTipText = "Hora que se genero la order de compra/venta";
            // 
            // colorden
            // 
            this.colorden.HeaderText = "Orden";
            this.colorden.MaxInputLength = 10;
            this.colorden.Name = "colorden";
            this.colorden.ReadOnly = true;
            this.colorden.ToolTipText = "Tipo de orden que se lanzo[Compra/Venta]";
            this.colorden.Width = 50;
            // 
            // colstatus
            // 
            this.colstatus.HeaderText = "Status/Hora";
            this.colstatus.Name = "colstatus";
            this.colstatus.ReadOnly = true;
            this.colstatus.ToolTipText = "Status de la Compra/venta si se termino se pone fehca-hora y si se cancela tambie" +
                "n.";
            this.colstatus.Width = 130;
            // 
            // colid
            // 
            this.colid.HeaderText = "id";
            this.colid.MaxInputLength = 50;
            this.colid.Name = "colid";
            this.colid.ReadOnly = true;
            // 
            // FrmHistorialOrdenes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 492);
            this.Controls.Add(this.dgvhOrdenes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmHistorialOrdenes";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Historial de Ordenes por Moneda Realizadas en el Dia";
            this.Load += new System.EventHandler(this.FrmHistorialOrdenes_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmHistorialOrdenes_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvhOrdenes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvhOrdenes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMoneda;
        private System.Windows.Forms.DataGridViewTextBoxColumn colprecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colvalor;
        private System.Windows.Forms.DataGridViewTextBoxColumn coltiempo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colorden;
        private System.Windows.Forms.DataGridViewTextBoxColumn colstatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colid;
    }
}