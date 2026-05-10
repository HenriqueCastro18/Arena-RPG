using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Application.Commands
{
    public class HealCommand : ICombatCommand
    {
        public void Execute(GameSession session, bool isHero, IAttackStrategy? attackStrategy = null)
        {
            string characterName = isHero ? session.Player.Name : session.Enemy.Name;
            int maxHp = isHero ? session.CurrentHeroMaxHp : session.CurrentMonsterMaxHp;
            
            int potions = isHero ? session.HeroPotions : session.MonsterPotions;
            bool wasStolen = isHero ? session.MonsterStolePotionThisTurn : session.HeroStolePotionThisTurn;
            string thiefName = isHero ? session.Enemy.Name : session.Player.Name;

            if (potions <= 0)
            {
                session.CombatLog.Add($"{characterName} tentou beber uma poção, mas o frasco estava vazio!");
                return;
            }

            // Gasta a poção
            if (isHero) session.HeroPotions--; else session.MonsterPotions--;

            if (wasStolen)
            {
                session.CombatLog.Add($"🥷 INTERROMPIDO! {thiefName} antecipou o movimento e ROUBOU a poção de {characterName}!");
                if (session.CurrentArenaEvent != "ToxicGas")
                {
                    int thiefMaxHp = isHero ? session.CurrentMonsterMaxHp : session.CurrentHeroMaxHp;
                    int healAmount = thiefMaxHp * 40 / 100;

                    // Cura o Ladrão, capped ao HP máximo efetivo
                    if (isHero) session.Enemy.CurrentHp = Math.Min(thiefMaxHp, session.Enemy.CurrentHp + healAmount);
                    else session.Player.CurrentHp = Math.Min(thiefMaxHp, session.Player.CurrentHp + healAmount);

                    if (isHero) session.LastMonsterHealed = healAmount;
                    else session.LastHeroHealed = healAmount;
                    
                    session.CombatLog.Add($"{thiefName} bebeu a poção roubada e recuperou HP nas tuas barbas!");
                }
                else
                {
                    session.CombatLog.Add($"☠️ {thiefName} bebeu a poção roubada, mas o Gás Tóxico ANULOU a cura!");
                }
            }
            else
            {
                if (session.CurrentArenaEvent == "ToxicGas")
                {
                    session.CombatLog.Add($"☠️ {characterName} tentou curar-se, mas o Gás Tóxico ANULOU o efeito!");
                }
                else
                {
                    int healAmount = maxHp * 40 / 100;
                    int hpBefore = isHero ? session.Player.CurrentHp : session.Enemy.CurrentHp;

                    // Cura capped ao HP máximo EFETIVO (respeitando penalidades)
                    if (isHero) session.Player.CurrentHp = Math.Min(maxHp, session.Player.CurrentHp + healAmount);
                    else session.Enemy.CurrentHp = Math.Min(maxHp, session.Enemy.CurrentHp + healAmount);

                    int actuallyHealed = (isHero ? session.Player.CurrentHp : session.Enemy.CurrentHp) - hpBefore;

                    if (isHero) session.LastHeroHealed = actuallyHealed;
                    else session.LastMonsterHealed = actuallyHealed;

                    session.CombatLog.Add($"{characterName} bebeu uma Poção e recuperou {actuallyHealed} de HP!");
                }
            }
        }
    }
}