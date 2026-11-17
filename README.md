# Sistema de Gestión para Gimnasio (`gimnasio_db`)

Proyecto final de conectividad de bases de datos desarrollado en **C# (.NET 8)** con **MySQL 8.0** mediante **ADO.NET** (`MySql.Data`), aplicando diseño relacional, DDL, DML, vistas, control de acceso, transacciones y SQL procedural.

## Integrantes del Equipo (3)
* **[Regina Palomares Quintero]** – Instalación de SGBD, Modelo Entidad-Relación, Arquitectura y Script DDL (`01_esquema.sql`).
* **[David Wong Reyes]** – Consultas Avanzadas, Reportes, Subconsultas y Vistas (`03_consultas_vistas.sql`).
* **[Luis Javier Fierros Hurtado]** – Poblado de Base de Datos y Carga Inicial de Registros (`02_datos.sql`).

## Arquitectura de la Base de Datos (6 Tablas)

El modelo cumple con el mínimo de 6 tablas propias del negocio (con 5 a 7 columnas cada una) y 6 llaves foráneas, incluyendo una relación **N:M** con atributos propios (`asistencias`):

| Tabla | Descripción | Llaves Foráneas (FK) |
| :--- | :--- | :--- |
| **`membresias`** | Catálogo de planes y tarifas del gimnasio. | Ninguna (Tabla catálogo) |
| **`socios`** | Registro de clientes inscritos y estatus de cuenta. | `id_membresia` → `membresias` |
| **`pagos`** | Historial de cobros y métodos de pago realizados. | `id_socio` → `socios`<br>`id_membresia` → `membresias` |
| **`instructores`** | Entrenadores del gimnasio y su especialidad. | Ninguna (Tabla catálogo) |
| **`clases`** | Sesiones grupales, horarios y cupo máximo. | `id_instructor` → `instructores` |
| **`asistencias`** | Tabla asociativa (**N:M**) con fecha y calificación de clase. | `id_socio` → `socios`<br>`id_clase` → `clases` |

## Estructura del Repositorio
* `01_esquema.sql` – Creación de `gimnasio_db`, las 6 tablas, los 5 tipos de constraints (`PK`, `FK`, `UNIQUE`, `CHECK`, `NOT NULL/DEFAULT`) y `ALTER TABLE`.
* `Modelo_ER.png` – Diagrama Entidad-Relación exportado desde MySQL Workbench.
* `/evidencias` – Capturas de instalación de MySQL Server 8.0.46, puerto `3306` y servicio activo.

## Consultas y Vistas Implementadas (03_consultas_vistas.sql)
Para cubrir los requerimientos de manipulación y consulta de datos desde la aplicación en C#, se implementaron:

##Filtros dinámicos: Búsquedas parametrizadas utilizando WHERE, LIKE, BETWEEN e IN sobre socios, fechas de registro y planes.
##Reportes agrupados: Cálculo de ingresos totales y promedios por plan y año mediante GROUP BY, HAVING, SUM, COUNT, AVG, CAST y ORDER BY.
##Combinaciones (Joins): Consultas con INNER JOIN para historial de pagos y LEFT JOIN para listar instructores con o sin clases asignadas.
##Subconsultas: Una subconsulta en cláusula WHERE (socios con pagos superiores al promedio) y una subconsulta correlacionada en SELECT (conteo de asistencias por socio).
##Vistas del sistema (3):
1. 'vw_socios_membresias: Directorio general de socios con su plan y costo mensual.'
2. 'vw_historial_pagos: Detalle completo de cobros realizados por socio y método de pago.'
3. 'vw_ocupacion_clases: Estadísticas de asistencia y calificación promedio por clase e instructor.'
