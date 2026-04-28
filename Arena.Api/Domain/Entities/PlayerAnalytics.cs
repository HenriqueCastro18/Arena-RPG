namespace Arena.Api.Domain.Entities
{
    public class PlayerAnalytics
    {
        public int TotalAttacks { get; set; } = 0;
        public int TotalHeals { get; set; } = 0;
        public int TotalDefends { get; set; } = 0;
        public int TotalUlts { get; set; } = 0;

        // Heurísticas de comportamento
        public bool UsaCuraFrequente => TotalHeals > 2; 
        public bool UsaMuitoEscudo => TotalDefends > 2;
        
        public string ObterEstiloPredominante()
        {
            if (TotalAttacks > TotalDefends && TotalAttacks > TotalHeals) return "Agressivo";
            if (TotalDefends > TotalAttacks && TotalDefends > TotalHeals) return "Defensivo";
            return "Equilibrado";
        }
    }
}