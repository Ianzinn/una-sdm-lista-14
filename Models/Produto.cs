namespace CacauShowApi.Models;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    /// <summary>Gourmet | Linha Regular | Sazonal</summary>
    public string Tipo { get; set; } = string.Empty;

    public decimal PrecoBase { get; set; }
}
