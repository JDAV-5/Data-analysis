using Microsoft.AspNetCore.Mvc;
using ETLService.Security;
using ETLService.Security.Model;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly DbHelper _db;
        private readonly JwtService _jwt;

        public SecurityController(DbHelper db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserModel inst)
        {
            var result = AuthNetCore.Login(inst, _db, _jwt);

            if (result == null)
                return Unauthorized("Credenciales incorrectas");

            return Ok(result);
        }

        [HttpPost]
        public IActionResult RecoveryPassword([FromBody] UserModel inst)
        {
            var result = AuthNetCore.RecoveryPassword(inst.username, _db);

            if (!result)
                return NotFound("Usuario no existe");

            return Ok("Proceso de recuperación iniciado");
        }
    }
}