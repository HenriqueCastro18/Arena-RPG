using Arena.Api.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DE PORTA PARA O RAILWAY ---
// O Railway define a porta na variável "PORT". Se não existir (local), usa 8080.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Adiciona os Controllers
builder.Services.AddControllers();

// 1. INJEÇÃO DE DEPENDÊNCIA
// Mantém o GameManager na memória para processar a lógica da IA
builder.Services.AddSingleton<GameManager>();

// 2. CONFIGURAÇÃO DE CORS (SOLUÇÃO PARA O ERRO DO VERCEL)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin() // Permite que qualquer link do Vercel aceda à API
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Ativa o CORS antes de mapear os controllers
app.UseCors("AllowFrontend");

// Mapeia as rotas dos Controllers (Ex: /api/game)
app.MapControllers();

// Rota de teste: se abrires o link do Railway no browser, verás esta mensagem
app.MapGet("/", () => $"Arena API está online na porta {port}!");

app.Run();
