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

        // Estado interno de fase (por instância — uma por sessão)
        private bool _phase2TransitionDone = false;
        private bool _phase3TransitionDone = false;

        public Arena.Api.Domain.Entities.AiDecision DecideNextMove(GameSession session)
        {
            bool canHeal   = session.CurrentArenaEvent != "ToxicGas" && session.MonsterPotions > 0;
            bool canDefend = session.CurrentArenaEvent != "MagneticStorm"
                             && session.MonsterShieldCooldown == 0
                             && session.MonsterShieldDurability > 0;
            bool canDodge  = session.MonsterDodgesLeft > 0;
            bool playerLow = session.Player.CurrentHp <= session.CurrentHeroMaxHp * 0.35f;

            // Determinar fase pelo HP atual do boss
            float bossHpRatio = (float)session.Enemy.CurrentHp / session.CurrentMonsterMaxHp;
            int phase = bossHpRatio > 0.65f ? 1 : (bossHpRatio > 0.35f ? 2 : 3);

            // Transições de fase (log único por fase)
            if (phase == 2 && !_phase2TransitionDone)
            {
                _phase2TransitionDone = true;
                session.CombatLog.Add($"🔶 [FASE 2] O {session.Enemy.Name} está ferido... e ficou MAIS PERIGOSO. Ele começou a ler os teus padrões!");
            }
            else if (phase == 3 && !_phase3TransitionDone)
            {
                _phase3TransitionDone = true;
                session.CombatLog.Add($"🔴 [FASE FINAL] O {session.Enemy.Name} rugiu. Com nada a perder, a luta verdadeira começa AGORA!");
            }

            // === ESQUIVA PROATIVA: boss esquiva quando herói tem ult pronta ===
            if (canDodge && session.HeroUltCharge >= 2 && phase >= 2 && _random.Next(100) < 45) {
                session.CombatLog.Add("🧠 [Esquiva Boss] O Boss leu o teu próximo movimento e recuou para evitar a tua Ultimate!");
                return new Arena.Api.Domain.Entities.AiDecision("Dodge", null);
            }

            // === PRIORIDADE MÁXIMA: ROUBO DE POÇÃO ===
            if (playerLow && session.HeroPotions > 0 && canDefend)
            {
                if (phase >= 2 && PlayerPatternTracker.PredizCuraNoDesespero())
                {
                    session.CombatLog.Add("🧠 [Hardcore Read] O Boss LÊ OS TEUS OLHOS! Ele meteu o escudo só para ROUBAR a tua poção!");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
                if (phase == 1 && _random.Next(100) < 50)
                {
                    session.CombatLog.Add("🧠 [Instinto] O Boss percebeu a tua fraqueza e tentou interceptar...");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
            }

            // === DECISÃO DA ULTIMATE (custo: 3 cargas) ===
            // 20% de chance de IGNORAR a Ultimate mesmo com ela carregada (imprevisibilidade real)
            if (session.MonsterUltCharge >= 3 && _random.Next(100) >= 20)
                return DecideUltimatePlay(session, canHeal, canDefend, phase);

            // === DECISÃO POR FASE ===
            return phase switch
            {
                1 => DecidePhase1(session, canHeal, canDefend),
                2 => DecidePhase2(session, canHeal, canDefend),
                _ => DecidePhase3(session, canHeal, canDefend)
            };
        }

        // FASE 1 (HP > 65%): Aprendizagem — metódico, coleta dados, testa o jogador
        private Arena.Api.Domain.Entities.AiDecision DecidePhase1(GameSession session, bool canHeal, bool canDefend)
        {
            if (session.HeroUltCharge >= 2 && canDefend && _random.Next(100) < 60)
            {
                session.CombatLog.Add("🧠 [Observação] O Boss nota a tua Ultimate carregada e ergueu o escudo por precaução.");
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            // Cura conservadora mais precoce do que os monstros normais
            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.55f && session.MonsterPotions > 0 && canHeal)
            {
                session.CombatLog.Add("🧠 [Estratégia] O Boss recuou para recuperar força enquanto te estuda.");
                return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
            }

            // 20% de caos inicial para começar imprevisível
            if (_random.Next(100) < 20 && canDefend)
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);

            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }

        // FASE 2 (35-65% HP): Adaptação — usa padrões do jogador contra ele
        private Arena.Api.Domain.Entities.AiDecision DecidePhase2(GameSession session, bool canHeal, bool canDefend)
        {
            // Contrariar jogador agressivo: quanto mais ele ataca, mais o boss bloqueia
            if (PlayerPatternTracker.PredizJogadorAgressivo() && canDefend)
            {
                int counterChance = (int)(PlayerPatternTracker.GetAggressivenessScore() * 45);
                if (_random.Next(100) < counterChance)
                {
                    session.CombatLog.Add("🧠 [Análise] O Boss identificou o teu padrão agressivo! Ele vai punir a tua próxima abertura!");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
            }

            // Defesa contra Ultimate do herói — aumenta com o tempo de luta
            if (session.HeroUltCharge >= 2 && canDefend)
            {
                int defendChance = Math.Min(90, 75 + session.RoundCount / 3);
                if (_random.Next(100) < defendChance)
                {
                    session.CombatLog.Add("🧠 [Adaptação] O Boss reconheceu o padrão! Ele ergueu o escudo ANTES que pudesses agir!");
                    return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                }
            }

            // Prever próxima ação e reagir preventivamente
            string predicted = PlayerPatternTracker.PredizProximaAcao();
            if (predicted == "Heal" && session.HeroPotions > 0 && canDefend && _random.Next(100) < 60)
            {
                session.CombatLog.Add("🧠 [Leitura Profunda] O Boss previu a tua cura e está pronto para intercetá-la!");
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.45f && session.MonsterPotions > 0 && canHeal)
                return new Arena.Api.Domain.Entities.AiDecision("Heal", null);

            // 15% de caos calculado para manter imprevisibilidade
            if (_random.Next(100) < 15 && canDefend)
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);

            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }

        // FASE 3 (HP < 35%): Enraivecido — caótico, instintivo e desesperado
        private Arena.Api.Domain.Entities.AiDecision DecidePhase3(GameSession session, bool canHeal, bool canDefend)
        {
            // Cura de emergência em HP crítico
            if (session.Enemy.CurrentHp <= session.CurrentMonsterMaxHp * 0.12f && session.MonsterPotions > 0 && canHeal)
            {
                session.CombatLog.Add("🧠 [Último Recurso] O Boss bebeu a última poção antes de ser abatido!");
                return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
            }

            // Alta chance de defender contra Ultimate do herói mesmo no caos
            if (session.HeroUltCharge >= 2 && canDefend && _random.Next(100) < 85)
            {
                session.CombatLog.Add("🧠 [Sobrevivência] O Boss não vai morrer assim. Bloqueou com toda a força que lhe resta!");
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            // Caos: 55% ataque puro, 25% defesa surpresa, 15% aposta na cura, 5% ataque
            int roll = _random.Next(100);
            if (roll < 55)
                return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());

            if (roll < 80 && canDefend)
            {
                session.CombatLog.Add("🧠 [Caos] O Boss fintou um ataque e recuou para te desestabilizar!");
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            if (roll < 92 && session.MonsterPotions > 0 && canHeal)
            {
                session.CombatLog.Add("🧠 [Aposta] O Boss apostou tudo numa cura de emergência!");
                return new Arena.Api.Domain.Entities.AiDecision("Heal", null);
            }

            return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
        }

        // DECISÃO DA ULTIMATE: contextual por fase e padrões do jogador
        private Arena.Api.Domain.Entities.AiDecision DecideUltimatePlay(GameSession session, bool canHeal, bool canDefend, int phase)
        {
            bool canDodge = session.MonsterDodgesLeft > 0;
            bool playerLikelyShields     = PlayerPatternTracker.PredizEscudoNoMedo() && session.HeroShieldCooldown == 0;
            bool playerCountersWithUlt   = PlayerPatternTracker.PredizUltimateBruta();
            bool playerAttacksAfterHeal  = PlayerPatternTracker.PredizAtaqueAposDefesa();

            // Fase 2+: Mind game se o jogador tende a escudar quando boss tem Ultimate
            if (phase >= 2 && playerLikelyShields)
            {
                double mindGameChance = phase == 3 ? 0.80 : 0.65;
                if (_random.NextDouble() < mindGameChance)
                {
                    session.CombatLog.Add("🧠 [Mind Game] O Boss sorriu. Sabe que estás a tremer e a agarrar o escudo. Ele GUARDA a Ultimate!");

                    // Se jogador ataca após defender, boss defende para contra-atacar
                    if (playerAttacksAfterHeal && canDefend)
                    {
                        session.CombatLog.Add("🧠 [Leitura Avançada] O Boss sabe que vais atacar a seguir. Ele prepara a resposta perfeita!");
                        return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
                    }

                    // Aproveita para curar ou atacar fisicamente enquanto não usa a ult
                    if (session.Enemy.CurrentHp < session.CurrentMonsterMaxHp * 0.65f && session.MonsterPotions > 0 && canHeal)
                        return new Arena.Api.Domain.Entities.AiDecision("Heal", null);

                    return new Arena.Api.Domain.Entities.AiDecision("Attack", new PhysicalAttack());
                }
            }

            // Jogador não tem medo e usa a própria ult de volta: esquivar e guardar
            if (playerCountersWithUlt && canDodge && phase >= 2 && _random.Next(100) < 60) {
                session.CombatLog.Add("🧠 [Esquiva Mestra] O Boss previu a tua Ultimate e esquivou antes de a tua carga disparar!");
                return new Arena.Api.Domain.Entities.AiDecision("Dodge", null);
            }
            if (playerCountersWithUlt)
            {
                session.CombatLog.Add("🧠 [Decisão] O Boss viu que não tens medo. Ele lança a Ultimate com tudo!");
                return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
            }

            // Prever ação do jogador com base na sequência
            string predicted = PlayerPatternTracker.PredizProximaAcao();
            if (predicted == "Defend" && phase >= 2 && _random.Next(100) < 55)
            {
                session.CombatLog.Add("🧠 [Previsão] O Boss prevê a tua defesa e decidiu guardar a Ultimate para o momento certo...");
                return new Arena.Api.Domain.Entities.AiDecision("Defend", null);
            }

            // Sem dados suficientes ou jogador imprevisível: lança a Ultimate
            return new Arena.Api.Domain.Entities.AiDecision("Ultimate", new UltimateAttack());
        }
    }
}
