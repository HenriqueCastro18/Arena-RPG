using Arena.Api.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DE PORTA PARA O RAILWAY ---
// O Railway passa a porta na variável de ambiente "PORT"
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Adiciona os Controllers
builder.Services.AddControllers();

// 1. INJEÇÃO DE DEPENDÊNCIA
builder.Services.AddSingleton<GameManager>();

// 2. CORS AJUSTADO
// Substituí o "AnyOrigin" pelo link do seu Vercel para garantir estabilidade
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("https://arena-l6ucn9v3n-hccastro04-6092s-projects.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Ativa o CORS
app.UseCors("AllowFrontend");

// Configuração básica de rotas
app.MapControllers();

// Rota de teste para você verificar se a API está viva pelo navegador
app.MapGet("/", () => $"Arena API está online na porta {port}!");

app.Run();