using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI_Pedido.Data;
using WebApplicationAPI_Pedido.Models;

namespace LojaClientesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly LojaContext _context;

        public ProdutosController(LojaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProdutos()
        {
            return Ok(_context.Produtos.ToList());
        }

        [HttpPost]
        public IActionResult PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        [HttpDelete("produtos/{id}")]
        public IActionResult DeleteProduto(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok($"Produto com ID {id} foi deletado com sucesso.");
        }

    }
}
