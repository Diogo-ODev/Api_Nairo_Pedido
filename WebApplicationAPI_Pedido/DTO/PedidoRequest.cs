namespace WebApplicationAPI_Pedido.DTO
{
    public class PedidoRequest
    {
        public int ClienteId { get; set; } // Apenas o ID do cliente será enviado
        public string Observacoes { get; set; } // Observações opcionais
        public List<ItemPedidoRequest> ItensPedido { get; set; } // Lista de itens do pedido
    }

    public class ItemPedidoRequest
    {
        public int ProdutoId { get; set; } // Apenas o ID do produto será enviado
        public int Quantidade { get; set; } // Quantidade do produto no pedido
    }

}
