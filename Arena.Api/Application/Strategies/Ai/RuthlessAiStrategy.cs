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
            bool canHeal = session.CurrentArenaEvent != "ToxicGas" && session.MonsterPotions > 0;
            bool canDefend = session.CurrentArenaEvent != "MagneticStorm" && session.MonsterShieldCooldown == 0 && session.MonsterShieldDurability > 0;
            bool canDodge = session.MonsterDodgesLeft > 0;
            bool heroIsShielding = session.HeroShieldDurability > 0 && session.HeroShieldCooldown == 0;
            bool heroUltReady = session.HeroUltCharge >= 2;

            // 1. SINERGIA DE ULTIMATES
            if (session.MonsterUltCharge >= 3)
            {
                // A Caçadora adora quando o escudo está levantado
                if (session.Enemy.Name.Contains("Caçadora", StringComparison.OrdinalIgnoreCase) && heroIsShielding) {
                    session.CombatLog.Add("🦅 [Tática Letal] A Caçadora sorri ao ver o teu escudo... Era a armadilha perfeita!");
                    return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
                }

                // Verme e Xamã usam ult na primeira oportunidade
                if (session.Enemy.Name.Contains("Verme", StringComparison.OrdinalIgnoreCase) || session.Enemy.Name.Contains("Xamã", StringComparison.OrdinalIgnoreCase))
                    return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());

                // Para outros monstros: evitar usar ult se herói tem escudo levantado
                if (heroIsShielding && session.Player.CurrentHp > session.CurrentHeroMaxHp * 0.3) {
                    if (canHeal && session.Enemy.CurrentHp < session.CurrentMonsterMaxHp * 0.6)
                        return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
                    if (canDodge && heroUltReady && _random.Next(100) < 55) {
                        session.CombatLog.Add("💨 [Tática] O monstro esquivou para preservar a Ultimate para o momento certo!");
                        return new Arena.Api.Domain.Entities.AiDecision("Dodge", null);
                    }
                    if (canDefend && _random.Next(100) < 50)
                        return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                    return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
                }

                return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
            }

            // 2. ESQUIVA PREVENTIVA CONTRA ULT DO HERÓI
            if (canDodge && heroUltReady && _random.Next(100) < 40) {
                session.CombatLog.Add("💨 [Instinto] O monstro sentiu o perigo e recuou numa esquiva ágil!");
                return new Arena.Api.Domain.Entities.AiDecision("Dodge", null);
            }

            // 3. INSTINTO DE CAÇADOR: herói a sangrar com poções
            if (session.Player.CurrentHp <= session.CurrentHeroMaxHp * 0.30)
            {
                if (canDefend && session.HeroPotions > 0) {
                    if (_random.Next(100) < 65) {
                        session.CombatLog.Add("🐺 [Instinto] O monstro fareja o teu desespero e tenta interceptar a tua cura!");
                        return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                    }
                }
                return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
            }

            // 4. SOBREVIVÊNCIA OPORTUNISTA
            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.45 && canHeal) {
                if (session.CurrentArenaEvent == "HealingWinds" || _random.Next(100) < 85)
                    return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
            }

            // 5. DEFESA PREDITIVA CONTRA ULT
            if (canDefend && heroUltReady) {
                if (_random.Next(100) < 50) return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            // 6. ATAQUE PADRÃO
            if (session.CurrentArenaEvent == "BloodFrenzy")
                return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());

            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }
    }
}
