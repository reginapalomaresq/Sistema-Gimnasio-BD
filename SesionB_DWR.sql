USE gimnasio_db;
START TRANSACTION;
UPDATE membresias SET precio = 500.00 WHERE id_membresia = 1;
COMMIT;

-- prueba serializable
START TRANSACTION;
UPDATE membresias SET precio = 450.00 WHERE id_membresia = 1;