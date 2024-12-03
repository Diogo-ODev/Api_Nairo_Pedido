using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI_Pedido.Data;
using WebApplicationAPI_Pedido.DTO;
using WebApplicationAPI_Pedido.Models;

namespace LojaClientesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly LojaContext _context;

        public PedidosController(LojaContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetPedido(int id)
        {
            // Carregar o pedido com os relacionamentos necessários
            var pedido = _context.Pedidos
                .Include(p => p.Cliente) // Carrega o cliente relacionado
                .Include(p => p.ItensPedido) // Carrega os itens do pedido
                    .ThenInclude(ip => ip.Produto) // Carrega os produtos relacionados aos itens
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound(new { Message = $"Pedido com ID {id} não encontrado." });
            }

            // Mapear a entidade Pedido para o DTO PedidoDTO
            var pedidoDTO = new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = pedido.Cliente != null ? pedido.Cliente.Nome : "Cliente não encontrado", // Prevenção de null
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes,
                ItensPedido = pedido.ItensPedido.Select(ip => new ItemPedidoDTO
                {
                    ProdutoId = ip.ProdutoId,
                    NomeProduto = ip.Produto != null ? ip.Produto.Nome : "Produto não encontrado", // Prevenção de null
                    Quantidade = ip.Quantidade,
                    PrecoUnitario = ip.PrecoUnitario
                }).ToList()
            };

            return Ok(pedidoDTO);
        }


        [HttpPost]
        public IActionResult PostPedido(PedidoRequest pedidoRequest)
        {
            if (pedidoRequest == null || pedidoRequest.ClienteId <= 0)
            {
                return BadRequest("Dados do pedido inválidos.");
            }

            // Verifica se o cliente existe
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == pedidoRequest.ClienteId);
            if (cliente == null)
            {
                return NotFound(new { Message = "Cliente não encontrado." });
            }

            // Lista para acumular erros de estoque
            var errosEstoque = new List<string>();

            // Itera pelos itens do pedido para verificar estoque e reduzir
            var itensPedido = new List<ItensPedido>();
            foreach (var itemRequest in pedidoRequest.ItensPedido)
            {
                var produto = _context.Produtos.FirstOrDefault(p => p.Id == itemRequest.ProdutoId);
                if (produto == null)
                {
                    return NotFound(new { Message = $"Produto com ID {itemRequest.ProdutoId} não encontrado." });
                }

                // Verifica se há estoque suficiente
                if (produto.Estoque < itemRequest.Quantidade)
                {
                    errosEstoque.Add($"Estoque insuficiente para o produto {produto.Nome}. Estoque disponível: {produto.Estoque}, solicitado: {itemRequest.Quantidade}");
                    continue;
                }

                // Reduz o estoque do produto
                produto.Estoque -= itemRequest.Quantidade;

                // Adiciona o item ao pedido
                itensPedido.Add(new ItensPedido
                {
                    ProdutoId = itemRequest.ProdutoId,
                    Quantidade = itemRequest.Quantidade,
                    PrecoUnitario = produto.Preco // Preenche automaticamente o preço do produto
                });
            }

            // Se houver erros de estoque, retorna uma resposta indicando os problemas
            if (errosEstoque.Any())
            {
                return BadRequest(new { Message = "Não foi possível processar o pedido.", Erros = errosEstoque });
            }

            // Cria o pedido
            var pedido = new Pedido
            {
                ClienteId = pedidoRequest.ClienteId,
                Observacoes = pedidoRequest.Observacoes,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                ItensPedido = itensPedido,
                ValorTotal = itensPedido.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            // Adiciona o pedido ao banco de dados
            _context.Pedidos.Add(pedido);

            // Salva as alterações no banco (inclui atualização do estoque)
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

    }
}
