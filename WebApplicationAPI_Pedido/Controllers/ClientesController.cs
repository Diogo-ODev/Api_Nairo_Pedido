using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI_Pedido.Data;
using WebApplicationAPI_Pedido.Models;

namespace LojaClientesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly LojaContext _context;

        public ClientesController(LojaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetClientes()
        {
            return Ok(_context.Clientes.ToList());
        }

        [HttpPost]
        public IActionResult PostCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetClientes), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public IActionResult PutCliente(int id, Cliente clienteAtualizado)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null) return NotFound();

            cliente.Nome = clienteAtualizado.Nome;
            cliente.Sobrenome = clienteAtualizado.Sobrenome;
            cliente.Email = clienteAtualizado.Email;
            cliente.Telefone = clienteAtualizado.Telefone;
            cliente.Endereco = clienteAtualizado.Endereco;
            cliente.Cidade = clienteAtualizado.Cidade;
            cliente.Estado = clienteAtualizado.Estado;
            cliente.CEP = clienteAtualizado.CEP;

            _context.SaveChanges();
            return NoContent();
        }
    }
}
