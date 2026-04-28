using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Services
{
    public static class AiDecisionService
    {
        public static string DecidirAcaoProPlayer(
            Character boss, 
            Character player, 
            int bossPotions, 
            int bossUltCharge, 
            int playerShieldDurability)
        {
            // 1. Prioridade Absoluta: Sobrevivência
            if (boss.CurrentHp < (boss.MaxHp * 0.25) && bossPotions > 0)
                return "Heal";

            // 2. Oportunismo: Executar o jogador se ele estiver com vida baixa e sem escudo
            if (player.CurrentHp < (player.MaxHp * 0.35) && bossUltCharge >= 2 && playerShieldDurability == 0)
                return "Ultimate";

            // 3. Leitura de Jogo: Se o jogador ativou escudo agora, não desperdice a ULT
            if (playerShieldDurability > 0)
            {
                if (bossPotions > 0 && boss.CurrentHp < boss.MaxHp) return "Heal";
                return "Physical"; // Dá um ataque básico só para gastar a durabilidade do escudo dele
            }

            // 4. Uso de recurso otimizado: Tem ULT e o cara tá sem escudo? Fogo nela.
            if (bossUltCharge >= 2)
                return "Ultimate";

            // Padrão
            return "Physical";
        }
    }
}