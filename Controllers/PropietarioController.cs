using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InmobiliariaAPI.Data;
using InmobiliariaAPI.Interfaces;
using InmobiliariaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InmobiliariaAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietarioController : ControllerBase
    {
        private readonly Context _context;
        private readonly IConfiguration _config;
        private readonly IPropietarioService _service;

        public PropietarioController(Context context, IConfiguration config, IPropietarioService service)
        {
            _context = context;
            _config = config;
            _service = service;
        }

        // LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginView loginView)
        {
            if (loginView.Usuario == null || loginView.Clave == null)
                return BadRequest("Usuario y clave son requeridos");

            string hashed = await _service.HashPassword(loginView.Clave);/* Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginView.Clave,
                salt: Encoding.ASCII.GetBytes(_config["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8)); */

            var propietario = await _context.Propietarios
                .FirstOrDefaultAsync(x => x.Email == loginView.Usuario);

            if (propietario == null || propietario.Password != hashed)
                return BadRequest("Usuario o contraseña incorrecta");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, propietario.Email),
                new Claim("FullName", $"{propietario.Nombre} {propietario.Apellido}"),
                new Claim(ClaimTypes.Role, "Propietario")
            };

            var token = new JwtSecurityToken(
                issuer: _config["TokenAuthentication:Issuer"],
                audience: _config["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credenciales
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // Obtener datos del usuario logueado
        [HttpGet("usuarioActual")]
        public async Task<IActionResult> UsuarioActual()
        {
            var email = User.Identity?.Name;
            if (email == null) return Unauthorized();

            var propietario = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
            return propietario == null ? NotFound() : Ok(propietario);
        }

        // Editar perfil
        [HttpPut("editar")]
        public async Task<IActionResult> Editar([FromBody] Propietario entidad)
        {
            var email = User.Identity?.Name;
            var usuarioActual = await _context.Propietarios.FirstOrDefaultAsync(x => x.Email == email);

            if (usuarioActual == null) return Unauthorized();

            usuarioActual.Nombre = entidad.Nombre ?? usuarioActual.Nombre;
            usuarioActual.Apellido = entidad.Apellido ?? usuarioActual.Apellido;
            usuarioActual.Dni = entidad.Dni ?? usuarioActual.Dni;
            usuarioActual.Email = entidad.Email ?? usuarioActual.Email;
            usuarioActual.Telefono = entidad.Telefono ?? usuarioActual.Telefono;

            await _context.SaveChangesAsync();
            return Ok(usuarioActual);
        }

        // Crear nuevo propietario usando el servicio
        [HttpPost("crear")]
        [AllowAnonymous]
        public async Task<IActionResult> Crear([FromBody] Propietario propietario)
        {
            if (propietario == null) return BadRequest("Propietario inválido");

            var creado = await _service.CrearPropietario(propietario);
            return Ok(creado);
        }

        // Editar contraseña usando el servicio
        [HttpPut("editarPassword")]
        public async Task<IActionResult> EditarPassword(int id, [FromBody] string nuevaPassword)
        {
            if (string.IsNullOrEmpty(nuevaPassword)) return BadRequest("Password no puede estar vacía");

            var resultado = await _service.EditarPassword(id, nuevaPassword);
            return resultado ? Ok("Password actualizada") : NotFound("Propietario no encontrado");
        }
    }
}
