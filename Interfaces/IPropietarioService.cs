using InmobiliariaAPI.Models;

namespace InmobiliariaAPI.Interfaces
{
    public interface IPropietarioService
    {
        Task<Propietario> CrearPropietario(Propietario propietario);
        Task<bool> EditarPassword(int id, string nuevaPassword);
        Task<Propietario> ObtenerPropietario(int id);
        Task<bool> EliminarPropietario(int id);
        Task<bool> ActualizarPropietario(Propietario propietario);
        Task<string> HashPassword(string password);

    }
}