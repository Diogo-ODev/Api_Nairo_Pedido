using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI_Pedido.Data;
using WebApplicationAPI_Pedido.Models;

namespace LojaClientesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItensPedidoController : ControllerBase
    {
        private readonly LojaContext _context;

        public ItensPedidoController(LojaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetItensPedido()
        {
            return Ok(_context.ItensPedido.ToList());
        }

        [HttpPost]
        public IActionResult PostItensPedido(ItensPedido item)
        {
            _context.ItensPedido.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetItensPedido), new { id = item.Id }, item);
        }
    }
}
