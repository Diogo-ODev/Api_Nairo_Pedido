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
                .Include(p => p.ItensPedido) // Inclui itens relacionados, se necessário
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            return Ok(pedido);
        }

        [HttpPost]
        public IActionResult PostPedido(PedidoDTO pedidoDTO)
        {
            if (pedidoDTO == null || pedidoDTO.ClienteId <= 0)
            {
                return BadRequest("Dados do pedido inválidos.");
            }

            // Criar o Pedido a partir do DTO
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

            // Calcular o ValorTotal do pedido
            pedido.ValorTotal = pedido.ItensPedido.Sum(item => item.Quantidade * item.PrecoUnitario);

            // Adicionar o pedido no banco
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }
    }
}
