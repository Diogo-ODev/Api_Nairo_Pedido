using System;
using System.Collections.Generic;

namespace WebApplicationAPI_Pedido.DTO
{
    public class PedidoDTO
    {
        public int ClienteId { get; set; }
        public string Observacoes { get; set; }
        public List<ItemPedidoDTO> ItensPedido { get; set; }
    }

    public class ItemPedidoDTO
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}

