var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles(); // Serve index.html por padrão
app.UseStaticFiles(); // Permite servir arquivos da pasta wwwroot

app.MapGet("/sobre", async context => //Determinar que o arquivo index.html seja o padrão
{
await context.Response.SendFileAsync("wwwroot/sobre.html");
});

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SabordoBrasil.Services; // Certifique-se de que este namespace está correto

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext
builder.Services.AddDbContext<SeuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use o provider de banco de dados adequado

// Configuração do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<SeuDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Opções de senha
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
});

// Registro dos serviços
builder.Services.AddScoped<IPratoService, PratoService>(); // Registre a implementação de IPratoService
builder.Services.AddScoped<IComentarioService, ComentarioService>(); // Registre a implementação de IComentarioService

// Adicione os Controllers
builder.Services.AddControllers();

// Outras configurações...

var app = builder.Build();

// Configuração do pipeline de middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // Se você estiver usando autenticação
app.UseAuthorization();
app.MapControllers();

app.Run();