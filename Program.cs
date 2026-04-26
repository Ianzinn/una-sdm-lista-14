using Microsoft.EntityFrameworkCore;
using CacauShowApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Serviços ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "CacauShow Smart ERP API",
        Version = "v1",
        Description = "Ecossistema de micro-gerenciamento de estoques, produções sazonais, franquias e pedidos da CacauShow."
    });
});

// EF Core InMemory
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("CacauShowDb"));

var app = builder.Build();

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CacauShow Smart ERP v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000
});

app.UseAuthorization();
app.MapControllers();

app.Run();
