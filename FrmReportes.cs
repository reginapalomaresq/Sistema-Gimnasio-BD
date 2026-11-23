using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SistemaGimnasioApp.DAO;

namespace SistemaGimnasioApp
{
    public partial class FrmReportes : Form
    {
        private readonly GimnasioDAO dao = new GimnasioDAO();
        private DataGridView dgvReportes;
        private Label lblDescripcion;

        public FrmReportes()
        {
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            this.Text = "Reportes, Vistas, Subconsultas y Operadores Set";
            this.Size = new Size(920, 540);
            this.StartPosition = FormStartPosition.CenterScreen;

            FlowLayoutPanel panelBotones = new FlowLayoutPanel
            {
                Location = new Point(15, 12),
                Size = new Size(875, 85),
                WrapContents = true
            };

            panelBotones.Controls.Add(CrearBtn("Vista 1: Socios y Planes", () =>
                Ejecutar("Vista vw_socios_membresias (INNER JOIN)", "SELECT * FROM vw_socios_membresias;")));

            panelBotones.Controls.Add(CrearBtn("Vista 2: Historial Pagos", () =>
                Ejecutar("Vista vw_historial_pagos (Múltiple JOIN)", "SELECT * FROM vw_historial_pagos ORDER BY fecha_pago DESC;")));

            panelBotones.Controls.Add(CrearBtn("Vista 3: Ocupación Clases", () =>
                Ejecutar("Vista vw_ocupacion_clases (LEFT JOIN + Agregados)", "SELECT * FROM vw_ocupacion_clases;")));

            panelBotones.Controls.Add(CrearBtn("Agrupamiento (GROUP BY + HAVING)", () =>
                Ejecutar("Ingresos por Plan con HAVING > 1000 y CAST",
                @"SELECT UPPER(m.nombre_plan) AS Plan, COUNT(p.id_pago) AS Total_Pagos,
                         CAST(SUM(p.monto) AS DECIMAL(10,2)) AS Ingresos_Totales,
                         ROUND(AVG(p.monto), 2) AS Promedio, YEAR(p.fecha_pago) AS Año
                  FROM pagos p
                  INNER JOIN membresias m ON p.id_membresia = m.id_membresia
                  GROUP BY m.nombre_plan, YEAR(p.fecha_pago)
                  HAVING SUM(p.monto) > 1000
                  ORDER BY Ingresos_Totales DESC;")));

            panelBotones.Controls.Add(CrearBtn("Subconsulta WHERE", () =>
                Ejecutar("Socios con pagos superiores al promedio general",
                @"SELECT id_socio AS ID, nombre AS Nombre, apellidos AS Apellidos, email AS Correo
                  FROM socios
                  WHERE id_socio IN (SELECT id_socio FROM pagos WHERE monto > (SELECT AVG(monto) FROM pagos));")));

            panelBotones.Controls.Add(CrearBtn("Subconsulta Correlacionada + Funciones", () =>
                Ejecutar("Antigüedad, inversión total (Funciones) y asistencias (Subconsulta correlacionada)",
                @"SELECT s.id_socio AS ID, CONCAT(s.nombre, ' ', s.apellidos) AS Socio,
                         fn_antiguedad_dias(s.id_socio) AS Dias_Inscrito,
                         fn_total_pagado_socio(s.id_socio) AS Total_Pagado,
                         (SELECT COUNT(*) FROM asistencias a WHERE a.id_socio = s.id_socio) AS Asistencias
                  FROM socios s ORDER BY Asistencias DESC;")));

            panelBotones.Controls.Add(CrearBtn("Operador Set (UNION)", () =>
                Ejecutar("Directorio Unificado de Socios e Instructores (UNION)",
                @"SELECT CONCAT(nombre, ' ', apellidos) AS Nombre_Completo, telefono AS Telefono, 'Socio' AS Rol
                  FROM socios
                  UNION
                  SELECT CONCAT(nombre, ' ', apellidos) AS Nombre_Completo, telefono AS Telefono, 'Instructor' AS Rol
                  FROM instructores
                  ORDER BY Rol, Nombre_Completo;")));

            lblDescripcion = new Label
            {
                Text = "Seleccione un reporte arriba para consultar la base de datos.",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(15, 105),
                AutoSize = true
            };

            dgvReportes = new DataGridView
            {
                Location = new Point(15, 132),
                Size = new Size(875, 355),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.AddRange(new Control[] { panelBotones, lblDescripcion, dgvReportes });
        }

        private Button CrearBtn(string texto, Action accion)
        {
            Button btn = new Button
            {
                Text = texto,
                Size = new Size(210, 34),
                BackColor = Color.FromArgb(46, 64, 83),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3)
            };
            btn.Click += (s, e) => accion();
            return btn;
        }

        private void Ejecutar(string titulo, string sql)
        {
            try
            {
                lblDescripcion.Text = $"Mostrando: {titulo}";
                dgvReportes.DataSource = dao.EjecutarConsultaTabla(sql);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ConexionDB.TraducirErrorMySql(ex), "Error en Reporte", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}