using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmLogin : Form
    {
        private ComboBox cmbPerfil;
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnIngresar;

        public FrmLogin()
        {
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Inicio de Sesión - Sistema Gimnasio";
            this.Size = new Size(490, 390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "Acceso al Sistema del Gimnasio",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(40, 22),
                AutoSize = true
            };

            Label lblPerfil = new Label { Text = "Perfil de acceso:", Location = new Point(40, 75), AutoSize = true };
            cmbPerfil = new ComboBox
            {
                Location = new Point(40, 100),
                Width = 390,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPerfil.Items.AddRange(new object[] { "Administrador", "Recepción", "Instructor" });
            cmbPerfil.SelectedIndexChanged += CmbPerfil_SelectedIndexChanged;

            Label lblUsuario = new Label { Text = "Usuario de Base de Datos:", Location = new Point(40, 145), AutoSize = true };
            txtUsuario = new TextBox { Location = new Point(40, 170), Width = 390, ReadOnly = true };

            Label lblPass = new Label { Text = "Contraseña:", Location = new Point(40, 212), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(40, 237), Width = 390, UseSystemPasswordChar = true };

            btnIngresar = new Button
            {
                Text = "Conectar e Ingresar",
                Location = new Point(40, 285),
                Width = 390,
                Height = 40,
                BackColor = Color.FromArgb(40, 116, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnIngresar.Click += BtnIngresar_Click;

            this.Controls.AddRange(new Control[] { lblTitulo, lblPerfil, cmbPerfil, lblUsuario, txtUsuario, lblPass, txtPassword, btnIngresar });

            cmbPerfil.SelectedIndex = 0;
        }

        private void CmbPerfil_SelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (cmbPerfil.SelectedItem?.ToString())
            {
                case "Administrador":
                    txtUsuario.Text = "usr_admin";
                    txtPassword.Text = "AdminGym2026!";
                    break;
                case "Recepción":
                    txtUsuario.Text = "usr_recepcion";
                    txtPassword.Text = "RecepGym2026!";
                    break;
                case "Instructor":
                    txtUsuario.Text = "usr_instructor";
                    txtPassword.Text = "InstGym2026!";
                    break;
            }
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Por favor complete usuario y contraseña.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConexionDB.PerfilActual = cmbPerfil.SelectedItem?.ToString() ?? "";
            ConexionDB.UsuarioActual = txtUsuario.Text.Trim();
            ConexionDB.PasswordActual = txtPassword.Text;

            try
            {
                using (MySqlConnection conn = ConexionDB.ObtenerConexion())
                {
                    conn.Open();
                    MessageBox.Show($"Conexión exitosa como {ConexionDB.PerfilActual} ({ConexionDB.UsuarioActual}).",
                        "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FrmMenu menu = new FrmMenu();
                    this.Hide();
                    menu.ShowDialog();
                    this.Show();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error de Acceso a BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
