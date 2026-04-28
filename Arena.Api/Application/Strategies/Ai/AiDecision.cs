using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Application.Strategies.Ai
{
    public class AiDecision
    {
        public string ActionType { get; set; } = "Physical";
        public IAttackStrategy? AttackStrategy { get; set; } = null;
    }
}