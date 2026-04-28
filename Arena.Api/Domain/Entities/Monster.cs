namespace Arena.Api.Domain.Entities
{
    public class Monster : Character
    {
        public string ElementType { get; private set; }

        public Monster(string name, int maxHp, int attackPower, string elementType) 
            : base(name, maxHp, attackPower)
        {
            ElementType = elementType;
        }
    }
}