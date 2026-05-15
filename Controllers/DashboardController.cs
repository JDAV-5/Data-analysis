using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ETLService.Security;
using System.Data;

namespace ETLService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DbHelper _db;

        public DashboardController(DbHelper db)
        {
            _db = db;
        }

        // =========================================
        // KPI: VENTAS TOTALES
        // =========================================
        [HttpGet("ventas-totales")]
        public IActionResult GetVentasTotales()
        {
            using var conn = _db.GetConnection();

            conn.Open();

            string query = @"
                SELECT
                    SUM(TotalVenta) AS VentasTotales
                FROM Ventas
            ";

            using var cmd = new SqlCommand(query, conn);

            var result = cmd.ExecuteScalar();

            return Ok(new
            {
                VentasTotales = result
            });
        }



        // =========================================
        // KPI: PRODUCTOS MÁS VENDIDOS
        // =========================================
        [HttpGet("productos-top")]
        public IActionResult GetProductosTop()
        {
            using var conn = _db.GetConnection();

            conn.Open();

            string query = @"
                SELECT TOP 5
                    p.NombreProducto,
                    SUM(dv.Cantidad) AS TotalVendido
                FROM DetalleVenta dv

                INNER JOIN Productos p
                ON dv.Id_Producto = p.Id_Producto

                GROUP BY p.NombreProducto

                ORDER BY TotalVendido DESC
            ";

            using var cmd = new SqlCommand(query, conn);

            using var reader = cmd.ExecuteReader();

            var data = new List<object>();

            while (reader.Read())
            {
                data.Add(new
                {
                    Producto = reader["NombreProducto"],
                    TotalVendido = reader["TotalVendido"]
                });
            }

            return Ok(data);
        }



        // =========================================
        // KPI: VENTAS POR DÍA
        // =========================================
        [HttpGet("ventas-por-dia")]
        public IActionResult GetVentasPorDia()
        {
            using var conn = _db.GetConnection();

            conn.Open();

            string query = @"
                SELECT
                    CONVERT(DATE, FechaVenta) AS Fecha,
                    SUM(TotalVenta) AS TotalVentas
                FROM Ventas

                GROUP BY CONVERT(DATE, FechaVenta)

                ORDER BY Fecha
            ";

            using var cmd = new SqlCommand(query, conn);

            using var reader = cmd.ExecuteReader();

            var data = new List<object>();

            while (reader.Read())
            {
                data.Add(new
                {
                    Fecha = reader["Fecha"],
                    TotalVentas = reader["TotalVentas"]
                });
            }

            return Ok(data);
        }



        // =========================================
        // ALERTA: STOCK BAJO
        // =========================================
        [HttpGet("stock-bajo")]
        public IActionResult GetStockBajo()
        {
            using var conn = _db.GetConnection();

            conn.Open();

            string query = @"
                SELECT
                    NombreProducto,
                    Stock
                FROM Productos

                WHERE Stock <= 20

                ORDER BY Stock ASC
            ";

            using var cmd = new SqlCommand(query, conn);

            using var reader = cmd.ExecuteReader();

            var data = new List<object>();

            while (reader.Read())
            {
                data.Add(new
                {
                    Producto = reader["NombreProducto"],
                    Stock = reader["Stock"]
                });
            }

            return Ok(data);
        }
    }
}