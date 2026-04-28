using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Application.Commands
{
    public class DefendCommand : ICombatCommand
    {
        public void Execute(GameSession session, bool isHero, IAttackStrategy? attackStrategy = null)
        {
            var characterName = isHero ? session.Player.Name : session.Enemy.Name;
            
            // CORREÇÃO: Agora exige que a Durabilidade seja > 0 rigorosamente!
            bool canDefend = isHero 
                ? (session.HeroShieldCooldown == 0 && session.HeroShieldDurability > 0 && session.CurrentArenaEvent != "MagneticStorm")
                : (session.MonsterShieldCooldown == 0 && session.MonsterShieldDurability > 0 && session.CurrentArenaEvent != "MagneticStorm");

            bool stolePotion = isHero ? session.HeroStolePotionThisTurn : session.MonsterStolePotionThisTurn;

            if (!canDefend)
            {
                session.CombatLog.Add($"{characterName} tentou defender, mas o escudo não tinha poder para bloquear!");
            }
            else
            {
                if (isHero) session.HeroDefendedThisTurn = true; else session.IsMonsterDefending = true;

                if (stolePotion)
                    session.CombatLog.Add($"🛡️ {characterName} avançou numa postura agressiva para interceptar o inimigo!");
                else
                    session.CombatLog.Add($"{characterName} assumiu uma postura defensiva impenetrável!");
            }
        }
    }
}