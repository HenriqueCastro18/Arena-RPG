using System;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Domain.Strategies
{
    public class PhysicalAttack : IAttackStrategy
    {
        private readonly Random _random = new Random();

        public int Execute(Character attacker, Character target)
        {
            int damage = attacker.AttackPower;
            
            // 20% de chance de acerto crítico (dano x2)
            bool isCritical = _random.Next(1, 101) <= 20;
            if (isCritical)
            {
                damage *= 2;
            }

            target.TakeDamage(damage);
            return damage;
        }
    }
}