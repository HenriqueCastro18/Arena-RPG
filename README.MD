<div align="center">

# ⚔️ Arena RPG — Arena Viva

<video src="https://github.com/user-attachments/assets/4c91ef8d-3e2f-48d1-abe8-603aeb33a1b5" width="100%" controls></video>

<br/>

> *Um motor de combate por turnos onde a IA não joga aleatoriamente — ela te estuda, aprende os teus padrões e usa-os contra ti.*

<br/>

![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)
![Firebase](https://img.shields.io/badge/Firebase-FFCA28?style=for-the-badge&logo=firebase&logoColor=black)
![Render](https://img.shields.io/badge/Render-46E3B7?style=for-the-badge&logo=render&logoColor=black)
![Vercel](https://img.shields.io/badge/Vercel-000000?style=for-the-badge&logo=vercel&logoColor=white)

</div>

---

## 📖 Sobre o Projeto

**Arena RPG** é um jogo de combate por turnos construído do zero — sem game engines, sem bibliotecas de UI. O motor gráfico com sprites animados em Canvas, o sistema de combate e a IA foram todos implementados manualmente.

O diferencial está na profundidade estratégica: cada ação consome recursos limitados, cada monstro tem habilidades únicas, e o boss final **aprende o teu estilo de jogo** ao longo das batalhas e adapta o comportamento para te derrotar.

---

## 🎮 Como Jogar

O combate é por turnos simultâneos. Tu escolhes uma ação, a IA decide a ação do monstro ao mesmo tempo. As ações defensivas têm prioridade sobre os ataques. O jogo termina quando o HP de qualquer lado chega a zero.

---

## ⚙️ Recursos de Combate

<table>
<tr>
<td width="50%">

### 🛡️ Escudo — 3 Cargas
Defende reduz **60% do dano**. Cada uso gasta 1 carga. Ultimates inimigas gastam uma carga extra ao impactar. Com 0 cargas, entra em cooldown de **3 turnos**.

</td>
<td width="50%">

### ⚡ Ultimate — 2 Cargas (herói)
Só carrega quando o ataque **acerta**. Esquivas do inimigo bloqueiam o carregamento. Cada herói e monstro tem um efeito único ao usar a Ultimate.

</td>
</tr>
<tr>
<td width="50%">

### 🧪 Poções — 3 por Combate
Curam **40% do HP máximo efetivo**. Se o inimigo usar Defender enquanto bebes uma poção, ele **rouba-a** e cura-se no lugar.

</td>
<td width="50%">

### 💨 Esquiva — 2 por Combate
Cancela **100% do dano** e os efeitos secundários das Ultimates (maldições, roubo de escudo, roubo de poção). Monstros também esquivam.

</td>
</tr>
</table>

### 🩸 Pacto Sombrio — 1 Uso por Batalha
Sacrifica permanentemente **40% do HP máximo** em troca de +1 carga de Ultimate e +1 carga de Escudo. Ação de desespero — usares quando a situação é crítica.

---

## 🧙 Heróis

| Herói | HP | ATK | Ultimate |
|---|---|---|---|
| ⚔️ **Arthur** — Guerreiro | 1150 | 140 | Ataque pesado sem efeito secundário |
| 🔮 **Maga Arcana** — Mago | 1000 | 185 | **Explosão Arcana** — Reduz o HP máximo do inimigo em 15% permanentemente |
| 🏹 **Robin** — Arqueiro | 1090 | 150 | **Chuva de Flechas** — Atordoa o inimigo, fazendo-o perder o próximo turno |
| 👑 **Rei Soberano** — Rei | 1200 | 130 | **Golpe de Estado** — Cura 15% do HP máximo e restaura o Escudo para 3/3 |

---

## 👾 Inimigos

| Monstro | HP | ATK | Ultimate |
|---|---|---|---|
| 🐗 Orc Selvagem | 540 | 148 | — |
| 🏹 Caçadora Escura | 520 | 170 | **Tiro Sombrio** — Estilhaça completamente o escudo do herói |
| 🧙 Mago Sombrio | 570 | 168 | **Drenagem Vital** — Cura-se em 20% do seu HP máximo |
| 🪱 Verme Gigante | 680 | 145 | **Terremoto Devorador** — Remove 1 poção do herói |
| 🪓 Minotauro Guerreiro | 750 | 165 | **Golpe Fendido** — Reduz o HP máximo do herói em 15% permanentemente |
| 🔮 Minotauro Xamã | 820 | 178 | **Maldição Espiritual** — Impede o uso de poções por 2 turnos |

### 🐂 Minotauro Chefe — Boss Adaptativo

**Stats base:** 1450 HP / 130 ATK · Começa com 1 carga de Ultimate e 3 poções.

**Ultimate — Pacto Sombrio:** Drena **35% do HP máximo** permanentemente e consome toda a carga de Ultimate do herói. Bloqueável com Escudo ou Esquiva.

O Boss surge em **4 variantes** determinadas pelo teu histórico de batalha:

| Variante | Condição | Bónus |
|---|---|---|
| 🐂 Minotauro Chefe | Sem histórico (sorteado) | — |
| ⚔️ Minotauro Tático | Jogador agressivo | +200 HP / +30 ATK |
| 💥 Minotauro Quebra-Guarda | Jogador defensivo | +50 ATK especializado em escudos |
| ⚖️ Minotauro Implacável | Jogador equilibrado | +150 HP / +35 ATK |

---

## 🧠 Inteligência Artificial

### Monstros Comuns — `RuthlessAiStrategy`
Decide em tempo real com base no estado do combate: interceptar poções, esquivar de Ultimates, curar em HP crítico ou atacar.

### Boss — `LearningBossAiStrategy`

Opera em 3 fases à medida que perde HP:

| Fase | HP do Boss | Comportamento |
|---|---|---|
| **1 — Aprendizagem** | > 65% | Metódico, conservador, observa o jogador |
| **2 — Adaptação** | 35–65% | Usa os dados do `PlayerPatternTracker` para prever e contra-atacar |
| **3 — Enraivecido** | < 35% | Caótico e perigoso — bloqueia Ultimates com 85% de chance |

### `PlayerPatternTracker` — O Cérebro

Regista o teu comportamento durante toda a sessão e usa-o para alimentar a IA do Boss:

- **Frequência de cura com HP baixo** → Boss decide interceptar poções
- **Frequência de defesa com Ultimate do Boss pronta** → Boss decide guardar ou lançar
- **Score de agressividade** → Boss calibra bloqueios
- **Sequências de ações** → Boss prediz a tua próxima jogada

Os dados de treino são guardados automaticamente no **Firebase Firestore** ao final de cada batalha.

---

## 🌍 Arena Viva — Eventos Aleatórios

A cada 2–5 turnos, a arena altera as regras com um evento:

| Evento | Duração | Efeito |
|---|---|---|
| ☠️ Gás Tóxico | 2 turnos | Poções ineficazes para ambos |
| ⚡ Tempestade Magnética | Instantâneo | Destrói todos os escudos |
| ✨ Bênção de Mana | 2 turnos | Ataques carregam +2 de Ultimate |
| 🩸 Frenesi de Batalha | 2 turnos | +50% de dano físico |
| 🍃 Ventos Curativos | 2 turnos | Ambos recuperam 10% do HP por turno |
| 🌌 Vazio de Mana | Instantâneo | Reseta a Ultimate de ambos para 0 |
| 🎁 Suprimentos | Instantâneo | +1 poção para cada lado |
| 🛡️ Égide Divina | Instantâneo | Restaura os escudos de ambos (3/3) |

---

## 🌪️ Último Suspiro

Quando qualquer combatente cai abaixo de **20% do HP máximo**, ativa automaticamente (uma vez por combate): cura **25% do HP** e ganha carga de Ultimate extra por turno — mas o HP máximo diminui 5% a cada turno enquanto ativo.

---

## 🏰 Modo Masmorra

6 salas consecutivas sem restaurar HP entre elas. O Boss da Sala 6 já tem o teu perfil psicológico completo.

```
Sala 1 → Verme Gigante       Sala 4 → Minotauro Guerreiro
Sala 2 → Mago Sombrio        Sala 5 → Minotauro Xamã
Sala 3 → Caçadora Escura     Sala 6 → Minotauro Chefe (Boss Adaptativo)
```

---

## 🛠️ Arquitetura

Construído com princípios **SOLID** e os seguintes padrões de design:

| Padrão | Onde é Usado |
|---|---|
| **Command** | `AttackCommand`, `DefendCommand`, `HealCommand`, `DodgeCommand` via `CommandFactory` |
| **Strategy** | `PhysicalAttack`, `UltimateAttack`, `RuthlessAiStrategy`, `LearningBossAiStrategy` |
| **Factory** | `CharacterFactory` (heróis e monstros), `CommandFactory` (ações) |
| **Dependency Injection** | `ITrainingLogService` — `FirestoreTrainingLogService` em produção, `NullTrainingLogService` local |

```
Arena.Api/
├── Application/
│   ├── Commands/          → Command Pattern (ações de combate)
│   ├── Services/          → GameSession (motor) · GameManager (sessões)
│   └── Strategies/Ai/     → RuthlessAiStrategy · LearningBossAiStrategy
├── Domain/
│   ├── Entities/          → Hero · Monster · TrainingReport
│   ├── Factories/         → CharacterFactory
│   ├── Interfaces/        → IAttackStrategy · IAiStrategy · ITrainingLogService
│   └── Strategies/        → PhysicalAttack · UltimateAttack
├── Infrastructure/
│   └── Firebase/          → FirestoreTrainingLogService · NullTrainingLogService
└── Controllers/           → GameController (REST API)
```

---

## ☁️ Infraestrutura de Produção

| Camada | Plataforma | Função |
|---|---|---|
| 🌐 **Frontend** | [Vercel](https://vercel.com) | Hospedagem estática — deploy automático via GitHub |
| ⚙️ **API** | [Render](https://render.com) | Backend .NET 10 via Docker — free tier sem cartão |
| 🔥 **Dados de Treino** | [Firebase Firestore](https://firebase.google.com) | Persistência dos logs de IA por batalha |

---

## 🚀 Executar Localmente

**Pré-requisitos:** .NET 10 SDK · VS Code + Live Server

```bash
# 1. API
cd Arena.Api
dotnet run
# Servidor em http://localhost:8080

# 2. Frontend
# Abre Arena.Web/ no VS Code → Go Live
```

### Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/game/start` | Inicia nova sessão de combate |
| `POST` | `/api/game/{id}/attack` | Executa ação do jogador |
| `POST` | `/api/game/{id}/bloodpact` | Ativa o Pacto Sombrio |
| `POST` | `/api/game/simulate` | Simula 100 batalhas (balanceamento) |

---

## 👨‍💻 Desenvolvedor

<div align="center">

**Henrique Castro**
Backend Developer — C# · .NET · Software Engineering

<br/>

[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/HenriqueCastro18)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/henrique-castro-b13798345)

</div>
