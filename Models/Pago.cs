namespace InmobiliariaAPI.Models;

public class Pago
{
    //	id	nroPago	idContrato	fecha	importe
    public int Id { get; set; }
    public int NroPago { get; set; }
    public int IdContrato { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Importe { get; set; }
}

