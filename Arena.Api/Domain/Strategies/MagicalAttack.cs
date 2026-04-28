using System;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Domain.Strategies
{
    public class MagicalAttack : IAttackStrategy
    {
        private readonly Random _random = new Random();

        public int Execute(Character attacker, Character target)
        {
            // Magia não tem crítico, mas varia entre 80% a 120% da força base
            double multiplier = _random.Next(80, 121) / 100.0;
            int damage = (int)(attacker.AttackPower * multiplier);

            target.TakeDamage(damage);
            return damage;
        }
    }
}