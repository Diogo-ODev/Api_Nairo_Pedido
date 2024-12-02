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

            // Buscar o cliente pelo ID
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == pedidoRequest.ClienteId);
            if (cliente == null)
            {
                return NotFound(new { Message = "Cliente não encontrado." });
            }

            // Criar o Pedido
            var pedido = new Pedido
            {
                ClienteId = pedidoRequest.ClienteId,
                Observacoes = pedidoRequest.Observacoes,
                DataPedido = DateTime.UtcNow,
                Status = "Pendente", // Status padrão
                ItensPedido = pedidoRequest.ItensPedido.Select(item => new ItensPedido
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = _context.Produtos
                        .Where(p => p.Id == item.ProdutoId)
                        .Select(p => p.Preco)
                        .FirstOrDefault()
                }).ToList()
            };

            // Calcular o Valor Total
            pedido.ValorTotal = pedido.ItensPedido.Sum(item => item.Quantidade * item.PrecoUnitario);

            // Adicionar ao banco de dados
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            // Retornar o DTO com NomeCliente preenchido
            var pedidoDTO = new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = cliente.Nome, // Preenchido aqui com base no cliente carregado
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes,
                ItensPedido = pedido.ItensPedido.Select(ip => new ItemPedidoDTO
                {
                    ProdutoId = ip.ProdutoId,
                    NomeProduto = _context.Produtos.FirstOrDefault(p => p.Id == ip.ProdutoId)?.Nome,
                    Quantidade = ip.Quantidade,
                    PrecoUnitario = ip.PrecoUnitario
                }).ToList()
            };

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedidoDTO);
        }


    }
}
