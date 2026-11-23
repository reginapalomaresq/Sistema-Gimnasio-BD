using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmPagos : Form
    {
        private readonly GimnasioDAO dao = new GimnasioDAO();
        private ComboBox cmbSocio, cmbMembresia, cmbMetodo;
        private TextBox txtMonto;
        private CheckBox chkSimularError;
        private DataGridView dgvPagos;

        public FrmPagos()
        {
            ConstruirInterfaz();
            CargarCombos();
            CargarHistorial();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Cobro de Membresías (Operación Transaccional)";
            this.Size = new Size(780, 520);
            this.StartPosition = FormStartPosition.CenterScreen;

            GroupBox grpCobro = new GroupBox { Text = "Registrar Pago y Activar Membresía (MySqlTransaction)", Location = new Point(15, 10), Size = new Size(735, 175) };

            grpCobro.Controls.Add(new Label { Text = "Socio:", Location = new Point(15, 28), AutoSize = true });
            cmbSocio = new ComboBox { Location = new Point(15, 48), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            grpCobro.Controls.Add(new Label { Text = "Membresía a pagar:", Location = new Point(280, 28), AutoSize = true });
            cmbMembresia = new ComboBox { Location = new Point(280, 48), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMembresia.SelectedIndexChanged += (s, e) =>
            {
                if (cmbMembresia.SelectedItem is DataRowView row)
                    txtMonto.Text = row["precio"].ToString();
            };

            grpCobro.Controls.Add(new Label { Text = "Monto ($):", Location = new Point(525, 28), AutoSize = true });
            txtMonto = new TextBox { Location = new Point(525, 48), Width = 185 };

            grpCobro.Controls.Add(new Label { Text = "Método de pago:", Location = new Point(15, 88), AutoSize = true });
            cmbMetodo = new ComboBox { Location = new Point(15, 108), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMetodo.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "Transferencia" });
            cmbMetodo.SelectedIndex = 0;

            chkSimularError = new CheckBox
            {
                Text = "Simular fallo antes del COMMIT (Demostrar ROLLBACK)",
                Location = new Point(215, 110),
                AutoSize = true,
                ForeColor = Color.DarkRed
            };

            Button btnCobrar = new Button
            {
                Text = "Ejecutar Transacción",
                Location = new Point(525, 103),
                Size = new Size(185, 34),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCobrar.Click += BtnCobrar_Click;

            grpCobro.Controls.AddRange(new Control[] { cmbSocio, cmbMembresia, txtMonto, cmbMetodo, chkSimularError, btnCobrar });

            dgvPagos = new DataGridView
            {
                Location = new Point(15, 200),
                Size = new Size(735, 265),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.AddRange(new Control[] { grpCobro, dgvPagos });
        }

        private void CargarCombos()
        {
            try
            {
                cmbSocio.DataSource = dao.EjecutarConsultaTabla("SELECT id_socio, CONCAT(nombre, ' ', apellidos, ' (', estatus, ')') AS info FROM socios ORDER BY nombre;");
                cmbSocio.DisplayMember = "info";
                cmbSocio.ValueMember = "id_socio";

                cmbMembresia.DataSource = dao.ObtenerMembresiasActivas();
                cmbMembresia.DisplayMember = "nombre_plan";
                cmbMembresia.ValueMember = "id_membresia";
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarHistorial()
        {
            try
            {
                dgvPagos.DataSource = dao.EjecutarConsultaTabla("SELECT * FROM vw_historial_pagos ORDER BY id_pago DESC;");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCobrar_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtMonto.Text, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Ingrese un monto numérico mayor a 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSocio = Convert.ToInt32(cmbSocio.SelectedValue);
            int idMembresia = Convert.ToInt32(cmbMembresia.SelectedValue);

            try
            {
                int idPago = dao.RegistrarPagoConTransaccion(idSocio, idMembresia, monto, cmbMetodo.Text, chkSimularError.Checked);
                MessageBox.Show($"TRANSACCIÓN CONFIRMADA (COMMIT).\n\nPago #{idPago} registrado y membresía del socio actualizada a 'Activo'.",
                    "Commit Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCombos();
                CargarHistorial();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"TRANSACCIÓN REVERTIDA (ROLLBACK).\n\n{ConexionDB.TraducirErrorMySql(ex)}",
                    "Rollback Ejecutado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"TRANSACCIÓN REVERTIDA (ROLLBACK).\n\nMotivo: {ex.Message}\nNinguna tabla fue modificada.",
                    "Rollback Ejecutado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}