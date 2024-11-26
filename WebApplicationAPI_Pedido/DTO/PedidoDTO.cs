using System;
using System.Collections.Generic;

namespace WebApplicationAPI_Pedido.DTO
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NomeCliente { get; set; } // Nome do cliente diretamente no DTO
        public DateTime DataPedido { get; set; }
        public string Status { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
        public List<ItemPedidoDTO> ItensPedido { get; set; }
    }

    public class ItemPedidoDTO
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

}

