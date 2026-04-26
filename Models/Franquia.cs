namespace CacauShowApi.Models;

public class Franquia
{
    public int Id { get; set; }
    public string NomeLoja { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;

    /// <summary>Total de itens suportados no estoque da loja.</summary>
    public int CapacidadeEstoque { get; set; }
}
