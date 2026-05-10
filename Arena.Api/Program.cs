using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Infrastructure.Firebase;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddControllers();
builder.Services.AddSingleton<GameManager>();

var firebaseCreds     = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
var firebaseProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");

if (!string.IsNullOrWhiteSpace(firebaseCreds) && !string.IsNullOrWhiteSpace(firebaseProjectId))
    builder.Services.AddSingleton<ITrainingLogService>(
        new FirestoreTrainingLogService(firebaseProjectId, firebaseCreds));
else
    builder.Services.AddSingleton<ITrainingLogService, NullTrainingLogService>();

builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend",
        p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors("AllowFrontend");
app.MapControllers();
app.MapGet("/", () => $"Arena API está online na porta {port}!");
app.Run();
