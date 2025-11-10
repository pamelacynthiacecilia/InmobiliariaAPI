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
    public class InmuebleController : ControllerBase
    {
        private readonly Context _context;
        private readonly IConfiguration _config;

       private readonly IWebHostEnvironment _environment;

        public InmuebleController(Context context, IConfiguration config, IWebHostEnvironment environment)
        {
            _context = context;
            _config = config;
            _environment = environment;
        }

        [HttpGet("inmueblesDelPropietario")]
        public async Task<ActionResult> InmueblesDelPropietario()
        {
            try
            {
                var usuario = User.Identity.Name;

                var inmuebles = await _context.Inmuebles
                                .Where(e => e.PropietarioInmueble.Email == usuario)
                                .Select(e => new
                                {
                                    e.Id,
                                    e.Direccion,
                                    e.Estado,
                                    e.Tipo,
                                    e.Uso,
                                    e.Precio,
                                    e.Ambientes,
                                    e.ImageUrl,
                                    e.IdPropietario
                                })
                                .ToListAsync();

                return Ok(inmuebles);
            }
            catch (Exception ex)
            {
                return BadRequest(new {
                    error = "Error al obtener los inmuebles.",
                    detalle = ex.Message
                });
            }
}


        [HttpGet("obtenerInmueblePorId/{id}")]
        public async Task<IActionResult> ObtenerInmueblePorId(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                return Ok(_context.Inmuebles.Include(e => e.PropietarioInmueble).Where(e => e.PropietarioInmueble.Email == usuario).Single(e => e.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

[HttpPut("modificarEstadoInmueble")]
public async Task<IActionResult> ModificarEstadoInmueble(int id, [FromBody] Inmueble entidad)
{
    try
    {
        var usuario = User.Identity.Name;

        var inmueble = _context.Inmuebles
            .Include(i => i.PropietarioInmueble)
            .FirstOrDefault(x => x.Id == id && x.PropietarioInmueble.Email == usuario);

        if (inmueble == null)
            return NotFound(new { error = "Inmueble no encontrado o no pertenece al usuario." });

        // uso estado del body
        inmueble.Estado = entidad.Estado;

        _context.Inmuebles.Update(inmueble);
        await _context.SaveChangesAsync();

        return Ok(new {
            mensaje = "Estado actualizado correctamente",
            id = inmueble.Id,
            estado = inmueble.Estado
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new {
            error = "Error al modificar el estado.",
            detalle = ex.Message,
            stack = ex.StackTrace
        });
    }
}




        /*  [HttpPut("modificarEstadoInmueble")]
          public async Task<IActionResult> ModificarEstadoInmueble(int id, [FromBody] Inmueble entidad)
          {
              try
              {
                  var usuario = User.Identity.Name;

                  var inmueble = _context.Inmuebles
                      .Include(i => i.PropietarioInmueble)
                      .FirstOrDefault(x => x.Id == id && x.PropietarioInmueble.Email == usuario);

                  if (inmueble == null)
                      return NotFound(new { error = "Inmueble no encontrado o no pertenece al usuario." });

                  // Actualizar estado
                  inmueble.Estado = inmueble.Estado == 1 ? 2 : 1;

                  _context.Inmuebles.Update(inmueble);
                  await _context.SaveChangesAsync();

                  return Ok(new {
                      mensaje = "Estado actualizado correctamente",
                      id = inmueble.Id,
                      estado = inmueble.Estado
                  });
              }
              catch (Exception ex)
              {
                  return BadRequest(new {
                      error = "Error al modificar el estado.",
                      detalle = ex.Message,
                      stack = ex.StackTrace
                  });
              }
          }
  */

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = _context.Inmuebles.Include(e => e.PropietarioInmueble).FirstOrDefault(e => e.Id == id && e.PropietarioInmueble.Email == User.Identity.Name);
                if (entidad != null)
                {
                    _context.Inmuebles.Remove(entidad);
                    _context.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("crearInmueble")]
        public async Task<IActionResult> CrearInmueble([FromForm] Inmueble entidad)
{
    try
    {
        // Verificar autenticación
        var email = User?.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Unauthorized(new { error = "Usuario no autenticado." });

        // Buscar propietario
        var propietario = _context.Propietarios.FirstOrDefault(p => p.Email == email);
        if (propietario == null)
            return BadRequest(new { error = "No se encontró el propietario con ese email." });

        // Asignar propietario al inmueble
        entidad.IdPropietario = propietario.Id;

        // Guardar inmueble para obtener el Id
        _context.Inmuebles.Add(entidad);
        await _context.SaveChangesAsync();

        // Si hay imagen, guardarla en /Uploads
        if (entidad.ImagenFile != null && entidad.Id > 0)
        {
            if (_environment.WebRootPath == null)
                return BadRequest(new { error = "Ruta del servidor no disponible." });

            string uploadsPath = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(entidad.ImagenFile.FileName);
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg"; // Valor por defecto

            string fileName = "inmueble_" + entidad.Id + extension;
            string fullPath = Path.Combine(uploadsPath, fileName);

            entidad.ImageUrl = Path.Combine("/Uploads", fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await entidad.ImagenFile.CopyToAsync(stream);
            }

            _context.Inmuebles.Update(entidad);
            await _context.SaveChangesAsync();
        }

        // Respuesta final
        return Ok(new {
            mensaje = "Inmueble creado correctamente",
            id = entidad.Id,
            imagen = entidad.ImageUrl
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new {
            error = "Ocurrió un error al procesar el inmueble.",
            detalle = ex.Message,
            stack = ex.StackTrace
        });
    }
}



    }

}