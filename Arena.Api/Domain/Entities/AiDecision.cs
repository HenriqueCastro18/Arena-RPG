using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Domain.Entities
{
    public class AiDecision
    {
        public string ActionType { get; }
        public IAttackStrategy? AttackStrategy { get; }

        public AiDecision(string actionType, IAttackStrategy? attackStrategy)
        {
            ActionType = actionType;
            AttackStrategy = attackStrategy;
        }
    }
}