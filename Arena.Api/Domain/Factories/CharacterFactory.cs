using System;
using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Factories
{
    public static class CharacterFactory
    {
        private static readonly Random _rand = new Random();
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
                "Huntress"        => new Monster("Caçadora Escura",    520, 170, "Trevas"),
                "EvilMago"        => new Monster("Mago Sombrio",       570, 168, "Trevas"),
                "Worm"            => new Monster("Verme Gigante",       680, 145, "Terra"),
                "MinotaurWarrior" => new Monster("Minotauro Guerreiro", 750, 165, "Físico"),
                "MinotaurShaman"  => new Monster("Minotauro Xamã",     820, 178, "Mágico"),
                "MinotaurBoss"    => new Monster("Minotauro Chefe",   1450, 130, "Chefe"),
                _                 => new Monster("Orc Selvagem",        540, 148, "Terra")
            };

            if (monsterType == "MinotaurBoss")
            {
                int totalActions = analytics != null
                    ? analytics.TotalAttacks + analytics.TotalHeals + analytics.TotalDefends + analytics.TotalUlts
                    : 0;

                if (totalActions < 5)
                {
                    // Sem dados suficientes: sorteia aleatoriamente entre as 4 variantes
                    return _rand.Next(4) switch
                    {
                        0 => baseMonster,
                        1 => new Monster("Minotauro Tático",       baseMonster.MaxHp + 200, baseMonster.AttackPower + 30, "Chefe Assassino"),
                        2 => new Monster("Minotauro Quebra-Guarda", baseMonster.MaxHp,       baseMonster.AttackPower + 50, "Chefe Furioso"),
                        _ => new Monster("Minotauro Implacável",   baseMonster.MaxHp + 150, baseMonster.AttackPower + 35, "Chefe Equilibrado"),
                    };
                }

                var estilo = analytics!.ObterEstiloPredominante();

                if (estilo == "Agressivo")
                    return new Monster("Minotauro Tático", baseMonster.MaxHp + 200, baseMonster.AttackPower + 30, "Chefe Assassino");

                if (analytics.UsaMuitoEscudo)
                    return new Monster("Minotauro Quebra-Guarda", baseMonster.MaxHp, baseMonster.AttackPower + 50, "Chefe Furioso");

                if (estilo == "Equilibrado")
                    return new Monster("Minotauro Implacável", baseMonster.MaxHp + 150, baseMonster.AttackPower + 35, "Chefe Equilibrado");
            }

            return baseMonster;
        }
    }
}