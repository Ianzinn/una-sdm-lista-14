using Microsoft.EntityFrameworkCore;
using CacauShowApi.Models;

namespace CacauShowApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Franquia> Franquias => Set<Franquia>();
    public DbSet<LoteProducao> LotesProducao => Set<LoteProducao>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
}
