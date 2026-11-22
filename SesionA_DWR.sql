USE gimnasio_db;
SET SESSION TRANSACTION ISOLATION LEVEL READ COMMITTED;
START TRANSACTION;
SELECT id_membresia, nombre_plan, precio FROM membresias WHERE id_membresia = 1;