using System.Collections.Generic;

namespace Arena.Api.Domain.Entities
{
    public sealed record TrainingReport(
        string Data,
        string Batalha,
        string Vencedor,
        int TotalTurnos,
        AiReport CerebroDaIA,
        IReadOnlyList<string> HistoricoDeTurnos
    );

    public sealed record AiReport(
        int VezesQueOJogadorCurouNoDesespero,
        int VezesQueOJogadorAtacouNoDesespero,
        int VezesQueOJogadorDefendeuNoDesespero,
        bool Conclusao_CurarNoDesespero,
        int VezesQueOJogadorDefendeuDaUlt,
        int VezesQueOJogadorIgnorouAUlt,
        int VezesQueOJogadorUsouUltContraUlt,
        bool Conclusao_EscudoNoMedo,
        bool Conclusao_UltimateSemMedo,
        int TotalAcoes,
        int Ataques,
        int Ultimates,
        int Curas,
        int Defesas,
        float PontuacaoAgressividade,
        bool Conclusao_JogadorAgressivo,
        bool Conclusao_JogadorDefensivo,
        int MaxAtaquesConsecutivos,
        string UltimaAcaoRegistada
    );
}
