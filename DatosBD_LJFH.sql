USE gimnasio_db;

-- 1. MEMBRESÍAS (15 registros)
INSERT INTO membresias (nombre_plan, duracion_dias, precio, estatus) VALUES
('Básico Mensual', 30, 450.00, 'Activa'),
('Plus Mensual', 30, 650.00, 'Activa'),
('VIP Todo Incluido', 30, 950.00, 'Activa'),
('Estudiante Mensual', 30, 350.00, 'Activa'),
('Trimestral Básico', 90, 1200.00, 'Activa'),
('Trimestral VIP', 90, 2500.00, 'Activa'),
('Semestral Básico', 180, 2200.00, 'Activa'),
('Semestral VIP', 180, 4600.00, 'Activa'),
('Anual Básico', 365, 4000.00, 'Activa'),
('Anual VIP', 365, 8500.00, 'Activa'),
('Pase Semanal', 7, 180.00, 'Activa'),
('Pase Diario', 1, 60.00, 'Activa'),
('Plan Pareja Mensual', 30, 1100.00, 'Activa'),
('Plan Adulto Mayor', 30, 300.00, 'Activa'),
('Verano Promo 2025', 30, 399.00, 'Inactiva');

-- 2. INSTRUCTORES (15 registros)
INSERT INTO instructores (nombre, apellidos, especialidad, telefono, salario) VALUES
('Roberto', 'Mendoza Ruiz', 'Pesas y Hipertrofia', '6441000001', 9500.00),
('Laura', 'Gómez Castro', 'Yoga y Pilates', '6441000002', 8800.00),
('Carlos', 'Valdez Soto', 'CrossFit', '6441000003', 10500.00),
('Sofía', 'Ramírez Ortiz', 'Spinning', '6441000004', 8200.00),
('Diego', 'Navarro Félix', 'Boxeo y Funcional', '6441000005', 9200.00),
('Valeria', 'Torres Morales', 'Zumba y Baile Fitness', '6441000006', 7800.00),
('Andrés', 'Salazar Luna', 'Calistenia', '6441000007', 8500.00),
('Fernanda', 'Ríos Herrera', 'GAP y Tonificación', '6441000008', 8000.00),
('Jorge', 'Castillo Vega', 'Powerlifting', '6441000009', 11000.00),
('Mariana', 'Duarte León', 'Stretching y Movilidad', '6441000010', 7500.00),
('Héctor', 'Paredes Cruz', 'Entrenamiento HIIT', '6441000011', 9000.00),
('Paola', 'Ibarra Flores', 'Aeróbicos', '6441000012', 7600.00),
('Luis', 'Esquivel Domínguez', 'Artes Marciales Mixtas', '6441000013', 10200.00),
('Camila', 'Serrano Gil', 'Barre', '6441000014', 8400.00),
('Oscar', 'Quintero Blanco', 'Acondicionamiento General', '6441000015', 7900.00);

-- 3. SOCIOS (15 registros)
INSERT INTO socios (nombre, apellidos, email, telefono, fecha_registro, estatus, id_membresia) VALUES
('Carlos', 'Pérez López', 'carlos.perez@email.com', '6442000001', '2026-01-10', 'Activo', 3),
('Ana', 'Martínez Ruiz', 'ana.martinez@email.com', '6442000002', '2026-01-15', 'Activo', 2),
('Luis', 'Hernández Gómez', 'luis.hernandez@email.com', '6442000003', '2026-02-01', 'Activo', 1),
('María', 'González Díaz', 'maria.gonzalez@email.com', '6442000004', '2026-02-12', 'Activo', 4),
('José', 'Rodríguez Sánchez', 'jose.rodriguez@email.com', '6442000005', '2026-02-20', 'Activo', 6),
('Elena', 'Ramírez Torres', 'elena.ramirez@email.com', '6442000006', '2026-03-05', 'Activo', 3),
('Miguel', 'Flores Vargas', 'miguel.flores@email.com', '6442000007', '2026-03-14', 'Suspendido', 1),
('Lucía', 'Castro Morales', 'lucia.castro@email.com', '6442000008', '2026-03-22', 'Activo', 5),
('David', 'Ortiz Jiménez', 'david.ortiz@email.com', '6442000009', '2026-04-02', 'Activo', 10),
('Sofía', 'Silva Mendoza', 'sofia.silva@email.com', '6442000010', '2026-04-18', 'Activo', 2),
('Javier', 'Reyes Romero', 'javier.reyes@email.com', '6442000011', '2026-05-03', 'Activo', 8),
('Daniela', 'Cruz Navarro', 'daniela.cruz@email.com', '6442000012', '2026-05-19', 'Activo', 4),
('Alejandro', 'Morales Peña', 'alejandro.morales@email.com', '6442000013', '2026-06-07', 'Suspendido', 1),
('Paula', 'Vargas Ríos', 'paula.vargas@email.com', '6442000014', '2026-06-25', 'Activo', 3),
('Fernando', 'Pérez Aguirre', 'fernando.perez@email.com', '6442000015', '2026-07-11', 'Activo', 2);

-- 4. CLASES (15 registros)
INSERT INTO clases (nombre_clase, horario, cupo_maximo, id_instructor) VALUES
('CrossFit Matutino', 'Lunes a Viernes 07:00 - 08:00', 20, 3),
('Yoga Restaurativo', 'Lunes y Miércoles 08:30 - 09:30', 15, 2),
('Spinning Extremo', 'Martes y Jueves 18:00 - 19:00', 25, 4),
('Boxeo Funcional', 'Lunes, Miércoles y Viernes 19:00 - 20:00', 18, 5),
('Zumba Fiesta', 'Martes y Jueves 17:00 - 18:00', 30, 6),
('Calistenia Básica', 'Sábados 09:00 - 10:30', 15, 7),
('GAP Intensivo', 'Lunes y Miércoles 17:00 - 18:00', 20, 8),
('Club de Powerlifting', 'Viernes 17:00 - 19:00', 12, 9),
('Movilidad y Postura', 'Sábados 11:00 - 12:00', 15, 10),
('HIIT Quema Grasa', 'Martes y Jueves 07:00 - 08:00', 22, 11),
('Aeróbicos Clásicos', 'Lunes y Miércoles 10:00 - 11:00', 25, 12),
('Defensa Personal MMA', 'Martes y Jueves 20:00 - 21:30', 16, 13),
('Barre Sculpt', 'Viernes 09:00 - 10:00', 14, 14),
('Circuito Full Body', 'Sábados 08:00 - 09:00', 20, 15),
('Hipertrofia Guiada', 'Lunes a Jueves 16:00 - 17:00', 15, 1);

-- 5. PAGOS (15 registros)
INSERT INTO pagos (id_socio, id_membresia, monto, fecha_pago, metodo_pago) VALUES
(1, 3, 950.00, '2026-01-10 09:15:00', 'Tarjeta'),
(2, 2, 650.00, '2026-01-15 10:30:00', 'Transferencia'),
(3, 1, 450.00, '2026-02-01 11:00:00', 'Efectivo'),
(4, 4, 350.00, '2026-02-12 16:45:00', 'Efectivo'),
(5, 6, 2500.00, '2026-02-20 18:20:00', 'Tarjeta'),
(6, 3, 950.00, '2026-03-05 08:50:00', 'Tarjeta'),
(7, 1, 450.00, '2026-03-14 14:10:00', 'Efectivo'),
(8, 5, 1200.00, '2026-03-22 12:00:00', 'Transferencia'),
(9, 10, 8500.00, '2026-04-02 17:30:00', 'Tarjeta'),
(10, 2, 650.00, '2026-04-18 19:05:00', 'Transferencia'),
(11, 8, 4600.00, '2026-05-03 10:15:00', 'Tarjeta'),
(12, 4, 350.00, '2026-05-19 15:40:00', 'Efectivo'),
(13, 1, 450.00, '2026-06-07 11:25:00', 'Efectivo'),
(14, 3, 950.00, '2026-06-25 18:00:00', 'Tarjeta'),
(15, 2, 650.00, '2026-07-11 09:45:00', 'Transferencia');

-- 6. ASISTENCIAS (15 registros)
INSERT INTO asistencias (id_socio, id_clase, fecha_asistencia, calificacion_clase) VALUES
(1, 1, '2026-08-01 07:02:00', 5),
(2, 2, '2026-08-01 08:28:00', 5),
(3, 4, '2026-08-01 18:55:00', 4),
(4, 5, '2026-08-02 17:00:00', 5),
(5, 3, '2026-08-02 17:58:00', 4),
(6, 1, '2026-08-03 07:00:00', 5),
(8, 7, '2026-08-03 16:55:00', 4),
(9, 8, '2026-08-04 17:05:00', 5),
(10, 3, '2026-08-04 18:00:00', 5),
(11, 12, '2026-08-04 19:58:00', 5),
(12, 5, '2026-08-06 17:02:00', 4),
(14, 13, '2026-08-07 08:59:00', 5),
(15, 10, '2026-08-08 07:01:00', 4),
(1, 15, '2026-08-10 16:00:00', 5),
(6, 2, '2026-08-10 08:30:00', 5);