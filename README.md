# Data Analytics and inventory (c# + SQL Server + Power BI)



Este proyecto implementa una solucion end-to-end de analisis e ingesta de datos. Itegra una aplicacion de consola en \*\*C# (.NET8)\*\* para el procesamiento masivo de ventas desde archivos CSV, una base de datos relacional en \*\*SQL SERVER\*\* y un dashboard interactivo en \*\*Power BI\*\* para la toma de decisiones.



\---



\## Dashboard con Power (BI)

\### 1.Ingesta y procesamiento (C# - .net 8)

\*lectura y validacion de archivos de transacciones en formato csv.

\*conexion directa a SQL SERVER mediante Microsoft.Data.SqlClient.

\*Invocacion de Stored Procedures para manejo seguro de parametros y prevencion de errores.
\*Consola interactiva con resumenes y alertas en tiempo real



\### 2. Base de datos relacional (SQLSERVER)

* \*\*Tablas:\*\* Productos (Inventario) y Ventas (Transacciones).
* \*\*Stores Procedure (sp\_RegistrarVenta):\*\* Valida disponibilidad de productos y reglas antes de registrar la venta.
* \*\*Trigger (trg\_Actualizar):\*\* Actualizacion automatica de stock tras cada insercion.
* \*\*Vista (vw\_ReporteInventarioVentas):\*\* Agregacion optimizada de datos lista para consumo	analitico en power bi



\### 3. Business Intelligence and dax (Power BI)

* Modelo analitico alimentado desde la vista de SQL SERVER.
* \*\*Metricas dax:\*\* Ingresos Totales, Ticket Promedio, Unidades Vendidas Y Deteccion de Produtos con Stock critico.
* Visualizaciones interactivas de tendencia temporal y distribucion por producto.

