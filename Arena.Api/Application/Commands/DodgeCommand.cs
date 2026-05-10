using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Application.Commands
{
    public class DodgeCommand : ICombatCommand
    {
        public void Execute(GameSession session, bool isHero, IAttackStrategy? attackStrategy = null)
        {
            if (isHero)
            {
                if (session.HeroDodgesLeft <= 0)
                {
                    session.CombatLog.Add($"❌ {session.Player.Name} tentou esquivar, mas não tem mais esquivas disponíveis!");
                    return;
                }
                session.HeroDodgesLeft--;
                session.HeroDodgingThisTurn = true;
                session.CombatLog.Add($"💨 {session.Player.Name} preparou-se para ESQUIVAR! ({session.HeroDodgesLeft} esquiva(s) restante(s))");
            }
            else
            {
                if (session.MonsterDodgesLeft <= 0)
                {
                    // Sem esquivas, executa ataque físico como fallback
                    new AttackCommand().Execute(session, false, new Domain.Strategies.PhysicalAttack());
                    return;
                }
                session.MonsterDodgesLeft--;
                session.MonsterDodgingThisTurn = true;
                session.CombatLog.Add($"💨 {session.Enemy.Name} recuou e preparou uma esquiva ágil! ({session.MonsterDodgesLeft} restante(s))");
            }
        }
    }
}
