using Microsoft.EntityFrameworkCore;
using WebApplicationAPI_Pedido.Models;

namespace WebApplicationAPI_Pedido.Data
{
    public class LojaContext : DbContext
    {
        public LojaContext(DbContextOptions<LojaContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<ItensPedido> ItensPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de chave estrangeira para ItensPedido
            modelBuilder.Entity<ItensPedido>()
                .HasOne(ip => ip.Pedido)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(ip => ip.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItensPedido>()
                .HasOne(ip => ip.Produto)
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
