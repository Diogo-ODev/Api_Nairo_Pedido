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
            var pedido = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.ItensPedido)
                    .ThenInclude(ip => ip.Produto)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Mapear para DTO
            var pedidoDTO = new PedidoDTO
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                NomeCliente = pedido.Cliente?.Nome,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes,
                ItensPedido = pedido.ItensPedido.Select(ip => new ItemPedidoDTO
                {
                    ProdutoId = ip.ProdutoId,
                    NomeProduto = ip.Produto?.Nome,
                    Quantidade = ip.Quantidade,
                    PrecoUnitario = ip.PrecoUnitario
                }).ToList()
            };

            return Ok(pedidoDTO);
        }

        [HttpPost]
        public IActionResult PostPedido(PedidoDTO pedidoDTO)
        {
            if (pedidoDTO == null || pedidoDTO.ClienteId <= 0)
            {
                return BadRequest("Dados do pedido inválidos.");
            }

            // Mapear os dados do DTO para a entidade Pedido
            var pedido = new Pedido
            {
                ClienteId = pedidoDTO.ClienteId,
                Observacoes = pedidoDTO.Observacoes,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                ItensPedido = pedidoDTO.ItensPedido.Select(item => new ItensPedido
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };

            // Calcular valor total
            pedido.ValorTotal = pedido.ItensPedido.Sum(item => item.Quantidade * item.PrecoUnitario);

            // Adicionar ao banco de dados
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

    }
}
