using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;  


namespace InmobiliariaAPI.Models;

//id	idPropietario	direccion	ambientes	tipo	uso	precio	estado
public class Inmueble
{
    public int Id { get; set; }

    [ForeignKey("PropietarioInmueble")]
    public int IdPropietario { get; set; }

    public string? Direccion { get; set; }
    public string? Ambientes { get; set; }
    public string? Tipo { get; set; }
    public string? Uso { get; set; }
    public decimal? Precio { get; set; }
    public int Estado { get; set; }
    public string? ImageUrl { get; set; }

    [JsonIgnore]
    public Propietario? PropietarioInmueble { get; set; }

    [NotMapped]
    [JsonIgnore]
    public IFormFile? ImagenFile { get; set; }

}