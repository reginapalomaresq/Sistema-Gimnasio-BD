USE gimnasio_db;

-- Limpieza previa si se re-ejecuta el script
DROP USER IF EXISTS 'usr_admin'@'localhost', 'usr_recepcion'@'localhost', 'usr_instructor'@'localhost';
DROP ROLE IF EXISTS 'rol_admin_gym', 'rol_recepcion', 'rol_instructor';

-- 3.4 Creación de los 3 Roles de BD
CREATE ROLE 'rol_admin_gym', 'rol_recepcion', 'rol_instructor';

-- 3.3 Asignación de privilegios (GRANT) a los ROLES (no directo a usuarios)

-- Permisos Rol 1: Administrador
GRANT ALL PRIVILEGES ON gimnasio_db.* TO 'rol_admin_gym';

-- Permisos Rol 2: Recepción (Operador)
GRANT SELECT, INSERT, UPDATE ON gimnasio_db.socios TO 'rol_recepcion';
GRANT SELECT, INSERT ON gimnasio_db.pagos TO 'rol_recepcion';
GRANT SELECT, INSERT ON gimnasio_db.asistencias TO 'rol_recepcion';
GRANT SELECT ON gimnasio_db.membresias TO 'rol_recepcion';
GRANT SELECT ON gimnasio_db.clases TO 'rol_recepcion';
GRANT SELECT ON gimnasio_db.vw_socios_membresias TO 'rol_recepcion';
GRANT SELECT ON gimnasio_db.vw_historial_pagos TO 'rol_recepcion';
GRANT SELECT ON gimnasio_db.vw_ocupacion_clases TO 'rol_recepcion';
GRANT EXECUTE ON PROCEDURE gimnasio_db.sp_registrar_pago_membresia TO 'rol_recepcion';
GRANT EXECUTE ON PROCEDURE gimnasio_db.sp_registrar_asistencia TO 'rol_recepcion';
GRANT EXECUTE ON PROCEDURE gimnasio_db.sp_alta_socio TO 'rol_recepcion';
GRANT EXECUTE ON FUNCTION gimnasio_db.fn_cupos_disponibles_hoy TO 'rol_recepcion';

-- Permisos Rol 3: Instructor (Solo lectura limitada a vistas)
GRANT SELECT ON gimnasio_db.vw_ocupacion_clases TO 'rol_instructor';
GRANT SELECT ON gimnasio_db.vw_socios_membresias TO 'rol_instructor';

-- Demostración explícita de REVOKE (Competencia 3.3)
REVOKE DELETE ON gimnasio_db.* FROM 'rol_recepcion';
REVOKE UPDATE ON gimnasio_db.pagos FROM 'rol_recepcion';

-- 3.2 Creación de Usuarios de BD para la App C#
CREATE USER 'usr_admin'@'localhost' IDENTIFIED BY 'AdminGym2026!';
CREATE USER 'usr_recepcion'@'localhost' IDENTIFIED BY 'RecepGym2026!';
CREATE USER 'usr_instructor'@'localhost' IDENTIFIED BY 'InstGym2026!';

-- Asignación de roles a los usuarios y activación por defecto
GRANT 'rol_admin_gym' TO 'usr_admin'@'localhost';
GRANT 'rol_recepcion' TO 'usr_recepcion'@'localhost';
GRANT 'rol_instructor' TO 'usr_instructor'@'localhost';

SET DEFAULT ROLE ALL TO 
    'usr_admin'@'localhost', 
    'usr_recepcion'@'localhost', 
    'usr_instructor'@'localhost';

FLUSH PRIVILEGES;