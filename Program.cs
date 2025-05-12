using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários
builder.Services.AddControllers();

var app = builder.Build();

// Configura o pipeline de middleware
app.UseDefaultFiles(); // Serve index.html por padrão
app.UseStaticFiles(); // Permite servir arquivos da pasta wwwroot
app.UseStaticFiles(); // Adiciona novamente o middleware para servir arquivos estáticos
app.UseRouting(); // Configura o roteamento

// Mapeia os controladores
app.MapControllers();

// Remove o MapGet desnecessário ou ajusta para servir index.html corretamente
app.MapGet("/", async context =>
{
    await context.Response.SendFileAsync("wwwroot/index.html");
});

// Inicia o aplicativo
app.Run();