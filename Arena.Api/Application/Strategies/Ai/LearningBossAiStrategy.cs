using System;
using Arena.Api.Application.Services;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Domain.Strategies;

namespace Arena.Api.Application.Strategies.Ai
{
    public class LearningBossAiStrategy : IAiStrategy
    {
        private static readonly Random _random = new Random();

        public Arena.Api.Domain.Entities.AiDecision DecideNextMove(GameSession session)
        {
            bool canHeal = session.CurrentArenaEvent != "ToxicGas";
            bool canDefend = session.CurrentArenaEvent != "MagneticStorm" && session.MonsterShieldCooldown == 0 && session.MonsterShieldDurability > 0;
            bool letalidade = session.Player.CurrentHp <= session.CurrentHeroMaxHp * 0.35;

            // 1. ROUBO DE POÇÃO (O JOGADOR TOXICO)
            // Se o herói tem pouca vida, tem poções e TEM O HÁBITO de curar no desespero...
            if (letalidade && session.HeroPotions > 0 && PlayerPatternTracker.PredizCuraNoDesespero())
            {
                if (canDefend)
                {
                    session.CombatLog.Add("🧠 [Hardcore Read] O Boss LÊ OS TEUS OLHOS! Ele meteu o escudo só para ROUBAR a tua poção!");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
            }

            // 2. MIND GAME DA ULTIMATE (O BAIT DO ESCUDO)
            if (session.MonsterUltCharge >= 2)
            {
                // Se o herói tem o hábito de defender quando o Boss tem a Ult...
                if (PlayerPatternTracker.PredizEscudoNoMedo() && session.HeroShieldCooldown == 0)
                {
                    session.CombatLog.Add("🧠 [Mind Game] O Boss sorriu. Ele sabe que estás a tremer e a agarrar o escudo. Ele GUARDA a Ultimate!");
                    
                    // Em vez de gastar a Ult no teu escudo, ele ataca para o partir ou cura-se!
                    if (session.Enemy.CurrentHp < session.CurrentMonsterMaxHp * 0.7 && session.MonsterPotions > 0 && canHeal)
                        return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
                    else
                        return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
                }
                
                // Se já partiu o teu escudo ou sabe que não o usas, manda a Ultimate!
                return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
            }

            // 3. DEFESA PRO-PLAYER CONTRA A TUA ULT
            if (session.HeroUltCharge >= 2 && canDefend)
            {
                // Se tens a Ult, ele não fica à espera de levar com ela na cara. 75% de chance de defender.
                if (_random.Next(100) < 75)
                {
                    session.CombatLog.Add("🧠 [Previsão de Dano] O Boss pressente a tua Ultimate e prepara a defesa!");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
            }

            // 4. SOBREVIVÊNCIA E RESPEITO AOS EVENTOS
            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.30 && session.MonsterPotions > 0)
            {
                if (canHeal) return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
                else session.CombatLog.Add("🧠 [Cálculo] O Boss queria curar-se, mas viu o Gás Tóxico e decidiu lutar até à morte!");
            }

            // 5. CAOS / IMPREVISIBILIDADE (Random factor)
            int rng = _random.Next(100);
            if (rng < 15 && canDefend) 
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            
            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }
    }
}