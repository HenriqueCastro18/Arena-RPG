using System;

namespace Arena.Api.Domain.Entities
{
    public abstract class Character
    {
        public string Name { get; protected set; }
        public int MaxHp { get; protected set; }
        
        // O protected foi removido daqui para liberar o acesso ao GameSession
        public int CurrentHp { get; set; } 
        
        public int AttackPower { get; protected set; } 

        public Character(string name, int maxHp, int attackPower)
        {
            Name = name;
            MaxHp = maxHp;
            CurrentHp = maxHp;
            AttackPower = attackPower;
        }

        public void TakeDamage(int damage)
        {
            CurrentHp -= damage;
            if (CurrentHp < 0) CurrentHp = 0;
        }

        public void Heal(int amount)
        {
            CurrentHp = Math.Min(MaxHp, CurrentHp + amount);
        }

        public bool IsDead() => CurrentHp <= 0;
    }
}