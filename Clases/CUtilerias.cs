using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CUtilerias
{
    public class CUtilerias
    {
        /// <summary>
        /// Metodo para poner color intercalado entre renglones a un grid
        /// </summary>
        /// <param name="iOpcion"> [1 o 2] 1-listview param type, 2- datagidview type</param>
        /// <param name="dtvDatos"> variable tipo datagridview que se va a alternar color</param>
        private void alternaColor(int iOpcion, DataGridView  dtvDatos)
        {
            if (iOpcion == 1)
            {
                //for (int i = 0; i < lvMonedas.Items.Count; i++)
                //{
                //    if (lvMonedas.Items[i].Index % 2 == 0)
                //        lvMonedas.Items[i].BackColor = Color.DarkGray;
                //    else
                //        lvMonedas.Items[i].BackColor = Color.Aquamarine;
                //}
            }
            else
            {
                for (int i = 0; i < dtvDatos.Rows.Count - 1; i++)
                {
                    if (dtvDatos.Rows[i].Index % 2 == 0)
                        dtvDatos.Rows[i].DefaultCellStyle.BackColor = Color.DarkGray;
                    else
                        dtvDatos.Rows[i].DefaultCellStyle.BackColor = Color.Aquamarine;
                }
            }
        }
    }
}
