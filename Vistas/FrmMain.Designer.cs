namespace CoinTrader
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dgvMonedas = new System.Windows.Forms.DataGridView();
            this.tHilogral = new System.Windows.Forms.Timer(this.components);
            this.tHiloOrdenes = new System.Windows.Forms.Timer(this.components);
            this.txtlogerror = new System.Windows.Forms.TextBox();
            this.dgcolmoneda = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcolprecioact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcolpreciomin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcolpreciomax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtpreciomax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chartactual = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartactual)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.Color.Gray;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Cursor = System.Windows.Forms.Cursors.Default;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(244, 84);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.Legend = "Legend1";
            series1.Name = "Mes";
            series1.YValuesPerPoint = 4;
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(924, 255);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "prueba";
            this.chart1.Titles.Add(title1);
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // dgvMonedas
            // 
            this.dgvMonedas.AllowUserToAddRows = false;
            this.dgvMonedas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvMonedas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonedas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcolmoneda,
            this.dgcolprecioact,
            this.dgcolpreciomin,
            this.dgcolpreciomax});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMonedas.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvMonedas.Location = new System.Drawing.Point(1, 83);
            this.dgvMonedas.Name = "dgvMonedas";
            this.dgvMonedas.RowHeadersVisible = false;
            this.dgvMonedas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMonedas.Size = new System.Drawing.Size(237, 600);
            this.dgvMonedas.TabIndex = 1;
            this.dgvMonedas.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMonedas_CellContentClick);
            // 
            // tHilogral
            // 
            this.tHilogral.Interval = 6000;
            this.tHilogral.Tick += new System.EventHandler(this.tHilogral_Tick);
            // 
            // tHiloOrdenes
            // 
            this.tHiloOrdenes.Interval = 6000;
            // 
            // txtlogerror
            // 
            this.txtlogerror.Location = new System.Drawing.Point(12, 12);
            this.txtlogerror.Name = "txtlogerror";
            this.txtlogerror.Size = new System.Drawing.Size(813, 20);
            this.txtlogerror.TabIndex = 2;
            // 
            // dgcolmoneda
            // 
            this.dgcolmoneda.HeaderText = "Moneda";
            this.dgcolmoneda.MaxInputLength = 10;
            this.dgcolmoneda.Name = "dgcolmoneda";
            this.dgcolmoneda.ReadOnly = true;
            this.dgcolmoneda.Width = 60;
            // 
            // dgcolprecioact
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgcolprecioact.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgcolprecioact.HeaderText = "Precio Act";
            this.dgcolprecioact.MaxInputLength = 14;
            this.dgcolprecioact.Name = "dgcolprecioact";
            this.dgcolprecioact.ReadOnly = true;
            this.dgcolprecioact.Width = 80;
            // 
            // dgcolpreciomin
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgcolpreciomin.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgcolpreciomin.HeaderText = "Precio Min";
            this.dgcolpreciomin.MaxInputLength = 14;
            this.dgcolpreciomin.Name = "dgcolpreciomin";
            this.dgcolpreciomin.ReadOnly = true;
            this.dgcolpreciomin.Width = 80;
            // 
            // dgcolpreciomax
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgcolpreciomax.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgcolpreciomax.HeaderText = "Precio Max";
            this.dgcolpreciomax.MaxInputLength = 14;
            this.dgcolpreciomax.Name = "dgcolpreciomax";
            this.dgcolpreciomax.ReadOnly = true;
            this.dgcolpreciomax.Visible = false;
            // 
            // txtpreciomax
            // 
            this.txtpreciomax.Location = new System.Drawing.Point(148, 60);
            this.txtpreciomax.MaxLength = 14;
            this.txtpreciomax.Name = "txtpreciomax";
            this.txtpreciomax.Size = new System.Drawing.Size(81, 20);
            this.txtpreciomax.TabIndex = 3;
            this.txtpreciomax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Aquamarine;
            this.label1.Location = new System.Drawing.Point(72, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Precio Max";
            // 
            // chartactual
            // 
            this.chartactual.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartactual.BackColor = System.Drawing.Color.Gray;
            chartArea2.BackColor = System.Drawing.Color.Gray;
            chartArea2.Name = "ChartArea1";
            this.chartactual.ChartAreas.Add(chartArea2);
            this.chartactual.Cursor = System.Windows.Forms.Cursors.Default;
            legend2.Name = "Legend1";
            this.chartactual.Legends.Add(legend2);
            this.chartactual.Location = new System.Drawing.Point(244, 359);
            this.chartactual.Name = "chartactual";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series2.Legend = "Legend1";
            series2.Name = "Mes";
            series2.YValuesPerPoint = 4;
            this.chartactual.Series.Add(series2);
            this.chartactual.Size = new System.Drawing.Size(924, 314);
            this.chartactual.TabIndex = 0;
            title2.Name = "Title1";
            title2.Text = "prueba";
            this.chartactual.Titles.Add(title2);
            this.chartactual.Click += new System.EventHandler(this.chart1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(42)))), ((int)(((byte)(82)))));
            this.ClientSize = new System.Drawing.Size(1169, 685);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtpreciomax);
            this.Controls.Add(this.txtlogerror);
            this.Controls.Add(this.dgvMonedas);
            this.Controls.Add(this.chartactual);
            this.Controls.Add(this.chart1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trader Coin  Multiplataform";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonedas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartactual)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataGridView dgvMonedas;
        private System.Windows.Forms.Timer tHilogral;
        private System.Windows.Forms.Timer tHiloOrdenes;
        private System.Windows.Forms.TextBox txtlogerror;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcolmoneda;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcolprecioact;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcolpreciomin;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgcolpreciomax;
        private System.Windows.Forms.TextBox txtpreciomax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartactual;
    }
}

