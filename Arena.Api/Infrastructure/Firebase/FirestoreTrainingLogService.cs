using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Infrastructure.Firebase
{
    public sealed class FirestoreTrainingLogService : ITrainingLogService
    {
        private readonly FirestoreDb _db;

        public FirestoreTrainingLogService(string projectId, string credentialsJson)
        {
            var credential = GoogleCredential.FromJson(credentialsJson);
            var client = new FirestoreClientBuilder { Credential = credential }.Build();
            _db = FirestoreDb.Create(projectId, client);
        }

        public async Task SaveAsync(TrainingReport report)
        {
            var doc = new Dictionary<string, object>
            {
                ["data"]        = report.Data,
                ["batalha"]     = report.Batalha,
                ["vencedor"]    = report.Vencedor,
                ["totalTurnos"] = report.TotalTurnos,
                ["cerebroDaIA"] = BuildAiDict(report.CerebroDaIA),
                ["historico"]   = report.HistoricoDeTurnos
            };

            await _db.Collection("treinos").AddAsync(doc);
        }

        private static Dictionary<string, object> BuildAiDict(AiReport ai) => new()
        {
            ["curaNoDesespero"]         = ai.VezesQueOJogadorCurouNoDesespero,
            ["ataqueNoDesespero"]       = ai.VezesQueOJogadorAtacouNoDesespero,
            ["defesaNoDesespero"]       = ai.VezesQueOJogadorDefendeuNoDesespero,
            ["preveCuraNoDesespero"]    = ai.Conclusao_CurarNoDesespero,
            ["defendeuDaUlt"]           = ai.VezesQueOJogadorDefendeuDaUlt,
            ["ignorouUlt"]              = ai.VezesQueOJogadorIgnorouAUlt,
            ["usouUltContraUlt"]        = ai.VezesQueOJogadorUsouUltContraUlt,
            ["preveEscudoNoMedo"]       = ai.Conclusao_EscudoNoMedo,
            ["preveUltimateBruta"]      = ai.Conclusao_UltimateSemMedo,
            ["totalAcoes"]              = ai.TotalAcoes,
            ["ataques"]                 = ai.Ataques,
            ["ultimates"]               = ai.Ultimates,
            ["curas"]                   = ai.Curas,
            ["defesas"]                 = ai.Defesas,
            ["pontuacaoAgressividade"]  = (double)ai.PontuacaoAgressividade,
            ["jogadorAgressivo"]        = ai.Conclusao_JogadorAgressivo,
            ["jogadorDefensivo"]        = ai.Conclusao_JogadorDefensivo,
            ["maxAtaquesConsecutivos"]  = ai.MaxAtaquesConsecutivos,
            ["ultimaAcao"]              = ai.UltimaAcaoRegistada
        };
    }
}
