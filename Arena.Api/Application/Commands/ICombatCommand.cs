using Arena.Api.Application.Services;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Application.Commands
{
    public interface ICombatCommand
    {
        void Execute(GameSession session, bool isHero, IAttackStrategy? attackStrategy = null);
    }
}