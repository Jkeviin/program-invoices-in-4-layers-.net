﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib_LN_Factura;

namespace App_Factura
{
    public partial class PR_ListarProducto : Form
    {
        public PR_ListarProducto()
        {
            InitializeComponent();
            
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormListaClientes_Load(object sender, EventArgs e)
        {
            llenarGRid();
        }
        

        private void BtnCerrar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            PR_EditarProducto frm = new PR_EditarProducto();

            if (DgvProducto.SelectedRows.Count > 0)
            {
                frm.txtCodigo.Text = DgvProducto.CurrentRow.Cells[0].Value.ToString();
                frm.txtNombre.Text = DgvProducto.CurrentRow.Cells[1].Value.ToString();
                frm.txtDescripcion.Text = DgvProducto.CurrentRow.Cells[3].Value.ToString();
                frm.txtValor.Text = DgvProducto.CurrentRow.Cells[4].Value.ToString();
                frm.txtStock.Text = DgvProducto.CurrentRow.Cells[5].Value.ToString();
                frm.FormClosed += new FormClosedEventHandler(ActualizarGrid);
                frm.ShowDialog();
            }
            else
                MessageBox.Show("seleccione una fila por favor");
        }


        private void ActualizarGrid(object sender, FormClosedEventArgs e)
        {
            llenarGRid();
        }

        private void llenarGRid()
        {
            LN_Factura objProveedor = new LN_Factura();
            if (!objProveedor.USP_Listar_Productos(DgvProducto))
            {
                MessageBox.Show(objProveedor.Error);
                objProveedor = null;
                return;
            }
            objProveedor = null;
            return;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de ELIMINAR el producto?", "Alerta¡¡", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LN_Factura objProveedor = new LN_Factura();
                // declarar variables
                string codigo;
                try
                {
                    //capturar variables
                    codigo = DgvProducto.CurrentRow.Cells[0].Value.ToString();

                    //Enviar valores al LN
                    objProveedor.Cod_P = codigo;

                    if (!objProveedor.USP_Eliminar_Categoria())
                    {
                        MessageBox.Show(objProveedor.Error);
                        objProveedor = null;
                        return;
                    }
                    llenarGRid();
                    MessageBox.Show(objProveedor.Error);
                    objProveedor = null;
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    objProveedor = null;
                }
            }
        }
    }
}
