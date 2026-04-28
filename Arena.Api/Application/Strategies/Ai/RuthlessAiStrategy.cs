using System;
using Arena.Api.Application.Services;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Domain.Strategies;

namespace Arena.Api.Application.Strategies.Ai
{
    public class RuthlessAiStrategy : IAiStrategy
    {
        private static readonly Random _random = new Random();

        public Arena.Api.Domain.Entities.AiDecision DecideNextMove(GameSession session)
        {
            bool canHeal = session.CurrentArenaEvent != "ToxicGas";
            bool canDefend = session.CurrentArenaEvent != "MagneticStorm" && session.MonsterShieldCooldown == 0 && session.MonsterShieldDurability > 0;
            bool heroIsShielding = session.HeroShieldDurability > 0 && session.HeroShieldCooldown == 0;

            // 1. SINERGIA DE ULTIMATES (A Maldade Específica de cada Monstro)
            if (session.MonsterUltCharge >= 2)
            {
                // A Caçadora ADORA quando tu metes o escudo, porque a Ult dela foi feita para o partir!
                if (session.Enemy.Name.Contains("Caçadora", StringComparison.OrdinalIgnoreCase) && heroIsShielding) {
                    session.CombatLog.Add("🦅 [Tática Letal] A Caçadora sorri ao ver o teu escudo... Era a armadilha perfeita!");
                    return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
                }
                
                // O Verme e o Xamã querem aleijar-te rápido (Roubar poção / Silenciar). Usam a Ult na primeira oportunidade!
                if (session.Enemy.Name.Contains("Verme", StringComparison.OrdinalIgnoreCase) || session.Enemy.Name.Contains("Xamã", StringComparison.OrdinalIgnoreCase)) {
                    return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
                }

                // Para os outros monstros: NÃO gastar a Ult se o Herói estiver de escudo levantado!
                if (heroIsShielding && session.Player.CurrentHp > session.CurrentHeroMaxHp * 0.3) {
                    // O Herói tem escudo, o monstro aproveita para curar, defender ou dar um ataque físico básico
                    if (canHeal && session.Enemy.CurrentHp < session.CurrentMonsterMaxHp * 0.6 && session.MonsterPotions > 0)
                        return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
                    
                    if (canDefend && _random.Next(100) < 50)
                        return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                        
                    return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
                }
                
                // Se o herói não tem escudo ou está em desespero, MANDA A ULT!
                return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
            }

            // 2. INSTINTO DE CAÇADOR E INTERCEÇÃO DE POÇÃO
            if (session.Player.CurrentHp <= session.CurrentHeroMaxHp * 0.25)
            {
                // O herói está a sangrar. Se tiver poções, de certeza que vai tentar curar-se.
                if (canDefend && session.HeroPotions > 0) {
                    // 60% de chance de o monstro ignorar o ataque e tentar interceptar e roubar a tua poção!
                    if (_random.Next(100) < 60) {
                        session.CombatLog.Add("🐺 [Instinto] O monstro fareja o teu desespero e tenta interceptar a tua cura!");
                        return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                    }
                }
                // Se não tentar roubar, ataca com tudo para finalizar!
                return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
            }

            // 3. SOBREVIVÊNCIA OPORTUNISTA
            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.40 && session.MonsterPotions > 0 && canHeal)
            {
                // Se houver Ventos Curativos, cura imediatamente para somar a vida. Senão, 85% de chance.
                if (session.CurrentArenaEvent == "HealingWinds" || _random.Next(100) < 85)
                    return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
            }

            // 4. DEFESA PREDITIVA CONTRA A TUA ULT
            if (canDefend && session.HeroUltCharge >= 2)
            {
                // O monstro vê que a tua Ultimate está pronta. 50% de chance de erguer o escudo por precaução.
                if (_random.Next(100) < 50) return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            // 5. ATAQUE PADRÃO E FRENESI
            if (session.CurrentArenaEvent == "BloodFrenzy")
                return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());

            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }
    }
}