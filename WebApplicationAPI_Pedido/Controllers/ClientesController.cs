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
            if (id != clienteAtualizado.Id)
            {
                return BadRequest("ID do cliente não corresponde ao informado na URL.");
            }

            // Busca o cliente no banco de dados
            var clienteExistente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (clienteExistente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            // Atualiza apenas os campos não vazios
            clienteExistente.Nome = !string.IsNullOrWhiteSpace(clienteAtualizado.Nome) ? clienteAtualizado.Nome : clienteExistente.Nome;
            clienteExistente.Sobrenome = !string.IsNullOrWhiteSpace(clienteAtualizado.Sobrenome) ? clienteAtualizado.Sobrenome : clienteExistente.Sobrenome;
            clienteExistente.Email = !string.IsNullOrWhiteSpace(clienteAtualizado.Email) ? clienteAtualizado.Email : clienteExistente.Email;
            clienteExistente.Telefone = !string.IsNullOrWhiteSpace(clienteAtualizado.Telefone) ? clienteAtualizado.Telefone : clienteExistente.Telefone;
            clienteExistente.Endereco = !string.IsNullOrWhiteSpace(clienteAtualizado.Endereco) ? clienteAtualizado.Endereco : clienteExistente.Endereco;
            clienteExistente.Cidade = !string.IsNullOrWhiteSpace(clienteAtualizado.Cidade) ? clienteAtualizado.Cidade : clienteExistente.Cidade;
            clienteExistente.Estado = !string.IsNullOrWhiteSpace(clienteAtualizado.Estado) ? clienteAtualizado.Estado : clienteExistente.Estado;
            clienteExistente.CEP = !string.IsNullOrWhiteSpace(clienteAtualizado.CEP) ? clienteAtualizado.CEP : clienteExistente.CEP;

            // Não atualiza o dataCadastro, mantém o existente
            clienteExistente.DataCadastro = clienteExistente.DataCadastro;

            // Salva as alterações no banco de dados
            _context.SaveChanges();

            return Ok(clienteExistente);
        }

        [HttpDelete("clientes/{id}")]
        public IActionResult DeleteCliente(int id)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return Ok($"Cliente com ID {id} foi deletado com sucesso.");
        }

    }
}
