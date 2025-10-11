
namespace InmobiliariaAPI.Models;

public class Propietario
{
    //	db -> id	nombre	apellido  email	telefono dni password avatar	
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Dni { get; set; }
    public string? Password { get; set; }
    public string? Avatar { get; set; }

 
}