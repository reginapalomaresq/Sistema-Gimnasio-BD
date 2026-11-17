USE gimnasio_db;

-- 2.2 Consulta con WHERE, LIKE, BETWEEN e IN (para filtros en C#)
SELECT id_socio, nombre, apellidos, email, fecha_registro
FROM socios
WHERE (nombre LIKE '%Carlos%' OR apellidos LIKE '%Pérez%')
  AND fecha_registro BETWEEN '2026-01-01' AND '2026-12-31'
  AND id_membresia IN (1, 2, 3);

-- 2.3 Reporte con GROUP BY, HAVING, Agregados, Funciones de cadena/fecha y CAST
SELECT 
    UPPER(m.nombre_plan) AS plan,
    COUNT(p.id_pago) AS total_pagos,
    CAST(SUM(p.monto) AS DECIMAL(10,2)) AS ingresos_totales,
    ROUND(AVG(p.monto), 2) AS promedio_pago,
    YEAR(p.fecha_pago) AS anio
FROM pagos p
INNER JOIN membresias m ON p.id_membresia = m.id_membresia
GROUP BY m.nombre_plan, YEAR(p.fecha_pago)
HAVING SUM(p.monto) > 1000
ORDER BY ingresos_totales DESC;

-- 2.4 Consulta con LEFT JOIN (Instructores y sus clases, incluso si aún no tienen clases asignadas)
SELECT 
    CONCAT(i.nombre, ' ', i.apellidos) AS instructor,
    i.especialidad,
    COALESCE(c.nombre_clase, 'Sin clase asignada') AS clase,
    c.horario
FROM instructores i
LEFT JOIN clases c ON i.id_instructor = c.id_instructor
ORDER BY instructor;

-- 2.5 Subconsulta 1 (en WHERE): Socios que han realizado pagos mayores al promedio general
SELECT id_socio, nombre, apellidos, email
FROM socios
WHERE id_socio IN (
    SELECT id_socio 
    FROM pagos 
    WHERE monto > (SELECT AVG(monto) FROM pagos)
);

-- 2.5 Subconsulta 2 (Correlacionada): Socios con su total de asistencias registradas
SELECT 
    s.id_socio,
    CONCAT(s.nombre, ' ', s.apellidos) AS socio,
    (SELECT COUNT(*) 
     FROM asistencias a 
     WHERE a.id_socio = s.id_socio) AS total_asistencias
FROM socios s
ORDER BY total_asistencias DESC;

-- 2.6 Vistas (Mínimo 3 para equipo de 3 personas)

-- Vista 1: Directorio completo de socios y su plan actual (INNER JOIN)
CREATE OR REPLACE VIEW vw_socios_membresias AS
SELECT 
    s.id_socio,
    CONCAT(s.nombre, ' ', s.apellidos) AS nombre_completo,
    s.email,
    s.telefono,
    s.estatus,
    m.nombre_plan,
    m.precio AS costo_mensual
FROM socios s
INNER JOIN membresias m ON s.id_membresia = m.id_membresia;

-- Vista 2: Detalle de pagos realizados con datos del socio y plan
CREATE OR REPLACE VIEW vw_historial_pagos AS
SELECT 
    p.id_pago,
    p.fecha_pago,
    CONCAT(s.nombre, ' ', s.apellidos) AS socio,
    m.nombre_plan,
    p.monto,
    p.metodo_pago
FROM pagos p
INNER JOIN socios s ON p.id_socio = s.id_socio
INNER JOIN membresias m ON p.id_membresia = m.id_membresia;

-- Vista 3: Ocupación y asistencia por clase e instructor
CREATE OR REPLACE VIEW vw_ocupacion_clases AS
SELECT 
    c.id_clase,
    c.nombre_clase,
    c.horario,
    CONCAT(i.nombre, ' ', i.apellidos) AS instructor,
    c.cupo_maximo,
    COUNT(a.id_asistencia) AS total_asistencias,
    ROUND(AVG(a.calificacion_clase), 1) AS promedio_calificacion
FROM clases c
INNER JOIN instructores i ON c.id_instructor = i.id_instructor
LEFT JOIN asistencias a ON c.id_clase = a.id_clase
GROUP BY c.id_clase, c.nombre_clase, c.horario, instructor, c.cupo_maximo;