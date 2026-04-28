using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Arena.Api.Application.Commands;
using Arena.Api.Application.Strategies.Ai;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;
using Arena.Api.Domain.Strategies;
using IAiStrategy = Arena.Api.Domain.Interfaces.IAiStrategy;

namespace Arena.Api.Application.Services
{
    public class GameSession
    {
        public Hero Player { get; private set; }
        public Monster Enemy { get; private set; }
        public bool IsGameOver { get; set; }
        public string LastMonsterAttackType { get; set; } = "Physical";
        
        public List<string> CombatLog { get; set; }
        public List<string> FullCombatLog { get; set; } 
        
        public int MonsterUltCharge { get; set; }
        public int HeroPotions { get; set; }
        public int LastHeroHealed { get; set; } 
        public int HeroDamageTaken { get; set; } 
        public bool HeroDefendedThisTurn { get; set; }
        public int HeroUltCharge { get; set; } 

        public int MonsterPotions { get; set; }
        public int LastMonsterHealed { get; set; }
        public int MonsterDamageTaken { get; set; } 
        public bool IsMonsterDefending { get; set; } 
        public bool MonsterDefendedThisTurn { get; set; } 
        public bool MonsterActedFirst { get; set; }

        public int HeroShieldDurability { get; set; } 
        public int HeroShieldCooldown { get; set; } 
        public int MonsterShieldDurability { get; set; } 
        public int MonsterShieldCooldown { get; set; } 

        public int RoundCount { get; set; }
        public string CurrentArenaEvent { get; set; }
        public int EventDuration { get; set; }
        public int RoundsUntilNextEvent { get; set; }
        public string TriggeredEventThisRound { get; set; }
        
        public int HeroMaxHpPenalty { get; set; }
        public int CurrentHeroMaxHp => Math.Max(1, Player.MaxHp - HeroMaxHpPenalty);
        public int MonsterMaxHpPenalty { get; set; }
        public int CurrentMonsterMaxHp => Math.Max(1, Enemy.MaxHp - MonsterMaxHpPenalty);
        public bool MonsterUsedBloodPactThisTurn { get; set; }

        public bool HeroInSecondWind { get; set; }
        public bool MonsterInSecondWind { get; set; }
        public bool HeroStolePotionThisTurn { get; set; }
        public bool MonsterStolePotionThisTurn { get; set; }

        public int MonsterDamageBuffDuration { get; set; }
        public int MonsterUltStealDuration { get; set; }

        // NOVAS MECÂNICAS DE CONTROLO (CC - Crowd Control)
        public bool MonsterIsStunned { get; set; }
        public int HeroPotionSilenceDuration { get; set; }

        public IAiStrategy MonsterAi { get; private set; }
        private static readonly Random rand = new Random();

        public GameSession(Hero player, Monster enemy)
        {
            Player = player; Enemy = enemy;
            IsGameOver = false; 
            CombatLog = new List<string>();
            FullCombatLog = new List<string>();
            
            MonsterUltCharge = 0; HeroPotions = 3; HeroUltCharge = 0;
            MonsterPotions = 2; HeroShieldDurability = 3; MonsterShieldDurability = 3;
            RoundCount = 0; CurrentArenaEvent = "Normal"; TriggeredEventThisRound = "";
            RoundsUntilNextEvent = rand.Next(1, 5);
            MonsterDamageBuffDuration = 0; MonsterUltStealDuration = 0;
            MonsterIsStunned = false; HeroPotionSilenceDuration = 0;

            if (Enemy.Name.Contains("Chefe", StringComparison.OrdinalIgnoreCase) || Enemy.Name.Contains("Boss", StringComparison.OrdinalIgnoreCase) || Enemy.Name.Contains("Tático", StringComparison.OrdinalIgnoreCase)) {
                MonsterAi = new LearningBossAiStrategy();
                MonsterPotions = 3; MonsterUltCharge = 1;
                CombatLog.Add($"👁️ O {Enemy.Name} entra na arena! A sua aura é pesada e a sua Ultimate já começa a brilhar...");
            } else {
                MonsterAi = new RuthlessAiStrategy();
            }

            CombatLog.Add($"A batalha entre {Player.Name} e {Enemy.Name} começou!");
        }

        public void ApplyBloodPact()
        {
            if (IsGameOver) return;
            if (CombatLog.Count > 0) { FullCombatLog.AddRange(CombatLog); FullCombatLog.Add("--- (Turno Extra: Pacto Sombrio) ---"); }
            CombatLog.Clear(); TriggeredEventThisRound = ""; 
            if (CurrentHeroMaxHp <= Player.MaxHp * 0.60) {
                CombatLog.Add($"⚠️ {Player.Name} tentou usar o Pacto de Sangue, mas o corpo está no limite!");
                return;
            }
            HeroMaxHpPenalty += Player.MaxHp * 40 / 100;

            // TRAVA DE SEGURANÇA: Garante que o HP Atual não excede o novo teto
            if (Player.CurrentHp > CurrentHeroMaxHp) Player.CurrentHp = CurrentHeroMaxHp;

            HeroUltCharge = 2; HeroShieldDurability = 1; HeroShieldCooldown = 0;
            CombatLog.Add($"🩸 PACTO DE SANGUE! {Player.Name} sacrificou HP Máximo pela Ultimate e Escudo!");
        }

        public void ExecuteMonsterBloodPact()
        {
            MonsterMaxHpPenalty += Enemy.MaxHp * 40 / 100;

            // TRAVA DE SEGURANÇA: Garante que o HP Atual não excede o novo teto
            if (Enemy.CurrentHp > CurrentMonsterMaxHp) Enemy.CurrentHp = CurrentMonsterMaxHp;

            MonsterUltCharge = 2; MonsterShieldDurability = 1; MonsterShieldCooldown = 0;
            MonsterUsedBloodPactThisTurn = true;
            CombatLog.Add($"🧠🩸 JOGADA DE MESTRE! {Enemy.Name} sacrificou HP Máximo para virar o jogo!");
        }

        public void PlayRound(string actionType, IAttackStrategy? heroAttack = null)
        {
            if (IsGameOver) return;
            if (CombatLog.Count > 0) { FullCombatLog.AddRange(CombatLog); FullCombatLog.Add($"--- Fim do Turno {RoundCount} ---"); }
            CombatLog.Clear(); 
            MonsterUsedBloodPactThisTurn = false; TriggeredEventThisRound = ""; 
            HeroStolePotionThisTurn = false; MonsterStolePotionThisTurn = false;
            LastHeroHealed = 0; LastMonsterHealed = 0; HeroDamageTaken = 0; MonsterDamageTaken = 0;
            HeroDefendedThisTurn = false; IsMonsterDefending = false; MonsterDefendedThisTurn = false;

            // 1. Verificação de Silêncio de Poções
            if (actionType == "Heal" && HeroPotionSilenceDuration > 0) {
                actionType = "Physical";
                CombatLog.Add("❌ [Maldição Espiritual] A aura negra impediu-te de beber a poção! Foste forçado a atacar!");
            }

            if (!Enemy.Name.Contains("Chefe", StringComparison.OrdinalIgnoreCase) && !Enemy.Name.Contains("Boss", StringComparison.OrdinalIgnoreCase) && !Enemy.Name.Contains("Tático", StringComparison.OrdinalIgnoreCase))
            {
                string context = "Normal";
                if (HeroUltCharge >= 2) context = "HasUlt";
                else if (Player.CurrentHp <= CurrentHeroMaxHp * 0.4) context = "LowHp";
                PlayerPatternTracker.RecordAction(context, actionType);
            }

            RoundCount++;

            var aiDecision = MonsterAi.DecideNextMove(this);
            string monsterActionType = aiDecision.ActionType;
            IAttackStrategy? monsterAttack = aiDecision.AttackStrategy;

            bool heroCanDefend = HeroShieldCooldown == 0 && HeroShieldDurability > 0 && CurrentArenaEvent != "MagneticStorm";
            bool monsterCanDefend = MonsterShieldCooldown == 0 && MonsterShieldDurability > 0 && CurrentArenaEvent != "MagneticStorm";
            
            if (actionType == "Defend" && heroCanDefend && monsterActionType == "Heal" && MonsterPotions > 0) HeroStolePotionThisTurn = true;
            if (monsterActionType == "Defend" && monsterCanDefend && actionType == "Heal" && HeroPotions > 0) MonsterStolePotionThisTurn = true;

            int heroPriority = (actionType == "Heal" || actionType == "Defend") ? 1 : 2;
            int monsterPriority = (monsterActionType == "Heal" || monsterActionType == "Defend") ? 1 : 2;
            MonsterActedFirst = (monsterPriority < heroPriority);

            var heroCommand = CommandFactory.Create(actionType);
            var monsterCommand = CommandFactory.Create(monsterActionType);

            // 2. Verificação de Atordoamento (Stun) do Monstro
            bool skipMonsterTurn = MonsterIsStunned;
            if (MonsterIsStunned) {
                CombatLog.Add($"💫 [Atordoado] O impacto do último ataque deixou {Enemy.Name} tonto! Ele perdeu o turno!");
                MonsterIsStunned = false; // Remove o Stun para a próxima rodada
            }

            // Memória de Ultimate antes de executar (para saber se o golpe foi usado)
            int prevHeroUlt = HeroUltCharge;
            int prevMonsterUlt = MonsterUltCharge;

            // Execução dos Comandos
            if (MonsterActedFirst) {
                if (!skipMonsterTurn) monsterCommand.Execute(this, false, monsterAttack);
                if (!Player.IsDead()) heroCommand.Execute(this, true, heroAttack);
            } else {
                heroCommand.Execute(this, true, heroAttack);
                if (!Enemy.IsDead() && !skipMonsterTurn) monsterCommand.Execute(this, false, monsterAttack);
            }
            LastMonsterAttackType = skipMonsterTurn ? "Stunned" : monsterActionType;

            // 3. APLICAÇÃO DOS EFEITOS CRIATIVOS DAS ULTIMATES
            if (actionType == "Ultimate" && HeroUltCharge < prevHeroUlt) {
                ApplyHeroCreativeUltimate();
            }
            if (!skipMonsterTurn && monsterActionType == "Ultimate" && MonsterUltCharge < prevMonsterUlt) {
                ApplyMonsterCreativeUltimate();
            }

            // TRAVA DE SEGURANÇA FINAL DA RODADA: O HP Atual não pode exceder as reduções da Ultimate
            if (Player.CurrentHp > CurrentHeroMaxHp) Player.CurrentHp = CurrentHeroMaxHp;
            if (Enemy.CurrentHp > CurrentMonsterMaxHp) Enemy.CurrentHp = CurrentMonsterMaxHp;

            // Ganho Passivo de Fúria dos Monstros
            if (!Enemy.IsDead() && !Player.IsDead() && RoundCount % 2 == 0 && MonsterUltCharge < 2) {
                MonsterUltCharge++;
                CombatLog.Add($"⚠️ A fúria de {Enemy.Name} está a aumentar... (Ultimate a carregar)");
            }

            // Fim da Rodada e Eventos
            if (Enemy.IsDead() || Player.IsDead()) {
                IsGameOver = true;
                CombatLog.Add(Enemy.IsDead() ? $"{Enemy.Name} foi obliterado! Vitória!" : $"{Player.Name} caiu... Fim de jogo.");
                ExportarTreinoJson();
            } else {
                // Reduzir contadores de Debuff
                if (HeroPotionSilenceDuration > 0) {
                    HeroPotionSilenceDuration--;
                    if (HeroPotionSilenceDuration == 0) CombatLog.Add("✨ A maldição espiritual dissipou-se. Já podes usar poções novamente.");
                }

                if (!HeroInSecondWind && Player.CurrentHp > 0 && Player.CurrentHp <= CurrentHeroMaxHp * 0.20) {
                    HeroInSecondWind = true; Player.Heal(CurrentHeroMaxHp * 25 / 100); 
                    TriggeredEventThisRound = "SecondWindHero";
                    CombatLog.Add($"🌪️ ÚLTIMO SUSPIRO! {Player.Name} recuperou HP e entrou num transe de adrenalina!");
                }
                if (!MonsterInSecondWind && Enemy.CurrentHp > 0 && Enemy.CurrentHp <= CurrentMonsterMaxHp * 0.20) {
                    MonsterInSecondWind = true; Enemy.Heal(CurrentMonsterMaxHp * 25 / 100);
                    TriggeredEventThisRound = TriggeredEventThisRound == "SecondWindHero" ? "SecondWindBoth" : "SecondWindMonster";
                    CombatLog.Add($"🌪️ ÚLTIMO SUSPIRO INIMIGO! {Enemy.Name} curou HP e enfureceu-se!");
                }

                if (HeroInSecondWind) { if (HeroUltCharge < 2) HeroUltCharge++; HeroMaxHpPenalty += Player.MaxHp * 5 / 100; }
                if (MonsterInSecondWind) { if (MonsterUltCharge < 2) MonsterUltCharge++; MonsterMaxHpPenalty += Enemy.MaxHp * 5 / 100; }

                if (CurrentArenaEvent == "HealingWinds" && !IsGameOver) {
                    Player.Heal(CurrentHeroMaxHp * 10 / 100); Enemy.Heal(CurrentMonsterMaxHp * 10 / 100);
                    CombatLog.Add($"🍃 Os Ventos Curativos restauraram vida a ambos os lutadores!");
                }

                if (MonsterDamageBuffDuration > 0) { MonsterDamageBuffDuration--; if (MonsterDamageBuffDuration == 0) CombatLog.Add("📉 A fúria dissipou-se."); }
                if (MonsterUltStealDuration > 0) { MonsterUltStealDuration--; }

                if (EventDuration > 0) {
                    EventDuration--;
                    if (EventDuration == 0) { CurrentArenaEvent = "Normal"; RoundsUntilNextEvent = rand.Next(1, 5); CombatLog.Add("🌍 A poeira assenta e as regras da arena voltam ao normal.");}
                }
                if (HeroShieldCooldown > 0) { HeroShieldCooldown--; if (HeroShieldCooldown == 0) { HeroShieldDurability = 3; CombatLog.Add($"✨ O escudo de {Player.Name} foi restaurado!"); } }
                if (MonsterShieldCooldown > 0) { MonsterShieldCooldown--; if (MonsterShieldCooldown == 0) { MonsterShieldDurability = 3; CombatLog.Add($"✨ O escudo de {Enemy.Name} foi restaurado!"); } }

                if (CurrentArenaEvent == "Normal") {
                    RoundsUntilNextEvent--;
                    if (RoundsUntilNextEvent <= 0) {
                        int r = rand.Next(8);
                        string[] evs = {"ToxicGas", "MagneticStorm", "ManaBlessing", "BloodFrenzy", "HealingWinds", "ManaVoid", "Supplies", "DivineAegis"};
                        CurrentArenaEvent = evs[r]; EventDuration = r < 5 ? 2 : 0;
                        TriggeredEventThisRound = evs[r];
                        if (r==0) CombatLog.Add("☠️ EVENTO DE ARENA: Gás Tóxico inundará o campo!");
                        else if (r==1) { HeroShieldDurability = 0; HeroShieldCooldown = 2; MonsterShieldDurability = 0; MonsterShieldCooldown = 2; CombatLog.Add("⚡ EVENTO DE ARENA: Tempestade Magnética ativada! Escudos estilhaçados!"); }
                        else if (r==2) CombatLog.Add("✨ EVENTO DE ARENA: Bênção de Mana! As Ultimates carregam rápido!");
                        else if (r==3) CombatLog.Add("🩸 EVENTO DE ARENA: Frenesi de Batalha! (+50% de dano)");
                        else if (r==4) CombatLog.Add("🍃 EVENTO DE ARENA: Ventos Curativos ativados!");
                        else if (r==5) { HeroUltCharge=0; MonsterUltCharge=0; CombatLog.Add("🌌 EVENTO DE ARENA (Instantâneo): Vazio de Mana sugou a Ultimate de ambos!"); }
                        else if (r==6) { HeroPotions++; MonsterPotions++; CombatLog.Add("🎁 EVENTO DE ARENA (Instantâneo): Suprimentos! (+1 Poção para cada)"); }
                        else if (r==7) { HeroShieldDurability=3; MonsterShieldDurability=3; HeroShieldCooldown=0; MonsterShieldCooldown=0; CombatLog.Add("🛡️ EVENTO DE ARENA (Instantâneo): Égide Divina restaurou os Escudos!"); }
                    }
                }
            }
        }

        private void ApplyHeroCreativeUltimate() {
            if (Player.Name.Contains("Maga", StringComparison.OrdinalIgnoreCase) || Player.Name.Contains("Mago", StringComparison.OrdinalIgnoreCase)) {
                MonsterMaxHpPenalty += Enemy.MaxHp * 15 / 100;
                CombatLog.Add("💥 [Efeito: Explosão Arcana] A magia distorce a realidade! O HP Máximo do monstro foi reduzido permanentemente em 15%!");
            }
            else if (Player.Name.Contains("Arqueiro", StringComparison.OrdinalIgnoreCase)) {
                MonsterIsStunned = true;
                CombatLog.Add("🎯 [Efeito: Chuva de Flechas] Uma flecha perfurou um ponto vital! O monstro está ATORDOADO e perderá o próximo turno!");
            }
            else if (Player.Name.Contains("Rei", StringComparison.OrdinalIgnoreCase)) {
                HeroShieldDurability = 3; HeroShieldCooldown = 0;
                Player.Heal(Player.MaxHp * 15 / 100);
                CombatLog.Add("👑 [Efeito: Golpe de Estado] A majestade do Rei impõe respeito! Ele curou feridas e restaurou o seu Escudo na totalidade!");
            }
        }

        private void ApplyMonsterCreativeUltimate() {
            if (Enemy.Name.Contains("Sombrio", StringComparison.OrdinalIgnoreCase)) {
                Enemy.Heal(Enemy.MaxHp * 20 / 100);
                CombatLog.Add("🦇 [Efeito: Drenagem Vital] Magia de sangue! O Mago Sombrio sugou a tua força vital para se curar massivamente!");
            }
            else if (Enemy.Name.Contains("Caçadora", StringComparison.OrdinalIgnoreCase)) {
                HeroShieldDurability = 0; HeroShieldCooldown = 2;
                CombatLog.Add("🦅 [Efeito: Tiro Sombrio] A flecha perfurou as tuas defesas críticas! O teu ESCUDO FOI COMPLETAMENTE ESTILHAÇADO!");
            }
            else if (Enemy.Name.Contains("Verme", StringComparison.OrdinalIgnoreCase)) {
                if (HeroPotions > 0) {
                    HeroPotions--;
                    CombatLog.Add("🪱 [Efeito: Terremoto Devorador] O tremor engoliu os teus mantimentos! PERDESTE 1 POÇÃO PARA SEMPRE!");
                }
            }
            else if (Enemy.Name.Contains("Guerreiro", StringComparison.OrdinalIgnoreCase)) {
                HeroMaxHpPenalty += Player.MaxHp * 15 / 100;
                CombatLog.Add("🪓 [Efeito: Golpe Fendido] O machado mutilou a tua armadura! O teu HP Máximo foi esmagado permanentemente!");
            }
            else if (Enemy.Name.Contains("Xamã", StringComparison.OrdinalIgnoreCase)) {
                HeroPotionSilenceDuration = 3; // 3 rodadas contando com o tick de fim de turno
                CombatLog.Add("💀 [Efeito: Maldição Espiritual] Uma aura negra envolve as tuas poções. Estás IMPEDIDO de te curar durante 2 turnos!");
            }
            else if (Enemy.Name.Contains("Tático", StringComparison.OrdinalIgnoreCase) || Enemy.Name.Contains("Boss", StringComparison.OrdinalIgnoreCase) || Enemy.Name.Contains("Chefe", StringComparison.OrdinalIgnoreCase)) {
                
                // Redução de 50% de HP Máximo
                HeroMaxHpPenalty += CurrentHeroMaxHp / 2;
                CombatLog.Add("⚖️ [Efeito: Execução Tática] O Chefe esmagou 50% da tua vitalidade máxima permanentemente!");
                
                // Mantém o roubo de fúria se o jogador a tiver
                if (HeroUltCharge > 0) {
                    HeroUltCharge = 0;
                    if (MonsterUltCharge < 2) MonsterUltCharge++;
                    CombatLog.Add("⚖️ Além disso, ele leu o teu desespero e ROUBOU toda a tua carga de Ultimate!");
                } else {
                    CombatLog.Add("⚖️ Ele tentou roubar a tua Ultimate, mas não tinhas fúria nenhuma!");
                }
            }
        }

        private void ExportarTreinoJson()
        {
            try
            {
                var logFinalDaLuta = new List<string>(FullCombatLog);
                logFinalDaLuta.AddRange(CombatLog);

                var relatorio = new
                {
                    Data = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Batalha = $"{Player.Name} vs {Enemy.Name}",
                    Vencedor = Player.IsDead() ? Enemy.Name : Player.Name,
                    TotalTurnos = RoundCount,
                    CerebroDaIA = new 
                    {
                        VezesQueOJogadorCurouNoDesespero = PlayerPatternTracker.LowHpHeals,
                        VezesQueOJogadorAtacouNoDesespero = PlayerPatternTracker.LowHpAttacks,
                        Conclusao_CurarNoDesespero = PlayerPatternTracker.PredizCuraNoDesespero(),
                        VezesQueOJogadorDefendeuDaUlt = PlayerPatternTracker.BossHasUltDefends,
                        VezesQueOJogadorIgnorouAUlt = PlayerPatternTracker.BossHasUltOtherActions,
                        Conclusao_EscudoNoMedo = PlayerPatternTracker.PredizEscudoNoMedo()
                    },
                    HistoricoDeTurnos = logFinalDaLuta
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(relatorio, options);
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "LogsDeTreino");
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                string fileName = $"Treino_{Enemy.Name.Replace(" ", "")}_{DateTime.Now:HHmmss_ffff}.json";
                string fullPath = Path.Combine(directoryPath, fileName);
                File.WriteAllText(fullPath, jsonString);
                Console.WriteLine($"[Arena] JSON de Treino exportado para a raiz: {fullPath}");
            }
            catch (Exception ex) { Console.WriteLine($"[Arena] Erro ao gerar JSON de treino: {ex.Message}"); }
        }
    }
}