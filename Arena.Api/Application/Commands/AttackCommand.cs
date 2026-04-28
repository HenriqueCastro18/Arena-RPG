using System;
using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Domain.Strategies;

namespace Arena.Api.Application.Commands
{
    public class AttackCommand : ICombatCommand
    {
        private static readonly Random rand = new Random();

        public void Execute(GameSession session, bool isHero, IAttackStrategy? attackStrategy = null)
        {
            if (session.Player.IsDead() || session.Enemy.IsDead() || attackStrategy == null) return;

            string attackerName = isHero ? session.Player.Name : session.Enemy.Name;
            string defenderName = isHero ? session.Enemy.Name : session.Player.Name;

            bool isUlt = attackStrategy is UltimateAttack;
            bool isPhysical = attackStrategy is PhysicalAttack;
            
            // ----------------------------------------------------
            // ARMADILHA DO MINOTAURO XAMÃ (ANTI-ULT / REFLEXÃO)
            // ----------------------------------------------------
            if (isHero && isUlt && session.MonsterUltStealDuration > 0) {
                session.HeroUltCharge = 0;
                session.MonsterUltStealDuration = 0; 
                
                // 1. Quebra os escudos do Herói
                session.HeroShieldDurability = 0;
                session.HeroShieldCooldown = 3;
                
                // 2. Calcula o dano exato que o Herói causaria
                int enemyHpBefore = session.Enemy.CurrentHp;
                attackStrategy.Execute(session.Player, session.Enemy);
                int reflectedDamage = enemyHpBefore - session.Enemy.CurrentHp;
                
                // Reverte a vida do monstro para anular o golpe nele
                session.Enemy.Heal(reflectedDamage); 

                // 3. Aplica o dano refletido no Herói
                session.Player.TakeDamage(reflectedDamage);
                session.HeroDamageTaken += reflectedDamage;

                session.CombatLog.Add($"🔮 ARMADILHA XAMÂNICA ATIVADA! O {defenderName} absorveu a tua Ultimate e ESTILHAÇOU os teus escudos!");
                session.CombatLog.Add($"💥 REFLEXÃO! O feitiço virou-se contra o feiticeiro: recebeste {reflectedDamage} de dano da tua própria magia!");
                return; 
            }

            string attackLogName = isUlt ? "💥 ULTIMATE" : "🗡️ Ataque";
            if (!isHero && isUlt) attackLogName = "🔥 ULTIMATE DAS SOMBRAS";
            if (!isHero && isPhysical) attackLogName = "🗡️ Revide";

            // 1. Gestão de Carga da Ultimate
            if (isHero) {
                if (isUlt) session.HeroUltCharge = 0;
                else if (session.HeroUltCharge < 2) {
                    session.HeroUltCharge = Math.Min(2, session.HeroUltCharge + (session.CurrentArenaEvent == "ManaBlessing" ? 2 : 1));
                }
            } else {
                if (isUlt) session.MonsterUltCharge = 0;
                else if (session.MonsterUltCharge < 2) {
                    session.MonsterUltCharge = Math.Min(2, session.MonsterUltCharge + (session.CurrentArenaEvent == "ManaBlessing" ? 2 : 1));
                }
            }

            // =========================================================================
            // 2. ULTIMATES DOS MINOTAUROS (APENAS HABILIDADES, ZERO DANO)
            // =========================================================================
            bool isMinotaur = attackerName.Contains("Guerreiro", StringComparison.OrdinalIgnoreCase) || 
                              attackerName.Contains("Xamã", StringComparison.OrdinalIgnoreCase) || 
                              attackerName.Contains("Shaman", StringComparison.OrdinalIgnoreCase) || 
                              attackerName.Contains("Chefe", StringComparison.OrdinalIgnoreCase) || 
                              attackerName.Contains("Boss", StringComparison.OrdinalIgnoreCase);

            if (!isHero && isUlt && isMinotaur) {
                
                session.CombatLog.Add($"🔥 {attackerName} canalizou o seu poder numa 💥 HABILIDADE ESPECIAL!");

                if (attackerName.Contains("Xamã", StringComparison.OrdinalIgnoreCase) || attackerName.Contains("Shaman", StringComparison.OrdinalIgnoreCase)) {
                    session.MonsterUltStealDuration = 3; 
                    session.CombatLog.Add($"🔮 [Armadilha] Anti-Ultimate: O {attackerName} preparou uma defesa mágica! Se usares a tua Ultimate, o dano será REFLETIDO para ti!");
                }
                else if (attackerName.Contains("Guerreiro", StringComparison.OrdinalIgnoreCase)) {
                    session.MonsterDamageBuffDuration = 3; 
                    session.CombatLog.Add($"🪓 Fúria Taurina: O {attackerName} enfureceu-se! O seu dano aumentará 30% nos próximos 2 turnos!");
                }
                else if (attackerName.Contains("Chefe", StringComparison.OrdinalIgnoreCase) || attackerName.Contains("Boss", StringComparison.OrdinalIgnoreCase)) {
                    bool defenderIsDefending = session.HeroDefendedThisTurn;
                    bool attackerInSecondWind = session.MonsterInSecondWind;
                    bool successfulBlock = defenderIsDefending && !attackerInSecondWind;

                    if (successfulBlock) {
                        session.CombatLog.Add($"🛡️ O {defenderName} ergueu o escudo e BLOQUEOU a maldição Esmagar Crânios!");
                        session.HeroShieldDurability -= 2; 
                        if (session.HeroShieldDurability <= 0) session.HeroShieldCooldown = 3;
                    } else {
                        if (defenderIsDefending && attackerInSecondWind) {
                            session.CombatLog.Add($"🌪️ A maldição de {attackerName} atravessou o escudo devido ao Último Suspiro!");
                        }
                        
                        int penalty = session.Player.MaxHp * 50 / 100; 
                        session.HeroMaxHpPenalty += penalty;
                        session.CombatLog.Add($"🐂 Esmagar Crânios: Um rugido ensurdecedor! O {attackerName} reduziu o teu HP Máximo em 50% permanentemente!");
                        
                        if (session.CurrentHeroMaxHp <= 0) {
                            session.CombatLog.Add($"☠️ O corpo de {defenderName} não suportou a brutalidade e cedeu completamente.");
                            session.Player.TakeDamage(session.Player.CurrentHp);
                        }
                    }
                }
                
                return; 
            }
            // =========================================================================

            // 3. Cálculo e Execução do Dano Base
            int hpBeforeAttack = isHero ? session.Enemy.CurrentHp : session.Player.CurrentHp;
            
            if (isHero) attackStrategy.Execute(session.Player, session.Enemy);
            else attackStrategy.Execute(session.Enemy, session.Player);
            
            int actualDamage = hpBeforeAttack - (isHero ? session.Enemy.CurrentHp : session.Player.CurrentHp);

            // Modificador: Frenesi de Batalha
            if (session.CurrentArenaEvent == "BloodFrenzy" && isPhysical && actualDamage > 0) {
                actualDamage += actualDamage / 2;
            }

            // Modificador: Buff Físico do Minotauro Guerreiro (+30%)
            if (!isHero && session.MonsterDamageBuffDuration > 0 && actualDamage > 0) {
                actualDamage += actualDamage * 30 / 100; 
            }

            // 👇 NOVO: AURA MÍSTICA DO XAMÃ (Corta o teu dano físico pela metade enquanto a armadilha está ativa!)
            if (isHero && isPhysical && session.MonsterUltStealDuration > 0 && actualDamage > 0 && session.CurrentArenaEvent != "ToxicGas") {
                actualDamage = actualDamage / 2;
                session.CombatLog.Add($"🛡️ [Aura Mística] A armadilha protege o Xamã contra a força bruta! O teu Ataque Físico causou apenas metade do dano.");
            }

            // 4. Passivas Defensivas
            bool dodged = false;
            if (!isHero && defenderName.Contains("Maga", StringComparison.OrdinalIgnoreCase) && isUlt && rand.Next(100) < 20) {
                session.Player.Heal(actualDamage); actualDamage = 0; dodged = true;
                session.CombatLog.Add($"{attackerName} usou {attackLogName}, mas a Maga Arcana anulou o dano com ✨ [Escudo de Mana]!");
            }
            else if (isHero && defenderName.Contains("Caçadora", StringComparison.OrdinalIgnoreCase) && isUlt && rand.Next(100) < 30) {
                session.Enemy.Heal(actualDamage); actualDamage = 0; dodged = true;
                session.CombatLog.Add($"{attackerName} usou {attackLogName}, mas a Caçadora esquivou completamente!");
            } 

            // 5. Lógica de Escudos (Redução de 60%)
            bool successfulBlockCombat = false;

            if (!dodged) {
                bool defenderIsDefending = isHero ? session.IsMonsterDefending : session.HeroDefendedThisTurn;
                bool attackerInSecondWind = isHero ? session.HeroInSecondWind : session.MonsterInSecondWind;
                
                if (defenderIsDefending && actualDamage > 0) {
                    if (attackerInSecondWind) {
                        session.CombatLog.Add($"🌪️ O ataque de {attackerName} atravessou o escudo devido ao Último Suspiro!");
                    } else {
                        int mitigated = (int)(actualDamage * 0.6); 
                        
                        if (isHero) session.Enemy.Heal(mitigated);
                        else session.Player.Heal(mitigated);
                        
                        actualDamage -= mitigated; 
                        
                        if (isHero) {
                            session.MonsterDefendedThisTurn = true; 
                            session.MonsterShieldDurability -= (isUlt ? 2 : 1);
                            if (session.MonsterShieldDurability <= 0) session.MonsterShieldCooldown = 3;
                        } else {
                            session.HeroDefendedThisTurn = true; 
                            session.HeroShieldDurability -= (isUlt ? 2 : 1);
                            if (session.HeroShieldDurability <= 0) session.HeroShieldCooldown = 3;
                        }
                    }
                }

                if (isHero) session.MonsterDamageTaken += actualDamage; 
                else session.HeroDamageTaken += actualDamage;

                successfulBlockCombat = defenderIsDefending && !attackerInSecondWind;
                
                if (successfulBlockCombat) {
                    session.CombatLog.Add($"{attackerName} usou {attackLogName}. {defenderName} BLOQUEOU parte do golpe.");
                } else {
                    session.CombatLog.Add($"{attackerName} usou {attackLogName} e causou {actualDamage} de dano.");
                }

                bool attackerIsDead = isHero ? session.Player.IsDead() : session.Enemy.IsDead();
                
                // 6. Passivas Ofensivas Pós-Ataque
                if (!attackerIsDead) {
                    bool hitEmCheio = !successfulBlockCombat;
                    
                    if (attackerName.Contains("Rei", StringComparison.OrdinalIgnoreCase) && isUlt && actualDamage > 0 && hitEmCheio && session.CurrentArenaEvent != "ToxicGas") {
                        int regen = (isHero ? session.CurrentHeroMaxHp : session.CurrentMonsterMaxHp) * 25 / 100; 
                        if (isHero) { session.Player.Heal(regen); session.LastHeroHealed += regen; } 
                        else { session.Enemy.Heal(regen); session.LastMonsterHealed += regen; }
                        session.CombatLog.Add($"👑 [Passiva] Sangue Real: O {attackerName} acertou a Ultimate EM CHEIO e curou {regen} de HP!");
                    }

                    if (attackerName.Contains("Sombrio", StringComparison.OrdinalIgnoreCase) && isPhysical && actualDamage > 0 && session.CurrentArenaEvent != "ToxicGas") {
                        int vamp = actualDamage * 30 / 100;
                        if (isHero) { session.Player.Heal(vamp); session.LastHeroHealed += vamp; } 
                        else { session.Enemy.Heal(vamp); session.LastMonsterHealed += vamp; }
                        session.CombatLog.Add($"🧛 [Passiva] Vampirismo: {attackerName} drenou {vamp} de HP!");
                    }
                }
            }
        }
    }
}