namespace Arena.Api.Domain.Entities
{
    public class Hero : Character
    {
        public string HeroClass { get; private set; }

        public Hero(string name, int maxHp, int attackPower, string heroClass) 
            : base(name, maxHp, attackPower) 
        {
            HeroClass = heroClass;
        }
    }
}