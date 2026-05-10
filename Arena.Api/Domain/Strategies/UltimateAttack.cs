using System;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Domain.Strategies
{
    public class UltimateAttack : IAttackStrategy
    {
        private readonly Random _random = new Random();

        public int Execute(Character attacker, Character target)
        {
            // O multiplicador varia entre 160% a 240% da força base.
            double multiplier = _random.Next(160, 241) / 100.0;
            int damage = (int)(attacker.AttackPower * multiplier);

            // Pode até adicionar regras especiais aqui futuramente (ex: ignorar defesa do monstro)

            target.TakeDamage(damage);
            return damage;
        }
    }
}