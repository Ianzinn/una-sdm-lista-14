using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;
using CacauShowApi.Models;

namespace CacauShowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotesProducaoController : ControllerBase
{
    private readonly AppDbContext _db;
    public LotesProducaoController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.LotesProducao.Include(l => l.Produto).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lote = await _db.LotesProducao.Include(l => l.Produto).FirstOrDefaultAsync(l => l.Id == id);
        return lote is null ? NotFound() : Ok(lote);
    }

    // ─── A. Controle de Qualidade de Lote ────────────────────────────────────
    [HttpPost]
    public async Task<IActionResult> Create(LoteProducao lote)
    {
        // Valida existência do Produto
        var produtoExiste = await _db.Produtos.AnyAsync(p => p.Id == lote.ProdutoId);
        if (!produtoExiste)
            return NotFound($"Produto com Id {lote.ProdutoId} não encontrado.");

        // Valida data de fabricação
        if (lote.DataFabricacao > DateTime.UtcNow)
            return Conflict("Lote inválido: Data de fabricação não pode ser maior que a data atual.");

        // Status padrão ao criar
        lote.Status = "Em Produção";

        _db.LotesProducao.Add(lote);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = lote.Id }, lote);
    }

    // ─── C. Fluxo de Vida do Produto — PATCH status ───────────────────────────
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] AtualizarStatusDto dto)
    {
        var lote = await _db.LotesProducao.FindAsync(id);
        if (lote is null) return NotFound();

        // Regra de Ouro: Descartado não pode ir para Qualidade Aprovada ou Distribuído
        if (lote.Status == "Descartado" &&
            (dto.Status == "Qualidade Aprovada" || dto.Status == "Distribuído"))
        {
            return BadRequest(
                "Regra de negócio violada: Um lote Descartado não pode ser reativado para 'Qualidade Aprovada' ou 'Distribuído'.");
        }

        var statusValidos = new[] { "Em Produção", "Qualidade Aprovada", "Distribuído", "Descartado" };
        if (!statusValidos.Contains(dto.Status))
            return BadRequest($"Status inválido. Use um dos valores: {string.Join(", ", statusValidos)}");

        lote.Status = dto.Status;
        await _db.SaveChangesAsync();
        return Ok(lote);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var lote = await _db.LotesProducao.FindAsync(id);
        if (lote is null) return NotFound();
        _db.LotesProducao.Remove(lote);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record AtualizarStatusDto(string Status);
