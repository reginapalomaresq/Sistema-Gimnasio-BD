DROP DATABASE IF EXISTS gimnasio_db;
CREATE DATABASE gimnasio_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE gimnasio_db;

-- 1. Tabla Membresías
CREATE TABLE membresias (
    id_membresia INT AUTO_INCREMENT PRIMARY KEY,
    nombre_plan VARCHAR(50) NOT NULL UNIQUE,
    duracion_dias INT NOT NULL DEFAULT 30,
    precio DECIMAL(10, 2) NOT NULL,
    estatus ENUM('Activa', 'Inactiva') NOT NULL DEFAULT 'Activa',
    CONSTRAINT chk_duracion CHECK (duracion_dias > 0),
    CONSTRAINT chk_precio_membresia CHECK (precio >= 0)
);

-- 2. Tabla Socios
CREATE TABLE socios (
    id_socio INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL,
    apellidos VARCHAR(80) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    telefono VARCHAR(15) NOT NULL,
    fecha_registro DATE NOT NULL DEFAULT (CURRENT_DATE),
    id_membresia INT NOT NULL,
    CONSTRAINT fk_socio_membresia 
        FOREIGN KEY (id_membresia) REFERENCES membresias(id_membresia)
        ON UPDATE CASCADE
        ON DELETE RESTRICT -- Impide borrar un plan si hay socios inscritos en él
);

-- 3. Tabla Pagos
CREATE TABLE pagos (
    id_pago INT AUTO_INCREMENT PRIMARY KEY,
    id_socio INT NOT NULL,
    id_membresia INT NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    fecha_pago DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    metodo_pago ENUM('Efectivo', 'Tarjeta', 'Transferencia') NOT NULL DEFAULT 'Efectivo',
    CONSTRAINT chk_monto_pago CHECK (monto > 0),
    CONSTRAINT fk_pago_socio 
        FOREIGN KEY (id_socio) REFERENCES socios(id_socio)
        ON UPDATE CASCADE
        ON DELETE RESTRICT, -- Conserva integridad histórica de pagos
    CONSTRAINT fk_pago_membresia 
        FOREIGN KEY (id_membresia) REFERENCES membresias(id_membresia)
        ON UPDATE CASCADE
        ON DELETE RESTRICT
);

-- 4. Tabla Instructores
CREATE TABLE instructores (
    id_instructor INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(60) NOT NULL,
    apellidos VARCHAR(80) NOT NULL,
    especialidad VARCHAR(60) NOT NULL DEFAULT 'Acondicionamiento General',
    telefono VARCHAR(15) NOT NULL UNIQUE,
    salario DECIMAL(10, 2) NOT NULL,
    CONSTRAINT chk_salario_instructor CHECK (salario >= 5000.00)
);

-- 5. Tabla Clases
CREATE TABLE clases (
    id_clase INT AUTO_INCREMENT PRIMARY KEY,
    nombre_clase VARCHAR(60) NOT NULL,
    horario VARCHAR(50) NOT NULL,
    cupo_maximo INT NOT NULL DEFAULT 20,
    id_instructor INT NOT NULL,
    CONSTRAINT chk_cupo_clase CHECK (cupo_maximo BETWEEN 1 AND 50),
    CONSTRAINT fk_clase_instructor 
        FOREIGN KEY (id_instructor) REFERENCES instructores(id_instructor)
        ON UPDATE CASCADE
        ON DELETE RESTRICT -- No permite eliminar un instructor si tiene clases asignadas
);

-- 6. Tabla Asistencias (Relación N:M entre Socios y Clases con atributos propios)
CREATE TABLE asistencias (
    id_asistencia INT AUTO_INCREMENT PRIMARY KEY,
    id_socio INT NOT NULL,
    id_clase INT NOT NULL,
    fecha_asistencia DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    calificacion_clase TINYINT NULL,
    CONSTRAINT chk_calificacion CHECK (calificacion_clase BETWEEN 1 AND 5),
    CONSTRAINT fk_asistencia_socio 
        FOREIGN KEY (id_socio) REFERENCES socios(id_socio)
        ON UPDATE CASCADE
        ON DELETE CASCADE, -- Si se elimina un socio de prueba, se limpian sus asistencias
    CONSTRAINT fk_asistencia_clase 
        FOREIGN KEY (id_clase) REFERENCES clases(id_clase)
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

-- Evidencia de ALTER TABLE solicitado en el avance semanal
ALTER TABLE socios 
ADD COLUMN estatus ENUM('Activo', 'Suspendido') NOT NULL DEFAULT 'Activo' AFTER fecha_registro;