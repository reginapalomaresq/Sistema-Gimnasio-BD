using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmSocios : Form
    {
        private readonly GimnasioDAO dao = new GimnasioDAO();
        private int idSocioSeleccionado = 0;

        private TextBox txtNombre, txtApellidos, txtEmail, txtTelefono, txtBuscar;
        private ComboBox cmbMembresia, cmbEstatus;
        private DateTimePicker dtpInicio, dtpFin;
        private DataGridView dgvSocios;

        public FrmSocios()
        {
            ConstruirInterfaz();
            CargarMembresias();
            CargarTabla();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Gestión de Socios (CRUD y Filtros)";
            this.Size = new Size(920, 580);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Panel superior de captura (CRUD) ---
            GroupBox grpDatos = new GroupBox { Text = "Datos del Socio", Location = new Point(15, 10), Size = new Size(875, 165) };

            grpDatos.Controls.Add(new Label { Text = "Nombre:", Location = new Point(15, 28), AutoSize = true });
            txtNombre = new TextBox { Location = new Point(15, 48), Width = 180 };

            grpDatos.Controls.Add(new Label { Text = "Apellidos:", Location = new Point(210, 28), AutoSize = true });
            txtApellidos = new TextBox { Location = new Point(210, 48), Width = 200 };

            grpDatos.Controls.Add(new Label { Text = "Correo (Único):", Location = new Point(425, 28), AutoSize = true });
            txtEmail = new TextBox { Location = new Point(425, 48), Width = 220 };

            grpDatos.Controls.Add(new Label { Text = "Teléfono:", Location = new Point(660, 28), AutoSize = true });
            txtTelefono = new TextBox { Location = new Point(660, 48), Width = 190 };

            grpDatos.Controls.Add(new Label { Text = "Plan de Membresía:", Location = new Point(15, 85), AutoSize = true });
            cmbMembresia = new ComboBox { Location = new Point(15, 105), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };

            grpDatos.Controls.Add(new Label { Text = "Estatus:", Location = new Point(250, 85), AutoSize = true });
            cmbEstatus = new ComboBox { Location = new Point(250, 105), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEstatus.Items.AddRange(new object[] { "Activo", "Suspendido" });
            cmbEstatus.SelectedIndex = 0;

            Button btnGuardar = CrearBoton("Guardar Nuevo", 410, 102, 110, Color.FromArgb(39, 174, 96));
            Button btnEditar = CrearBoton("Modificar", 530, 102, 100, Color.FromArgb(41, 128, 185));
            Button btnEliminar = CrearBoton("Eliminar", 640, 102, 100, Color.FromArgb(192, 57, 43));
            Button btnLimpiar = CrearBoton("Limpiar", 750, 102, 100, Color.Gray);

            btnGuardar.Click += BtnGuardar_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += (s, e) => LimpiarCampos();

            grpDatos.Controls.AddRange(new Control[] { txtNombre, txtApellidos, txtEmail, txtTelefono, cmbMembresia, cmbEstatus, btnGuardar, btnEditar, btnEliminar, btnLimpiar });

            // --- Panel de filtros (Requisito 4: texto + rango fechas) ---
            GroupBox grpFiltros = new GroupBox { Text = "Búsqueda y Filtros (LIKE + BETWEEN)", Location = new Point(15, 185), Size = new Size(875, 65) };

            grpFiltros.Controls.Add(new Label { Text = "Nombre / Correo:", Location = new Point(15, 28), AutoSize = true });
            txtBuscar = new TextBox { Location = new Point(120, 25), Width = 210 };

            grpFiltros.Controls.Add(new Label { Text = "Desde:", Location = new Point(350, 28), AutoSize = true });
            dtpInicio = new DateTimePicker { Location = new Point(398, 25), Width = 125, Format = DateTimePickerFormat.Short, Value = new DateTime(2026, 1, 1) };

            grpFiltros.Controls.Add(new Label { Text = "Hasta:", Location = new Point(540, 28), AutoSize = true });
            dtpFin = new DateTimePicker { Location = new Point(585, 25), Width = 125, Format = DateTimePickerFormat.Short, Value = new DateTime(2026, 12, 31) };

            Button btnFiltrar = CrearBoton("Filtrar", 730, 22, 120, Color.FromArgb(46, 64, 83));
            btnFiltrar.Click += (s, e) => CargarTabla();

            grpFiltros.Controls.AddRange(new Control[] { txtBuscar, dtpInicio, dtpFin, btnFiltrar });

            // --- Tabla DataGridView ---
            dgvSocios = new DataGridView
            {
                Location = new Point(15, 260),
                Size = new Size(875, 265),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvSocios.CellClick += DgvSocios_CellClick;

            this.Controls.AddRange(new Control[] { grpDatos, grpFiltros, dgvSocios });
        }

        private Button CrearBoton(string texto, int x, int y, int ancho, Color color)
        {
            return new Button { Text = texto, Location = new Point(x, y), Size = new Size(ancho, 30), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        }

        private void CargarMembresias()
        {
            try
            {
                cmbMembresia.DataSource = dao.ObtenerMembresiasActivas();
                cmbMembresia.DisplayMember = "nombre_plan";
                cmbMembresia.ValueMember = "id_membresia";
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTabla()
        {
            try
            {
                dgvSocios.DataSource = dao.BuscarSocios(txtBuscar.Text.Trim(), dtpInicio.Value, dtpFin.Value);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Todos los campos del socio son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Ingrese un correo electrónico válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!ValidarCampos()) return;
            try
            {
                int idPlan = Convert.ToInt32(cmbMembresia.SelectedValue);
                dao.InsertarSocio(txtNombre.Text.Trim(), txtApellidos.Text.Trim(), txtEmail.Text.Trim(), txtTelefono.Text.Trim(), idPlan);
                MessageBox.Show("Socio registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarTabla();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error al Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (idSocioSeleccionado == 0)
            {
                MessageBox.Show("Seleccione primero un socio de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidarCampos()) return;
            try
            {
                int idPlan = Convert.ToInt32(cmbMembresia.SelectedValue);
                dao.ActualizarSocio(idSocioSeleccionado, txtNombre.Text.Trim(), txtApellidos.Text.Trim(), txtEmail.Text.Trim(), txtTelefono.Text.Trim(), cmbEstatus.Text, idPlan);
                MessageBox.Show("Socio actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarTabla();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error al Actualizar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (idSocioSeleccionado == 0)
            {
                MessageBox.Show("Seleccione primero un socio de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("¿Seguro que desea eliminar este socio?", "Confirmar baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    dao.EliminarSocio(idSocioSeleccionado);
                    MessageBox.Show("Socio eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarTabla();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Restricción de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DgvSocios_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvSocios.Rows[e.RowIndex];
                idSocioSeleccionado = Convert.ToInt32(fila.Cells["ID"].Value);
                txtNombre.Text = fila.Cells["Nombre"].Value.ToString();
                txtApellidos.Text = fila.Cells["Apellidos"].Value.ToString();
                txtEmail.Text = fila.Cells["Correo"].Value.ToString();
                txtTelefono.Text = fila.Cells["Teléfono"].Value.ToString();
                cmbEstatus.Text = fila.Cells["Estatus"].Value.ToString();
                cmbMembresia.SelectedValue = Convert.ToInt32(fila.Cells["ID_Plan"].Value);
            }
        }

        private void LimpiarCampos()
        {
            idSocioSeleccionado = 0;
            txtNombre.Clear();
            txtApellidos.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            cmbEstatus.SelectedIndex = 0;
        }
    }
}