namespace CacauShowApi.Models;

public class LoteProducao
{
    public int Id { get; set; }

    /// <summary>Ex.: BATCH-2026-X</summary>
    public string CodigoLote { get; set; } = string.Empty;

    public DateTime DataFabricacao { get; set; }

    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    /// <summary>Em Produção | Qualidade Aprovada | Distribuído | Descartado</summary>
    public string Status { get; set; } = "Em Produção";
}
