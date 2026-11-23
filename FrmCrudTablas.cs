using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmCrudTablas : Form
    {
        private readonly GimnasioDAO dao = new GimnasioDAO();
        private ComboBox cmbTabla;
        private TextBox txtC1, txtC2, txtC3, txtC4, txtC5, txtBuscar;
        private Label lblC1, lblC2, lblC3, lblC4, lblC5;
        private DataGridView dgvDatos;
        private int idSeleccionado = 0;

        public FrmCrudTablas(string tablaInicial = "membresias")
        {
            ConstruirInterfaz();
            cmbTabla.SelectedItem = tablaInicial;
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Administración CRUD de Tablas del Sistema";
            this.Size = new Size(920, 560);
            this.StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(new Label { Text = "Seleccionar Tabla:", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), Location = new Point(15, 16), AutoSize = true });
            cmbTabla = new ComboBox { Location = new Point(150, 13), Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTabla.Items.AddRange(new object[] { "membresias", "instructores", "clases", "asistencias", "pagos" });
            cmbTabla.SelectedIndexChanged += (s, e) => ConfigurarCamposTabla();
            Controls.Add(cmbTabla);

            Controls.Add(new Label { Text = "Filtro rápido:", Location = new Point(400, 16), AutoSize = true });
            txtBuscar = new TextBox { Location = new Point(485, 13), Width = 240 };
            Button btnFiltrar = CrearBoton("Buscar", 735, 11, 155, Color.FromArgb(46, 64, 83));
            btnFiltrar.Click += (s, e) => CargarDatos();
            Controls.AddRange(new Control[] { txtBuscar, btnFiltrar });

            GroupBox grp = new GroupBox { Text = "Formulario de Registro / Edición", Location = new Point(15, 48), Size = new Size(875, 145) };

            lblC1 = new Label { Location = new Point(15, 25), AutoSize = true };
            txtC1 = new TextBox { Location = new Point(15, 45), Width = 155 };

            lblC2 = new Label { Location = new Point(185, 25), AutoSize = true };
            txtC2 = new TextBox { Location = new Point(185, 45), Width = 155 };

            lblC3 = new Label { Location = new Point(355, 25), AutoSize = true };
            txtC3 = new TextBox { Location = new Point(355, 45), Width = 155 };

            lblC4 = new Label { Location = new Point(525, 25), AutoSize = true };
            txtC4 = new TextBox { Location = new Point(525, 45), Width = 155 };

            lblC5 = new Label { Location = new Point(695, 25), AutoSize = true };
            txtC5 = new TextBox { Location = new Point(695, 45), Width = 160 };

            Button btnNuevo = CrearBoton("Guardar Nuevo", 410, 95, 110, Color.FromArgb(39, 174, 96));
            Button btnEditar = CrearBoton("Modificar", 530, 95, 100, Color.FromArgb(41, 128, 185));
            Button btnBorrar = CrearBoton("Eliminar", 640, 95, 100, Color.FromArgb(192, 57, 43));
            Button btnLimpiar = CrearBoton("Limpiar", 750, 95, 105, Color.Gray);

            btnNuevo.Click += (s, e) => Guardar(false);
            btnEditar.Click += (s, e) => Guardar(true);
            btnBorrar.Click += (s, e) => Eliminar();
            btnLimpiar.Click += (s, e) => Limpiar();

            grp.Controls.AddRange(new Control[] { lblC1, txtC1, lblC2, txtC2, lblC3, txtC3, lblC4, txtC4, lblC5, txtC5, btnNuevo, btnEditar, btnBorrar, btnLimpiar });
            Controls.Add(grp);

            dgvDatos = new DataGridView
            {
                Location = new Point(15, 205),
                Size = new Size(875, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvDatos.CellClick += DgvDatos_CellClick;
            Controls.Add(dgvDatos);
        }

        private Button CrearBoton(string t, int x, int y, int w, Color c) =>
            new Button { Text = t, Location = new Point(x, y), Size = new Size(w, 30), BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        private void ConfigurarCamposTabla()
        {
            Limpiar();
            string t = cmbTabla.Text;
            lblC5.Visible = txtC5.Visible = (t == "instructores");

            switch (t)
            {
                case "membresias":
                    lblC1.Text = "Nombre Plan:"; lblC2.Text = "Duración (días):"; lblC3.Text = "Precio ($):"; lblC4.Text = "Estatus (Activa/Inactiva):";
                    break;
                case "instructores":
                    lblC1.Text = "Nombre:"; lblC2.Text = "Apellidos:"; lblC3.Text = "Especialidad:"; lblC4.Text = "Teléfono:"; lblC5.Text = "Salario ($):";
                    break;
                case "clases":
                    lblC1.Text = "Nombre Clase:"; lblC2.Text = "Horario:"; lblC3.Text = "Cupo Máximo:"; lblC4.Text = "ID Instructor:";
                    break;
                case "asistencias":
                    lblC1.Text = "ID Socio:"; lblC2.Text = "ID Clase:"; lblC3.Text = "Calificación (1-5):"; lblC4.Text = "Fecha (YYYY-MM-DD):";
                    break;
                case "pagos":
                    lblC1.Text = "ID Socio:"; lblC2.Text = "ID Membresía:"; lblC3.Text = "Monto ($):"; lblC4.Text = "Método (Efectivo/Tarjeta):";
                    break;
            }
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                string t = cmbTabla.Text;
                string f = $"%{txtBuscar.Text.Trim()}%";
                string sql = t switch
                {
                    "membresias" => "SELECT * FROM membresias WHERE nombre_plan LIKE @f OR estatus LIKE @f;",
                    "instructores" => "SELECT * FROM instructores WHERE nombre LIKE @f OR apellidos LIKE @f OR especialidad LIKE @f;",
                    "clases" => "SELECT * FROM clases WHERE nombre_clase LIKE @f OR horario LIKE @f;",
                    "asistencias" => "SELECT * FROM asistencias WHERE CAST(id_socio AS CHAR) LIKE @f OR CAST(id_clase AS CHAR) LIKE @f;",
                    _ => "SELECT * FROM pagos WHERE metodo_pago LIKE @f OR CAST(id_socio AS CHAR) LIKE @f;"
                };
                dgvDatos.DataSource = dao.EjecutarConsultaTabla(sql, new MySqlParameter("@f", f));
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Guardar(bool esEdicion)
        {
            if (esEdicion && idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtC1.Text) || string.IsNullOrWhiteSpace(txtC2.Text) || string.IsNullOrWhiteSpace(txtC3.Text))
            {
                MessageBox.Show("Complete los campos requeridos antes de guardar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string t = cmbTabla.Text;
                if (t == "membresias")
                {
                    string sql = esEdicion
                        ? "UPDATE membresias SET nombre_plan=@c1, duracion_dias=@c2, precio=@c3, estatus=@c4 WHERE id_membresia=@id;"
                        : "INSERT INTO membresias (nombre_plan, duracion_dias, precio, estatus) VALUES (@c1, @c2, @c3, @c4);";
                    dao.EjecutarComando(sql, new MySqlParameter("@c1", txtC1.Text), new MySqlParameter("@c2", int.Parse(txtC2.Text)),
                        new MySqlParameter("@c3", decimal.Parse(txtC3.Text)), new MySqlParameter("@c4", string.IsNullOrWhiteSpace(txtC4.Text) ? "Activa" : txtC4.Text),
                        new MySqlParameter("@id", idSeleccionado));
                }
                else if (t == "instructores")
                {
                    string sql = esEdicion
                        ? "UPDATE instructores SET nombre=@c1, apellidos=@c2, especialidad=@c3, telefono=@c4, salario=@c5 WHERE id_instructor=@id;"
                        : "INSERT INTO instructores (nombre, apellidos, especialidad, telefono, salario) VALUES (@c1, @c2, @c3, @c4, @c5);";
                    dao.EjecutarComando(sql, new MySqlParameter("@c1", txtC1.Text), new MySqlParameter("@c2", txtC2.Text),
                        new MySqlParameter("@c3", txtC3.Text), new MySqlParameter("@c4", txtC4.Text),
                        new MySqlParameter("@c5", decimal.Parse(txtC5.Text)), new MySqlParameter("@id", idSeleccionado));
                }
                else if (t == "clases")
                {
                    string sql = esEdicion
                        ? "UPDATE clases SET nombre_clase=@c1, horario=@c2, cupo_maximo=@c3, id_instructor=@c4 WHERE id_clase=@id;"
                        : "INSERT INTO clases (nombre_clase, horario, cupo_maximo, id_instructor) VALUES (@c1, @c2, @c3, @c4);";
                    dao.EjecutarComando(sql, new MySqlParameter("@c1", txtC1.Text), new MySqlParameter("@c2", txtC2.Text),
                        new MySqlParameter("@c3", int.Parse(txtC3.Text)), new MySqlParameter("@c4", int.Parse(txtC4.Text)),
                        new MySqlParameter("@id", idSeleccionado));
                }
                else if (t == "asistencias")
                {
                    string sql = esEdicion
                        ? "UPDATE asistencias SET id_socio=@c1, id_clase=@c2, calificacion_clase=@c3 WHERE id_asistencia=@id;"
                        : "INSERT INTO asistencias (id_socio, id_clase, calificacion_clase) VALUES (@c1, @c2, @c3);";
                    dao.EjecutarComando(sql, new MySqlParameter("@c1", int.Parse(txtC1.Text)), new MySqlParameter("@c2", int.Parse(txtC2.Text)),
                        new MySqlParameter("@c3", int.Parse(txtC3.Text)), new MySqlParameter("@id", idSeleccionado));
                }
                else if (t == "pagos")
                {
                    string sql = esEdicion
                        ? "UPDATE pagos SET id_socio=@c1, id_membresia=@c2, monto=@c3, metodo_pago=@c4 WHERE id_pago=@id;"
                        : "INSERT INTO pagos (id_socio, id_membresia, monto, metodo_pago) VALUES (@c1, @c2, @c3, @c4);";
                    dao.EjecutarComando(sql, new MySqlParameter("@c1", int.Parse(txtC1.Text)), new MySqlParameter("@c2", int.Parse(txtC2.Text)),
                        new MySqlParameter("@c3", decimal.Parse(txtC3.Text)), new MySqlParameter("@c4", string.IsNullOrWhiteSpace(txtC4.Text) ? "Efectivo" : txtC4.Text),
                        new MySqlParameter("@id", idSeleccionado));
                }

                MessageBox.Show("Operación realizada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarDatos();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Verifique que los campos numéricos (ID, días, precio, salario, cupo) contengan números válidos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Eliminar()
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un registro primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string t = cmbTabla.Text;
            string colId = t switch
            {
                "membresias" => "id_membresia",
                "instructores" => "id_instructor",
                "clases" => "id_clase",
                "asistencias" => "id_asistencia",
                _ => "id_pago"
            };

            if (MessageBox.Show("¿Desea eliminar el registro seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    dao.EjecutarComando($"DELETE FROM {t} WHERE {colId} = @id;", new MySqlParameter("@id", idSeleccionado));
                    MessageBox.Show("Registro eliminado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar();
                    CargarDatos();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error al Eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DgvDatos_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow f = dgvDatos.Rows[e.RowIndex];
                idSeleccionado = Convert.ToInt32(f.Cells[0].Value);
                txtC1.Text = f.Cells[1].Value?.ToString();
                txtC2.Text = f.Cells[2].Value?.ToString();
                txtC3.Text = f.Cells[3].Value?.ToString();
                txtC4.Text = f.Cells[4].Value?.ToString();
                if (f.Cells.Count > 5) txtC5.Text = f.Cells[5].Value?.ToString();
            }
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtC1.Clear(); txtC2.Clear(); txtC3.Clear(); txtC4.Clear(); txtC5.Clear();
        }
    }
}