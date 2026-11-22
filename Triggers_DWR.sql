USE gimnasio_db;

-- Tabla auxiliar de auditoría para evidenciar Trigger de bitácora
CREATE TABLE IF NOT EXISTS bitacora_pagos (
    id_bitacora INT AUTO_INCREMENT PRIMARY KEY,
    id_pago INT NOT NULL,
    accion VARCHAR(20) NOT NULL,
    monto_anterior DECIMAL(10,2),
    monto_nuevo DECIMAL(10,2),
    usuario_bd VARCHAR(80),
    fecha_evento DATETIME DEFAULT CURRENT_TIMESTAMP
);

DELIMITER $$

-- ==========================================================
-- 5.1 STORED PROCEDURES (Mínimo 6 para equipo de 3)
-- ==========================================================

-- SP 1: Registrar pago y actualizar membresía del socio (Transacción + Parámetro OUT + Handler)
CREATE PROCEDURE sp_registrar_pago_membresia(
    IN p_id_socio INT,
    IN p_id_membresia INT,
    IN p_monto DECIMAL(10,2),
    IN p_metodo ENUM('Efectivo', 'Tarjeta', 'Transferencia'),
    OUT p_id_pago_generado INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_id_pago_generado = -1;
        RESIGNAL;
    END;

    START TRANSACTION;
        INSERT INTO pagos (id_socio, id_membresia, monto, metodo_pago)
        VALUES (p_id_socio, p_id_membresia, p_monto, p_metodo);

        SET p_id_pago_generado = LAST_INSERT_ID();

        UPDATE socios
        SET id_membresia = p_id_membresia,
            estatus = 'Activo'
        WHERE id_socio = p_id_socio;
    COMMIT;
END$$

-- SP 2: Registrar asistencia validando cupo máximo y estatus activo (Manejo de errores)
CREATE PROCEDURE sp_registrar_asistencia(
    IN p_id_socio INT,
    IN p_id_clase INT,
    IN p_calificacion TINYINT
)
BEGIN
    DECLARE v_estatus VARCHAR(20);
    DECLARE v_cupo INT;
    DECLARE v_inscritos_hoy INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        RESIGNAL;
    END;

    SELECT estatus INTO v_estatus FROM socios WHERE id_socio = p_id_socio;
    IF v_estatus <> 'Activo' THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El socio se encuentra Suspendido y no puede ingresar a clase.';
    END IF;

    SELECT cupo_maximo INTO v_cupo FROM clases WHERE id_clase = p_id_clase;
    SELECT COUNT(*) INTO v_inscritos_hoy 
    FROM asistencias 
    WHERE id_clase = p_id_clase AND DATE(fecha_asistencia) = CURDATE();

    IF v_inscritos_hoy >= v_cupo THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cupo máximo alcanzado para esta clase el día de hoy.';
    END IF;

    INSERT INTO asistencias (id_socio, id_clase, calificacion_clase)
    VALUES (p_id_socio, p_id_clase, p_calificacion);
END$$

-- SP 3: Alta de nuevo socio
CREATE PROCEDURE sp_alta_socio(
    IN p_nombre VARCHAR(60),
    IN p_apellidos VARCHAR(80),
    IN p_email VARCHAR(100),
    IN p_telefono VARCHAR(15),
    IN p_id_membresia INT
)
BEGIN
    INSERT INTO socios (nombre, apellidos, email, telefono, id_membresia)
    VALUES (p_nombre, p_apellidos, p_email, p_telefono, p_id_membresia);
END$$

-- SP 4: Actualizar salario de instructor con parámetro OUT de nuevo salario
CREATE PROCEDURE sp_aumentar_salario_instructor(
    IN p_id_instructor INT,
    IN p_porcentaje DECIMAL(5,2),
    OUT p_nuevo_salario DECIMAL(10,2)
)
BEGIN
    UPDATE instructores
    SET salario = salario + (salario * (p_porcentaje / 100))
    WHERE id_instructor = p_id_instructor;

    SELECT salario INTO p_nuevo_salario
    FROM instructores
    WHERE id_instructor = p_id_instructor;
END$$

-- SP 5: Suspender socio por falta de pago
CREATE PROCEDURE sp_suspender_socio(IN p_id_socio INT)
BEGIN
    UPDATE socios SET estatus = 'Suspendido' WHERE id_socio = p_id_socio;
END$$

-- SP 6: Reporte de ingresos por rango de fechas
CREATE PROCEDURE sp_reporte_ingresos_fechas(
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    SELECT p.id_pago, CONCAT(s.nombre, ' ', s.apellidos) AS socio,
           m.nombre_plan, p.monto, p.metodo_pago, p.fecha_pago
    FROM pagos p
    INNER JOIN socios s ON p.id_socio = s.id_socio
    INNER JOIN membresias m ON p.id_membresia = m.id_membresia
    WHERE DATE(p.fecha_pago) BETWEEN p_fecha_inicio AND p_fecha_fin
    ORDER BY p.fecha_pago DESC;
END$$

-- ==========================================================
-- 5.2 FUNCIONES DEFINIDAS POR EL USUARIO (Mínimo 3)
-- ==========================================================

-- Función 1: Calcular total gastado históricamente por un socio
CREATE FUNCTION fn_total_pagado_socio(p_id_socio INT)
RETURNS DECIMAL(10,2)
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_total DECIMAL(10,2);
    SELECT COALESCE(SUM(monto), 0.00) INTO v_total
    FROM pagos
    WHERE id_socio = p_id_socio;
    RETURN v_total;
END$$

-- Función 2: Calcular cupos disponibles de una clase en el día actual
CREATE FUNCTION fn_cupos_disponibles_hoy(p_id_clase INT)
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_max INT;
    DECLARE v_usados INT;
    SELECT cupo_maximo INTO v_max FROM clases WHERE id_clase = p_id_clase;
    SELECT COUNT(*) INTO v_usados FROM asistencias 
    WHERE id_clase = p_id_clase AND DATE(fecha_asistencia) = CURDATE();
    RETURN (v_max - v_usados);
END$$

-- Función 3: Calcular días de antigüedad de un socio
CREATE FUNCTION fn_antiguedad_dias(p_id_socio INT)
RETURNS INT
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_dias INT;
    SELECT DATEDIFF(CURDATE(), fecha_registro) INTO v_dias
    FROM socios WHERE id_socio = p_id_socio;
    RETURN COALESCE(v_dias, 0);
END$$

-- ==========================================================
-- 5.3 TRIGGERS (Mínimo 3 con propósito real)
-- ==========================================================

-- Trigger 1 (Bitácora): Registrar en auditoría cada vez que se inserta un pago
CREATE TRIGGER trg_after_insert_pago
AFTER INSERT ON pagos
FOR EACH ROW
BEGIN
    INSERT INTO bitacora_pagos (id_pago, accion, monto_anterior, monto_nuevo, usuario_bd)
    VALUES (NEW.id_pago, 'INSERT', 0.00, NEW.monto, USER());
END$$

-- Trigger 2 (Bitácora de modificación): Auditar cambios de monto en pagos
CREATE TRIGGER trg_after_update_pago
AFTER UPDATE ON pagos
FOR EACH ROW
BEGIN
    INSERT INTO bitacora_pagos (id_pago, accion, monto_anterior, monto_nuevo, usuario_bd)
    VALUES (NEW.id_pago, 'UPDATE', OLD.monto, NEW.monto, USER());
END$$

-- Trigger 3 (Regla de negocio): Impedir asignar un plan Inactivo a un socio nuevo
CREATE TRIGGER trg_before_insert_socio
BEFORE INSERT ON socios
FOR EACH ROW
BEGIN
    DECLARE v_estatus_plan VARCHAR(20);
    SELECT estatus INTO v_estatus_plan FROM membresias WHERE id_membresia = NEW.id_membresia;
    IF v_estatus_plan = 'Inactiva' THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'No se puede inscribir a un socio en una membresía Inactiva.';
    END IF;
END$$

DELIMITER ;

-- Ejemplo de consulta usando las 3 funciones para evidencia de Competencia 5.2:
SELECT 
    id_socio,
    CONCAT(nombre, ' ', apellidos) AS socio,
    fn_antiguedad_dias(id_socio) AS dias_inscrito,
    fn_total_pagado_socio(id_socio) AS total_invertido
FROM socios;