using Microsoft.EntityFrameworkCore;
using WebApplicationAPI_Pedido.Models;

namespace WebApplicationAPI_Pedido.Data
{
    public class LojaContext : DbContext
    {
        public LojaContext(DbContextOptions<LojaContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
    }
}
