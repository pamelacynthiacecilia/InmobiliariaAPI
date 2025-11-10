using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAPI.Models;

public class Contrato
{

    //id	precio	fechaInicio	fechaFin idInquilino	idInmueble	

    public int Id { get; set; }


    [ForeignKey("InquilinoContrato")]
    public int idInquilino { get; set; }

     [ForeignKey("InmuebleContrato")]
    public int idInmueble { get; set; }

    public decimal Precio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

     // [JsonIgnore]
    public Inquilino? InquilinoContrato { get; set; }
      [JsonIgnore]
    public Inmueble? InmuebleContrato { get; set; }

}