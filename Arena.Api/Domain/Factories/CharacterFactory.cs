using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Factories
{
    public static class CharacterFactory
    {
        public static Hero CreateHero(string heroClass)
        {
            return heroClass switch
            {
                "Mago"      => new Hero("Maga Arcana", 1000, 185, "Mago"),
                "Arqueiro"  => new Hero("Robin", 1090, 150, "Arqueiro"),
                "Rei"       => new Hero("Rei Soberano", 1200, 130, "Rei"),
                _           => new Hero("Arthur", 1150, 140, "Guerreiro")
            };
        }

        public static Monster CreateMonster(string monsterType, PlayerAnalytics? analytics = null)
        {
            var baseMonster = monsterType switch
            {
                "Huntress"  => new Monster("Caçadora Escura", 450, 160, "Trevas"),
                "EvilMago"  => new Monster("Mago Sombrio", 500, 155, "Trevas"),
                "Worm"      => new Monster("Verme Gigante", 600, 120, "Terra"),
                "MinotaurWarrior" => new Monster("Minotauro Guerreiro", 650, 150, "Físico"),
                "MinotaurShaman"  => new Monster("Minotauro Xamã", 700, 160, "Mágico"),
                "MinotaurBoss"    => new Monster("Minotauro Chefe", 1300, 180, "Chefe"), // Vida justa
                _           => new Monster("Orc Selvagem", 500, 140, "Terra")
            };

            if (monsterType == "MinotaurBoss" && analytics != null)
            {
                var estilo = analytics.ObterEstiloPredominante();
                
                if (estilo == "Agressivo")
                {
                    return new Monster("Minotauro Tático", baseMonster.MaxHp + 200, baseMonster.AttackPower + 40, "Chefe Assassino");
                }
                
                if (analytics.UsaMuitoEscudo)
                {
                    return new Monster("Minotauro Quebra-Guarda", baseMonster.MaxHp, baseMonster.AttackPower + 80, "Chefe Furioso");
                }
            }

            return baseMonster;
        }
    }
}