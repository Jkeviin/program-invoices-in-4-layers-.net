﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App_Factura
{
    public partial class PR_DashBoard : Form
    {
        //Constructor
        public PR_DashBoard()
        {
            InitializeComponent();
            OcultarSubmenu();
            //Estas lineas eliminan los parpadeos del formulario o controles en la interfaz grafica (Pero no en un 100%)
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
        }

        #region Datos Proveedor Actual
        private string nit;
        private string nombre;
        private string direccion;
        private string descripcion;
        private string correo;
        private string web;

        public string Nit { get => nit; set => nit = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public string Correo { get => correo; set => correo = value; }
        public string Web { get => web; set => web = value; }

        #endregion

        //METODO PARA REDIMENCIONAR/CAMBIAR TAMAÑO A FORMULARIO  TIEMPO DE EJECUCION ----------------------------------------------------------
        private int tolerance = 15;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //----------------DIBUJAR RECTANGULO / EXCLUIR ESQUINA PANEL 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));

            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);

            region.Exclude(sizeGripRectangle);
            this.panelContenedorPrincipal.Region = region;
            this.Invalidate();
        }
        //----------------COLOR Y GRIP DE RECTANGULO INFERIOR
        protected override void OnPaint(PaintEventArgs e)
        {

            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(55, 61, 69));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);

            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }
       
        //METODO PARA ARRASTRAR EL FORMULARIO---------------------------------------------------------------------
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void PanelBarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        //METODOS PARA CERRAR,MAXIMIZAR, MINIMIZAR FORMULARIO------------------------------------------------------
        int lx, ly;
        int sw, sh;

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            lx = this.Location.X;
            ly = this.Location.Y;
            sw = this.Size.Width;
            sh = this.Size.Height;
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            this.Location = Screen.PrimaryScreen.WorkingArea.Location;
            btnMaximizar.Visible = false;
            btnNormal.Visible = true;

        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.Size = new Size(sw, sh);
            this.Location = new Point(lx, ly);
            btnNormal.Visible = false;
            btnMaximizar.Visible = true;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de cerrar?", "Alerta¡¡", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de Cerrar Sesión?", "Alerta¡¡", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Login Login = new Login();
                Login.Show();
                this.Close();
            }
        }

        //METODOS PARA ANIMACION DE MENU SLIDING--
        private void btnMenu_Click(object sender, EventArgs e)
        {
            //-------CON EFECTO SLIDING
            if (panelMenu.Width == 230)
            {
                this.tmContraerMenu.Start();
                this.btnCompras.Text = "";
                this.btnCategorias.Text = "";
                this.btnProductos.Text = "";
                this.btnAjustes.Text = "";
                OcultarSubmenu();
            }
            else if (panelMenu.Width == 55)
            {
                this.tmExpandirMenu.Start();
                this.btnCompras.Text = "           COMPRAS";
                this.btnCategorias.Text = "         CATEGORIAS";
                this.btnProductos.Text = "         PRODUCTOS";
                this.btnAjustes.Text = "            AJUSTES";
            }

            //-------SIN EFECTO 
            //if (panelMenu.Width == 55)
            //{
            //    panelMenu.Width = 230;
            //}
            //else

            //    panelMenu.Width = 55;
        }

        private void tmExpandirMenu_Tick(object sender, EventArgs e)
        {
            if (panelMenu.Width >= 230)
                this.tmExpandirMenu.Stop();
            else
                panelMenu.Width = panelMenu.Width + 5;
            
        }

        private void tmContraerMenu_Tick(object sender, EventArgs e)
        {
            if (panelMenu.Width <= 55)
                this.tmContraerMenu.Stop();
            else
                panelMenu.Width = panelMenu.Width - 5;
        }

        //METODO PARA ABRIR FORM DENTRO DE PANEL-----------------------------------------------------
        private void AbrirFormEnPanel(object formHijo)
        {
            if (this.panelContenedorForm.Controls.Count > 0)
                this.panelContenedorForm.Controls.RemoveAt(0);
            Form fh = formHijo as Form;
            fh.TopLevel = false;
            fh.FormBorderStyle = FormBorderStyle.None;
            fh.Dock = DockStyle.Fill;            
            this.panelContenedorForm.Controls.Add(fh);
            this.panelContenedorForm.Tag = fh;
            fh.Show();
            fh = null;
        }
        //METODO PARA MOSTRAR FORMULARIO DE LOGO Al INICIAR ----------------------------------------------------------
        private void MostrarFormLogo()
        {
            AbrirFormEnPanel(new Logo());
        }

        private void FormMenuPrincipal_Load(object sender, EventArgs e)
        {
            MostrarFormLogo();
        }

        //METODO PARA MOSTRAR FORMULARIO DE LOGO Al CERRAR OTROS FORM ----------------------------------------------------------
        private void MostrarFormLogoAlCerrarForms(object sender, FormClosedEventArgs e)
        {
            MostrarFormLogo();
        }

        //METODO PARA HORA Y FECHA ACTUAL ----------------------------------------------------------
        private void tmFechaHora_Tick(object sender, EventArgs e)
        {
            lbFecha.Text = DateTime.Now.ToLongDateString();
            lblHora.Text = DateTime.Now.ToString("HH:mm:ssss");
        }


        #region Botones SubMenu
        private void OcultarSubmenu()
        {
            SubMenuCategorias.Visible = false;
            SubMenuCompras.Visible = false;
            SubMenuProductos.Visible = false;
        }

        private void btnCompras_Click(object sender, EventArgs e)
        {
            MostrarSubmenu(SubMenuCompras);
        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            MostrarSubmenu(SubMenuCategorias);
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            MostrarSubmenu(SubMenuProductos);
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new Logo());
        }

        private void btnAjustes_Click(object sender, EventArgs e)
        {
            PR_Ajustes fm = new PR_Ajustes();
            fm.FormClosed += new FormClosedEventHandler(MostrarFormLogoAlCerrarForms);
            AbrirFormEnPanel(fm);

            fm.txtNit.Text = nit;
            fm.txtNombre.Text = nombre;
            fm.txtDireccion.Text = direccion;
            fm.txtDescripcion.Text = descripcion;
            fm.txtPaginaWeb.Text = web;
            fm.txtCorreo.Text = correo;
            fm.txtCorreo.Text = correo;
            fm = null;
        }

        private void btnCrearCategoria_Click(object sender, EventArgs e)
        {
            PR_CrearCategoria fm = new PR_CrearCategoria();
            fm.FormClosed += new FormClosedEventHandler(MostrarFormLogoAlCerrarForms);
            AbrirFormEnPanel(fm);
            fm = null;
        }

        private void btnCrearProducto_Click(object sender, EventArgs e)
        {
            PR_CrearProducto fm = new PR_CrearProducto();
            fm.FormClosed += new FormClosedEventHandler(MostrarFormLogoAlCerrarForms);
            AbrirFormEnPanel(fm);
            fm = null;
        }

        private void btnEditarProducto_Click(object sender, EventArgs e)
        {
            PR_ListarProducto fm = new PR_ListarProducto();
            fm.FormClosed += new FormClosedEventHandler(MostrarFormLogoAlCerrarForms);
            AbrirFormEnPanel(fm);
            fm = null;
        }

        private void btnEditarCategoria_Click(object sender, EventArgs e)
        {
            PR_ListarCategoria fm = new PR_ListarCategoria();
            fm.FormClosed += new FormClosedEventHandler(MostrarFormLogoAlCerrarForms);
            AbrirFormEnPanel(fm);
            fm = null;
        }

        private void OcultarSubMenuActivo()
        {
            if (SubMenuProductos.Visible)
                SubMenuProductos.Visible = false;
            if (SubMenuCategorias.Visible)
                SubMenuCategorias.Visible = false;
            if (SubMenuCompras.Visible)
                SubMenuCompras.Visible = false;
        }

        private void MostrarSubmenu(Panel subMenu)
        {
            if (panelMenu.Width == 55)
            {
                MessageBox.Show("Abra el menu para acceder a las opciones.");
            }
            else
            {
                if (!subMenu.Visible)
                {
                    subMenu.Visible = true;
                }
                else
                {
                    subMenu.Visible = false;
                }
            }
        }
        #endregion


    }
}
