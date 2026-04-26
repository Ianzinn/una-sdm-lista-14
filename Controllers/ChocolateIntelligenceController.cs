using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/intelligence")]
public class ChocolateIntelligenceController : ControllerBase
{
    private readonly AppDbContext _db;
    public ChocolateIntelligenceController(AppDbContext db) => _db = db;

    /// <summary>
    /// Retorna a soma de itens vendidos agrupada por Cidade.
    /// Simula latência de consulta a servidor central de logística (2 segundos).
    /// </summary>
    [HttpGet("estoque-regional")]
    public async Task<IActionResult> EstoqueRegional()
    {
        // Simula latência de consulta a servidor central de logística pesada
        Thread.Sleep(2000);

        var resultado = await _db.Pedidos
            .Include(p => p.Unidade)
            .GroupBy(p => p.Unidade!.Cidade)
            .Select(g => new
            {
                Cidade = g.Key,
                TotalItensVendidos = g.Sum(p => p.Quantidade)
            })
            .OrderByDescending(x => x.TotalItensVendidos)
            .ToListAsync();

        return Ok(resultado);
    }
}
