using Arena.Api.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os Controllers
builder.Services.AddControllers();

// 1. INJEÇÃO DE DEPENDÊNCIA: Registra o GameManager como Singleton (uma única instância na memória)
builder.Services.AddSingleton<GameManager>();

// 2. CORS: Permite que o seu HTML (rodando em qualquer porta local) faça requisições para a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Ativa o CORS que configuramos acima
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();