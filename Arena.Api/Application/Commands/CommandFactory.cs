using System;

namespace Arena.Api.Application.Commands
{
    public static class CommandFactory
    {
        public static ICombatCommand Create(string actionType)
        {
            return actionType switch
            {
                "Heal" => new HealCommand(),
                "Defend" => new DefendCommand(),
                "Physical" => new AttackCommand(),
                "Ultimate" => new AttackCommand(),
                "Attack" => new AttackCommand(), // Fallback
                _ => throw new ArgumentException($"Comando desconhecido: {actionType}")
            };
        }
    }
}