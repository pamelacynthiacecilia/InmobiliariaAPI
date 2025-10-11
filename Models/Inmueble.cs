namespace InmobiliariaAPI.Models;

//id	idPropietario	direccion	ambientes	tipo	uso	precio	estado
public class Inmueble
{
    public int Id { get; set; }
    public int IdPropietario { get; set; }
    public string? Direccion { get; set; }
    public string? Ambientes { get; set; }
    public string? Tipo { get; set; }
    public string? Uso { get; set; }
    public string? Precio { get; set; }
    public bool Estado { get; set; }


}