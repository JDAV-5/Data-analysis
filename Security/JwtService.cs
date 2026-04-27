using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using APPCORE.Security;

namespace ETLService.Security
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Security_Users user)
        {
            // Validar configuración
            var keyString = _config["JwtSettings:Key"]
                ?? throw new Exception("JWT Key no configurada");

            var issuer = _config["JwtSettings:Issuer"]
                ?? throw new Exception("JWT Issuer no configurado");

            var audience = _config["JwtSettings:Audience"]
                ?? throw new Exception("JWT Audience no configurado");

            var lifetimeStr = _config["JwtSettings:TokenLifetimeMinutes"];

            if (!double.TryParse(lifetimeStr, out double lifetimeMinutes))
                lifetimeMinutes = 30; // valor por defecto

            // Clave de seguridad
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 📦 Claims (datos dentro del token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id_User?.ToString() ?? "0"),
                new Claim(ClaimTypes.Name, user.Nombres ?? "")
            };

            // Crear token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(lifetimeMinutes),
                signingCredentials: creds
            );

            // Convertir a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}