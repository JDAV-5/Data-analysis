using Microsoft.Data.SqlClient;
using System.Data;

namespace ETLService.Security
{
    public class DbHelper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DbHelper(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        // 🔌 Obtener conexión abierta
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // 🔹 Ejecutar SELECT (DataTable)
        public DataTable ExecuteQuery(string query, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            var dt = new DataTable();

            conn.Open();
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);

            return dt;
        }

        // 🔹 Ejecutar procedimientos almacenados (SELECT)
        public DataTable ExecuteStoredProcedure(string spName, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            var dt = new DataTable();

            conn.Open();
            using var reader = cmd.ExecuteReader();
            dt.Load(reader);

            return dt;
        }

        // 🔹 Ejecutar INSERT / UPDATE / DELETE
        public int ExecuteNonQuery(string query, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // Ejecutar procedimiento almacenado (INSERT/UPDATE/DELETE)
        public int ExecuteNonQuerySP(string spName, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // Ejecutar escalar (ej: COUNT, IDENTITY)
        public object? ExecuteScalar(string query, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteScalar();
        }

        // Ejecutar escalar con SP
        public object? ExecuteScalarSP(string spName, List<SqlParameter>? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
}