namespace Arena.Api.Application.Services
{
    public static class PlayerPatternTracker
    {
        public static int LowHpHeals { get; private set; }
        public static int LowHpAttacks { get; private set; }
        public static int BossHasUltDefends { get; private set; }
        public static int BossHasUltOtherActions { get; private set; }

        public static void RecordAction(string context, string action)
        {
            if (context == "LowHp") 
            {
                if (action == "Heal") LowHpHeals++; 
                else LowHpAttacks++;
            }
            else if (context == "HasUlt") 
            {
                if (action == "Defend") BossHasUltDefends++; 
                else BossHasUltOtherActions++;
            }
        }

        public static bool PredizCuraNoDesespero() => LowHpHeals > LowHpAttacks;
        public static bool PredizEscudoNoMedo() => BossHasUltDefends > BossHasUltOtherActions;
    }
}