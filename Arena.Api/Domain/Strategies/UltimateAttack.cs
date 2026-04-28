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
            // A Ultimate é devastadora. O multiplicador varia entre 200% a 300% da força base.
            double multiplier = _random.Next(200, 301) / 100.0;
            int damage = (int)(attacker.AttackPower * multiplier);

            // Pode até adicionar regras especiais aqui futuramente (ex: ignorar defesa do monstro)

            target.TakeDamage(damage);
            return damage;
        }
    }
}