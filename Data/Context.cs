using InmobiliariaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaAPI.Data
{
    public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
            {
            
        }

    public DbSet<Propietario> Propietarios { get; set; }
    public DbSet<Inquilino> Inquilinos { get; set; }
    public DbSet<Inmueble> Inmuebles { get; set; }
    public DbSet<Contrato> Contratos { get; set; }
    public DbSet<Pago> Pagos { get; set; }
}

}
