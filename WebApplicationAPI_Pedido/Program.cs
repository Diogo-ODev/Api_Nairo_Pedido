using WebApplicationAPI_Pedido.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext com SQLite
builder.Services.AddDbContext<LojaContext>(options =>
    options.UseSqlite("Data Source=loja_clientes.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migrations automáticas (somente para fins educacionais)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LojaContext>();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    
app.UseAuthorization();

app.MapControllers();

app.Run();