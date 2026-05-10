using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Arena.Api.Application.Services;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Domain.Strategies;
using Arena.Api.Domain.Factories;
using Arena.Api.Domain.Services;

namespace Arena.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameManager _gameManager;
        private readonly ITrainingLogService _trainingLog;

        public GameController(GameManager gameManager, ITrainingLogService trainingLog)
        {
            _gameManager = gameManager;
            _trainingLog = trainingLog;
        }

        [HttpPost("start")]
        public IActionResult StartGame([FromBody] StartGameDungeonRequest request)
        {
            PlayerAnalytics currentAnalytics = request.PlayerAnalytics ?? new PlayerAnalytics();

            var hero = CharacterFactory.CreateHero(request.HeroClass ?? "Guerreiro");
            var monster = CharacterFactory.CreateMonster(request.MonsterType ?? "Orc", currentAnalytics);
            
            var sessionId = _gameManager.StartNewGame(hero, monster);
            var session = _gameManager.GetSession(sessionId);
            
            return Ok(new { 
                SessionId = sessionId, 
                HeroName = $"{session!.Player.Name} ({session.Player.HeroClass})",
                HeroMaxHp = session.CurrentHeroMaxHp,
                MonsterName = $"{session.Enemy.Name} ({session.Enemy.ElementType})",
                MonsterMaxHp = session.CurrentMonsterMaxHp,
                HeroPotions = session.HeroPotions,
                HeroUltCharge = session.HeroUltCharge,
                HeroShieldDurability = session.HeroShieldDurability, 
                HeroShieldCooldown = session.HeroShieldCooldown,     
                CurrentArenaEvent = session.CurrentArenaEvent,
                EventDuration = session.EventDuration,
                Log = session.CombatLog,
                HeroDodgesLeft = session.HeroDodgesLeft,
                HeroBloodPactUsed = session.HeroBloodPactUsed,
                HeroInSecondWind = false,
                MonsterInSecondWind = false
            });
        }

        [HttpPost("{sessionId}/attack")]
        public IActionResult Attack(Guid sessionId, [FromBody] AttackRequest request)
        {
            var session = _gameManager.GetSession(sessionId);
            if (session == null) return NotFound("Partida não encontrada.");

            string acaoDaIa = AiDecisionService.DecidirAcaoProPlayer(
                session.Enemy, 
                session.Player, 
                session.MonsterPotions, 
                0, 
                session.HeroShieldDurability
            );

            if (request.AttackType == "Heal" || request.AttackType == "Defend" || request.AttackType == "Dodge")
                session.PlayRound(request.AttackType);
            else
                session.PlayRound("Attack", request.AttackType == "Ultimate" ? new UltimateAttack() : new PhysicalAttack());

            if (session.IsGameOver)
                _ = _trainingLog.SaveAsync(session.BuildTrainingReport());

            return Ok(new {
                IsGameOver = session.IsGameOver,
                HeroHp = session.Player.CurrentHp,
                MonsterHp = session.Enemy.CurrentHp,
                HeroMaxHp = session.CurrentHeroMaxHp,
                MonsterMaxHp = session.CurrentMonsterMaxHp,
                // Lendo os nomes corretos da GameSession
                HeroDamageTaken = session.HeroDamageTaken,
                MonsterDamageTaken = session.MonsterDamageTaken,
                HeroHealed = session.LastHeroHealed,
                MonsterHealed = session.LastMonsterHealed,
                HeroDefended = session.HeroDefendedThisTurn,
                MonsterDefended = session.MonsterDefendedThisTurn,
                HeroStolePotion = session.HeroStolePotionThisTurn,
                MonsterStolePotion = session.MonsterStolePotionThisTurn,
                // ...
                HeroPotions = session.HeroPotions,
                MonsterPotions = session.MonsterPotions,
                HeroUltCharge = session.HeroUltCharge,
                HeroShieldDurability = session.HeroShieldDurability,
                HeroShieldCooldown = session.HeroShieldCooldown,
                MonsterShieldDurability = session.MonsterShieldDurability,
                MonsterShieldCooldown = session.MonsterShieldCooldown,
                MonsterDodgesLeft = session.MonsterDodgesLeft,
                HeroInSecondWind = session.HeroInSecondWind,
                MonsterInSecondWind = session.MonsterInSecondWind,
                MonsterAction = session.LastMonsterAttackType ?? acaoDaIa,
                MonsterActedFirst = session.MonsterActedFirst,
                TriggeredEventThisRound = session.TriggeredEventThisRound,
                CurrentArenaEvent = session.CurrentArenaEvent,
                EventDuration = session.EventDuration,
                HeroDodgesLeft = session.HeroDodgesLeft,
                HeroBloodPactUsed = session.HeroBloodPactUsed,
                Log = session.CombatLog
            });
        }

        [HttpPost("{sessionId}/bloodpact")]
        public IActionResult BloodPact(Guid sessionId)
        {
            var session = _gameManager.GetSession(sessionId);
            if (session == null) return NotFound("Partida não encontrada.");

            session.ApplyBloodPact();

            return Ok(new {
                HeroHp = session.Player.CurrentHp,
                HeroMaxHp = session.CurrentHeroMaxHp,
                HeroUltCharge = session.HeroUltCharge,
                HeroShieldDurability = session.HeroShieldDurability,
                HeroShieldCooldown = session.HeroShieldCooldown,
                HeroBloodPactUsed = session.HeroBloodPactUsed,
                HeroInSecondWind = session.HeroInSecondWind,
                MonsterInSecondWind = session.MonsterInSecondWind,
                Log = session.CombatLog
            });
        }

        [HttpPost("simulate")]
        public IActionResult SimulateMatches([FromBody] StartGameRequest request)
        {
            int heroWins = 0;
            int monsterWins = 0;
            int roundsMedia = 0;

            for (int i = 0; i < 100; i++)
            {
                var hero = CharacterFactory.CreateHero(request.HeroClass ?? "Rei");
                var monster = CharacterFactory.CreateMonster(request.MonsterType ?? "MinotaurBoss");
                
                var sessionId = _gameManager.StartNewGame(hero, monster);
                var session = _gameManager.GetSession(sessionId);
                
                int rounds = 0;
                // O ponto de exclamação (!) abaixo remove o warning CS8602
                while (!session!.IsGameOver && rounds < 100) 
                {
                    rounds++;
                    string acaoHeroi = AiDecisionService.DecidirAcaoProPlayer(session.Player, session.Enemy, session.HeroPotions, session.HeroUltCharge, session.MonsterShieldCooldown > 0 ? 0 : 3);
                    session.PlayRound(acaoHeroi);
                }

                roundsMedia += rounds;
                if (session.Player.CurrentHp > 0) heroWins++;
                else monsterWins++;
            }

            return Ok(new {
                Mensagem = "Treinamento Finalizado. 100 Partidas Simultâneas.",
                HeroClass = request.HeroClass,
                MonsterType = request.MonsterType,
                Resultados = new {
                    VitoriasHeroi = heroWins,
                    VitoriasBoss = monsterWins,
                    MediaDeTurnos = roundsMedia / 100
                }
            });
        }
    }

    public class StartGameDungeonRequest
    {
        public string? HeroClass { get; set; }
        public string? MonsterType { get; set; }
        public PlayerAnalytics? PlayerAnalytics { get; set; }
    }

    public class StartGameRequest
    {
        public string? HeroClass { get; set; }
        public string? MonsterType { get; set; }
    }

    public class AttackRequest 
    { 
        public string? AttackType { get; set; } 
    }
}