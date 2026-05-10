namespace Arena.Api.Application.Services
{
    public static class PlayerPatternTracker
    {
        // === Rastreamento por contexto (HP baixo) ===
        public static int LowHpHeals   { get; private set; }
        public static int LowHpAttacks { get; private set; }
        public static int LowHpDefends { get; private set; }

        // === Rastreamento por contexto (boss com Ultimate) ===
        public static int BossHasUltDefends      { get; private set; }
        public static int BossHasUltOtherActions { get; private set; }
        public static int BossHasUltUltimateUses { get; private set; }

        // === Frequência geral de ações ===
        public static int TotalActions    { get; private set; }
        public static int PhysicalAttacks { get; private set; }
        public static int UltimateActions { get; private set; }
        public static int HealActions     { get; private set; }
        public static int DefendActions   { get; private set; }

        // === Sequência das últimas 3 ações ===
        public static string LastAction       { get; private set; } = "None";
        public static string SecondLastAction { get; private set; } = "None";
        public static string ThirdLastAction  { get; private set; } = "None";

        // === Padrão pós-cura ===
        public static int AttacksAfterHeal { get; private set; }
        public static int DefendsAfterHeal { get; private set; }

        // === Agressividade consecutiva ===
        public static int CurrentConsecutiveAttacks { get; private set; }
        public static int MaxConsecutiveAttacks     { get; private set; }

        public static void RecordAction(string context, string action)
        {
            // Padrão pós-cura: analisar antes de atualizar o histórico
            if (LastAction == "Heal")
            {
                if (action == "Physical" || action == "Attack" || action == "Ultimate")
                    AttacksAfterHeal++;
                else if (action == "Defend")
                    DefendsAfterHeal++;
            }

            // Atualizar sequência
            ThirdLastAction  = SecondLastAction;
            SecondLastAction = LastAction;
            LastAction       = action;

            TotalActions++;

            // Contagem por tipo de ação
            switch (action)
            {
                case "Physical":
                case "Attack":
                    PhysicalAttacks++;
                    CurrentConsecutiveAttacks++;
                    if (CurrentConsecutiveAttacks > MaxConsecutiveAttacks)
                        MaxConsecutiveAttacks = CurrentConsecutiveAttacks;
                    break;
                case "Ultimate":
                    UltimateActions++;
                    CurrentConsecutiveAttacks = 0;
                    break;
                case "Heal":
                    HealActions++;
                    CurrentConsecutiveAttacks = 0;
                    break;
                case "Defend":
                    DefendActions++;
                    CurrentConsecutiveAttacks = 0;
                    break;
                default:
                    CurrentConsecutiveAttacks = 0;
                    break;
            }

            // Rastreamento por contexto
            if (context == "LowHp")
            {
                if (action == "Heal")         LowHpHeals++;
                else if (action == "Defend")  LowHpDefends++;
                else                          LowHpAttacks++;
            }
            else if (context == "HasUlt")
            {
                if (action == "Defend")          BossHasUltDefends++;
                else if (action == "Ultimate")   BossHasUltUltimateUses++;
                else                             BossHasUltOtherActions++;
            }
        }

        // === Predições ===

        // Jogador cura quando está com HP baixo?
        public static bool PredizCuraNoDesespero() => LowHpHeals > LowHpAttacks;

        // Jogador usa escudo quando boss tem Ultimate?
        public static bool PredizEscudoNoMedo() => BossHasUltDefends > BossHasUltOtherActions;

        // Jogador é agressivo (>55% das ações são ataques ou ultimates)?
        public static bool PredizJogadorAgressivo() =>
            TotalActions > 4 && (PhysicalAttacks + UltimateActions) > TotalActions * 55 / 100;

        // Jogador é defensivo (>25% das ações são defesas)?
        public static bool PredizJogadorDefensivo() =>
            TotalActions > 4 && DefendActions > TotalActions * 25 / 100;

        // Jogador usa a própria Ultimate quando o boss tem Ultimate (não tem medo)?
        public static bool PredizUltimateBruta() =>
            BossHasUltUltimateUses > 0 && BossHasUltUltimateUses >= BossHasUltDefends;

        // Jogador ataca logo após se curar (comportamento previsível pós-cura)?
        public static bool PredizAtaqueAposDefesa() =>
            DefendActions > 2 && AttacksAfterHeal > DefendsAfterHeal;

        // Pontuação de agressividade: 0.0 = muito passivo, 1.0 = muito agressivo
        public static float GetAggressivenessScore() =>
            TotalActions > 0 ? (float)(PhysicalAttacks + UltimateActions) / TotalActions : 0.5f;

        // Previsão de próxima ação com base em sequência e tendências
        public static string PredizProximaAcao()
        {
            // Se o jogador repetiu a mesma ação 2 vezes seguidas, provavelmente vai mudar
            if (LastAction == SecondLastAction && LastAction != "None")
            {
                bool isAttack = LastAction == "Attack" || LastAction == "Physical" || LastAction == "Ultimate";
                return isAttack ? "Defend" : "Attack";
            }

            if (PredizJogadorAgressivo())  return "Attack";
            if (PredizJogadorDefensivo())  return "Defend";

            // Ação mais frequente como fallback
            if (DefendActions >= PhysicalAttacks && DefendActions >= HealActions)  return "Defend";
            if (HealActions   >= PhysicalAttacks && HealActions   >= DefendActions) return "Heal";
            return "Attack";
        }
    }
}
