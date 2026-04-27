using System.Data;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using ETLService.Security.Model;
using APPCORE.Security;

namespace ETLService.Security
{
    public class AuthNetCore
    {
        // LOGIN cON BD + JWT
        public static object? Login(UserModel inst, DbHelper db, JwtService jwt)
        {
            if (inst == null ||
                string.IsNullOrWhiteSpace(inst.username) ||
                string.IsNullOrWhiteSpace(inst.password))
            {
                return null;
            }

            using var conn = db.GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("sp_GetUserByUsername", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", inst.username);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            var user = new Security_Users
            {
                Id_User = (int)reader["Id_User"],
                Nombres = reader["Nombre"].ToString(),
                Password = reader["Password"].ToString(),
                Estado = reader["Estado"].ToString()
            };

            // Validar estado
            if (user.Estado != "Activo")
                return null;

            //  Validar contraseña (BCrypt)
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(inst.password, user.Password);

            if (!isValidPassword)
                return null;

            // Generar token JWT
            var token = jwt.GenerateToken(user);

            return new
            {
                token,
                user = new
                {
                    user.Id_User,
                    user.Nombres
                }
            };
        }

        // Recuperación de contraseña (básica)
        public static bool RecoveryPassword(string username, DbHelper db)
        {
            using var conn = db.GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("sp_GetUserByUsername", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", username);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return false;

            // Aquí podrías enviar correo en un sistema real
            return true;
        }

        // Obtener usuario desde el token (opcional)
        public static object? GetUserFromToken(System.Security.Claims.ClaimsPrincipal userClaims)
        {
            var idClaim = userClaims.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (idClaim == null)
                return null;

            return new
            {
                Id_User = idClaim.Value,
                Username = userClaims.Identity?.Name
            };
        }
    }
}