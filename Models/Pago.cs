using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;  


namespace InmobiliariaAPI.Models;
public class Pago
{
    //	id	nroPago	idContrato	fecha	importe
    public int Id { get; set; }
    public int NroPago { get; set; }

    [ForeignKey("PagoContrato")]
    public int IdContrato { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Importe { get; set; }

    [JsonIgnore]
    public Pago? PagoContrato { get; set; }

}

