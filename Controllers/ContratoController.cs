using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using InmobiliariaAPI.Data;
using Microsoft.EntityFrameworkCore;
using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Controllers
{

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ContratoController : ControllerBase
    {
        private readonly Context _context;
        private readonly IConfiguration _config;

        private readonly IWebHostEnvironment _environment;

        public ContratoController(Context context, IConfiguration config, IWebHostEnvironment environment)
        {
            _context = context;
            _config = config;
            _environment = environment;
        }



       [HttpGet("contratosDelInmueble/{id}")]
public async Task<ActionResult> contratosDelInmueble(int id)
{
    try
    {
        var email = User?.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Unauthorized(new { error = "Usuario no autenticado." });

        var contratos = await _context.Contratos
            .Include(c => c.InquilinoContrato)
            .Where(c => c.idInmueble == id)
            .ToListAsync();

        if (contratos == null || contratos.Count == 0)
            return NotFound(new { mensaje = "No hay contratos para este inmueble." });

        return Ok(contratos);
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = "Error al obtener contratos", detalle = ex.Message });
    }
}



    }
}