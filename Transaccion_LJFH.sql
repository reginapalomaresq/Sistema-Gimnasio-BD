USE gimnasio_db;

-- Prueba con error forzado (id_membresia = 999 no existe -> dispara HANDLER y hace ROLLBACK)
CALL sp_registrar_pago_membresia(1, 999, 650.00, 'Efectivo', @id_nuevo);
SELECT @id_nuevo AS resultado_transaccion; -- Devuelve -1 indicando que se hizo ROLLBACK

-- Prueba exitosa (id_socio = 1, id_membresia = 2 -> hace COMMIT en pagos y socios)
CALL sp_registrar_pago_membresia(1, 2, 650.00, 'Tarjeta', @id_nuevo);
SELECT @id_nuevo AS id_pago_confirmado; -- Devuelve el ID generado (> 0)