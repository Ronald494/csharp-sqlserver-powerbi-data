using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAnalytics
{
    internal class Program
    {
        // Cadena de conexión a tu base de datos DataAnalytics
        private static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=DataAnalytics;Trusted_Connection=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================================");
            Console.WriteLine("                 VENTAS Y CONTROL DE STOCK           ");
            Console.WriteLine("=========================================================\n");
            Console.ResetColor();

            string rutaArchivo = "ventas_prueba.csv";

            // crea uno si el archivo no existe
            if (!File.Exists(rutaArchivo))
            {
                File.WriteAllLines(rutaArchivo, new string[]
                {
                    "Producto,Cantidad,Fecha",
                    "Laptop Asus,2,2026-03-01",
                    "Monitor LG,4,2026-03-02",
                    "Teclado Mecanico,5,2026-03-03",
                    "Producto Fantasma,1,2026-03-03" // Para probar error de si no existe
                });
            }

            var lineas = File.ReadAllLines(rutaArchivo);
            decimal totalProcesado = 0;
            int ventasExitosas = 0;

            // Procesar cada registro
            for (int i = 1; i < lineas.Length; i++)
            {
                var columnas = lineas[i].Split(',');
                string producto = columnas[0].Trim();
                int cantidad = int.Parse(columnas[1].Trim());
                DateTime fecha = DateTime.Parse(columnas[2].Trim());

                var resultado = ProcesarVenta(producto, cantidad, fecha);

                if (resultado.Exito)
                {
                    ventasExitosas++;
                    totalProcesado += resultado.Monto;
                }
            }

            // resultado por consola 
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=========================================================");
            Console.WriteLine("                      Resultado                       ");
            Console.WriteLine("=========================================================");
            Console.ResetColor();
            Console.WriteLine($" Transacciones Exitosas: {ventasExitosas}");
            Console.WriteLine($" Monto Total Procesado:  ${totalProcesado:N2}");

            // Consultar alertas de stock en SQL
            MostrarAlertasStock();

            Console.WriteLine("\nPresiona cualquier tecla para finalizar");
            Console.ReadKey();
        }

        private static (bool Exito, decimal Monto) ProcesarVenta(string producto, int cantidad, DateTime fecha)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreProducto", producto);
                    cmd.Parameters.AddWithValue("@Cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        // obtener precio para el resultado
                        decimal precioUnitario = ObtenerPrecio(producto, con);
                        decimal total = cantidad * precioUnitario;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" [OK] ");
                        Console.ResetColor();
                        Console.WriteLine($"{producto,-18} | Cantidad: {cantidad,-2} | Total: ${total,8:N2}");

                        return (true, total);
                    }
                    catch (SqlException ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" [ERROR] ");
                        Console.ResetColor();
                        Console.WriteLine($"{producto,-18} | Mensaje: {ex.Message}");

                        return (false, 0);
                    }
                }
            }
        }

        private static decimal ObtenerPrecio(string producto, SqlConnection con)
        {
            string query = "SELECT PrecioUnitario FROM Productos WHERE Nombre = @Nombre";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Nombre", producto);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }

        private static void MostrarAlertasStock()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Nombre, Stock FROM Productos WHERE Stock <= 3";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n productos con stock crítico (<= 3):");
                    Console.ResetColor();

                    while (reader.Read())
                    {
                        Console.WriteLine($"   -> {reader["Nombre"]}: Quedan solo {reader["Stock"]} unidades.");
                    }
                }
            }
        }
    }
}