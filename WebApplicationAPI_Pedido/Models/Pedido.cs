using System;
using System.Collections.Generic;
namespace WebApplicationAPI_Pedido.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pendente";
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
        public List<ItensPedido> ItensPedido { get; set; } = new List<ItensPedido>();
    }
}
