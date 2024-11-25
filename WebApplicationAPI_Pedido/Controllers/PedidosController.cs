using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI_Pedido.Data;
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

        [HttpGet]
        public IActionResult GetPedidos()
        {
            return Ok(_context.Pedidos.ToList());
        }

        [HttpPost]
        public IActionResult PostPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Id }, pedido);
        }
    }
}
