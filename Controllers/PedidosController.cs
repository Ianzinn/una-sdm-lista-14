using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PedidosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Pedidos
            .Include(p => p.Unidade)
            .Include(p => p.Produto)
            .ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pedido = await _db.Pedidos
            .Include(p => p.Unidade)
            .Include(p => p.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    // ─── B. Sistema de Venda de Franquia ─────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Create(Pedido pedido)
    {
        // Verifica existência da Franquia (Unidade)
        var franquia = await _db.Franquias.FindAsync(pedido.UnidadeId);
        if (franquia is null)
            return NotFound($"Franquia (Unidade) com Id {pedido.UnidadeId} não encontrada.");

        // Verifica existência do Produto
        var produto = await _db.Produtos.FindAsync(pedido.ProdutoId);
        if (produto is null)
            return NotFound($"Produto com Id {pedido.ProdutoId} não encontrado.");

        // 1. Validação de Estoque — soma de itens já pedidos para a unidade
        var totalItensNaUnidade = await _db.Pedidos
            .Where(p => p.UnidadeId == pedido.UnidadeId)
            .SumAsync(p => (int?)p.Quantidade) ?? 0;

        if (totalItensNaUnidade + pedido.Quantidade > franquia.CapacidadeEstoque)
            return BadRequest("Capacidade logística da loja excedida. Não é possível receber mais produtos.");

        // 2. Lógica de Promoção Sazonal
        decimal valorTotal = produto.PrecoBase * pedido.Quantidade;

        if (produto.Tipo == "Sazonal")
        {
            Console.WriteLine("🍫 Produto sazonal detectado: Adicionando embalagem de presente premium!");
            valorTotal += 15.00m;
        }

        pedido.ValorTotal = valorTotal;
        pedido.Produto = null;   // evita tracking duplicado pelo EF
        pedido.Unidade = null;

        _db.Pedidos.Add(pedido);
        await _db.SaveChangesAsync();

        // Recarrega com includes para retorno
        var criado = await _db.Pedidos
            .Include(p => p.Unidade)
            .Include(p => p.Produto)
            .FirstAsync(p => p.Id == pedido.Id);

        return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, criado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pedido = await _db.Pedidos.FindAsync(id);
        if (pedido is null) return NotFound();
        _db.Pedidos.Remove(pedido);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
