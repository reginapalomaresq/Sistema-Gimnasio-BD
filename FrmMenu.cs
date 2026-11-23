using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmMenu : Form
    {
        public FrmMenu()
        {
            ConstruirMenu();
        }

        private void ConstruirMenu()
        {
            this.Text = $"Menú Principal - Perfil: {ConexionDB.PerfilActual}";
            this.Size = new Size(520, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblBienvenida = new Label
            {
                Text = $"Sesión activa: {ConexionDB.PerfilActual} ({ConexionDB.UsuarioActual})",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };

            // Botones de módulos
            Button btnSocios = CrearBoton("Gestión de Socios (CRUD y Filtros)", 65);
            Button btnPagos = CrearBoton("Cobro de Membresías (Transacción)", 115);
            Button btnAsistencias = CrearBoton("Registro de Asistencias y Clases", 165);
            Button btnReportes = CrearBoton("Reportes, Vistas y Consultas", 215);
            Button btnCatalogos = CrearBoton("Instructores y Membresías (Solo Admin)", 265);
            Button btnPruebaPermiso = CrearBoton("Probar Acción Restringida (Demo Seguridad)", 315);
            btnPruebaPermiso.BackColor = Color.FromArgb(176, 58, 46);

            // Control de visibilidad según el rol (Requisito 2)
            string perfil = ConexionDB.PerfilActual;
            if (perfil == "Recepción")
            {
                btnCatalogos.Visible = false; // Oculto para recepción
            }
            else if (perfil == "Instructor")
            {
                btnSocios.Visible = false;
                btnPagos.Visible = false;
                btnCatalogos.Visible = false;
            }

            // Evento de demostración de error de permisos (Competencia 3)
            btnPruebaPermiso.Click += (s, e) =>
            {
                try
                {
                    using (MySqlConnection conn = ConexionDB.ObtenerConexion())
                    {
                        conn.Open();
                        // Intenta borrar un registro para evidenciar restricción de permisos en MySQL
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM socios WHERE id_socio = -1;", conn))
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Tu perfil SÍ tiene permiso de eliminación en la BD.", "Permiso Validado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Control de Acceso Activo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnSocios.Click += (s, e) => new FrmSocios().ShowDialog();
            btnPagos.Click += (s, e) => new FrmPagos().ShowDialog();
            btnAsistencias.Click += (s, e) => new FrmCrudTablas("asistencias").ShowDialog();
            btnReportes.Click += (s, e) => new FrmReportes().ShowDialog();
            btnCatalogos.Click += (s, e) => new FrmCrudTablas("membresias").ShowDialog();

            this.Controls.AddRange(new Control[] { lblBienvenida, btnSocios, btnPagos, btnAsistencias, btnReportes, btnCatalogos, btnPruebaPermiso });
        }

        private Button CrearBoton(string texto, int top)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(30, top),
                Width = 440,
                Height = 38,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                BackColor = Color.FromArgb(46, 64, 83),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}
