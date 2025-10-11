namespace InmobiliariaAPI.Models;

public class Contrato
{

    //id	precio	fechaInicio	fechaFin	idInquilino	idInmueble	
    public int Id { get; set; }
    public decimal Precio { get; set; }
    public DateTime FehaInicio { get; set; }
    public DateTime FehaFin { get; set; }

}