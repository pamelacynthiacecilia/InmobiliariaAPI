using InmobiliariaAPI.Interfaces;
using InmobiliariaAPI.Models;
using InmobiliariaAPI.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
//using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InmobiliariaAPI.Services
{
   public class PropietarioService : IPropietarioService
{
    private readonly Context _context;
    private readonly IConfiguration _config;


    public PropietarioService(Context context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<Propietario> CrearPropietario(Propietario propietario)
    {
        propietario.Password = await HashPassword(propietario.Password);
        await _context.Propietarios.AddAsync(propietario);
        await _context.SaveChangesAsync();
        return propietario;
    }

    public async Task<bool> EditarPassword(int id, string nuevaPassword)
    {
        var propietario = await _context.Propietarios.FindAsync(id);
        if (propietario == null) return false;

        propietario.Password = await HashPassword(nuevaPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> HashPassword(string password)
{
    string salt = _config["Salt"];
    return Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: Encoding.ASCII.GetBytes(salt),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 1000,
        numBytesRequested: 256 / 8));
}

    public async Task<Propietario> ObtenerPropietario(int id)
    {
        return await _context.Propietarios.FindAsync(id);
    }

    public async Task<bool> EliminarPropietario(int id)
    {
        var propietario = await _context.Propietarios.FindAsync(id);
        if (propietario == null) return false;

        _context.Propietarios.Remove(propietario);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActualizarPropietario(Propietario propietario)
    {
        var existingPropietario = await _context.Propietarios.FindAsync(propietario.Id);
        if (existingPropietario == null) return false;

        existingPropietario.Nombre = propietario.Nombre;
        existingPropietario.Apellido = propietario.Apellido;
        existingPropietario.Dni = propietario.Dni;
        existingPropietario.Email = propietario.Email;
        await _context.SaveChangesAsync();
        return true;

    }


}

}
