# ⚔️ Arena RPG — Arena Viva

### 🎮 Demonstração do Projeto

<div align="center">
  <video src="https://github.com/user-attachments/assets/4c91ef8d-3e2f-48d1-abe8-603aeb33a1b5" width="100%" controls></video>

  <br><br>

  <img src="https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" />
  <img src="https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white" />
  <img src="https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white" />
  <img src="https://img.shields.io/badge/Firebase-FFCA28?style=for-the-badge&logo=firebase&logoColor=black" />
  <img src="https://img.shields.io/badge/Railway-0B0D0E?style=for-the-badge&logo=railway&logoColor=white" />
  <img src="https://img.shields.io/badge/Vercel-000000?style=for-the-badge&logo=vercel&logoColor=white" />

  <br><br>

  > Um motor de combate por turnos onde a inteligência artificial não joga aleatoriamente — ela te estuda, aprende os teus padrões e usa-os contra ti.

</div>

---

## 📖 O que é este projeto?

**Arena RPG** é um jogo de combate por turnos desenvolvido do zero com uma API em C# no backend e uma interface visual em HTML/CSS/JS no frontend. Não há biblioteca de game engine — o motor gráfico (sprites animados em Canvas), o sistema de combate e a IA foram todos implementados manualmente.

O núcleo do projeto está na profundidade estratégica: cada ação gasta recursos específicos, cada monstro tem habilidades únicas, e o boss final adapta o seu comportamento com base no que o jogador fez nas salas anteriores.

---

## 🎮 Como o Jogo Funciona

### Estrutura de uma Partida

O combate é por turnos alternados. Em cada turno, **o jogador escolhe uma ação** e a IA decide a ação do monstro simultaneamente. As ações de cada lado são executadas em ordem de prioridade (ações defensivas têm precedência sobre ataques). No final do turno, os efeitos são aplicados, os recursos são atualizados e o log de combate regista tudo.

O jogo termina quando o HP de qualquer lado chega a 0. Não há revives — a única exceção é o **Último Suspiro** (ver abaixo).

---

## ⚙️ Sistema de Recursos

Cada combatente (herói e monstros) gere um conjunto de recursos limitados. Gerir estes recursos corretamente **é a chave para vencer**.

### 🛡️ Escudo — 3 Cargas por Combate

Usar a ação **Defender** gasta sempre 1 carga de escudo, independentemente de o inimigo atacar ou não. Ao bloquear, o dano recebido é reduzido em **60%**. Um ataque Ultimate do inimigo gasta uma carga extra do escudo ao impactar.

Quando as 3 cargas se esgotam, o escudo entra em **cooldown de 3 turnos**, ficando completamente inutilizável. Ao restaurar, volta com as 3 cargas completas.

| Situação | Custo |
|---|---|
| Usar Defender | −1 carga (sempre) |
| Levar um Ultimate enquanto defende | −1 carga adicional |
| Escudo com 0 cargas | Em recarregamento (3 turnos) |

### ⚡ Ultimate — 2 Cargas (Herói) / 3 Cargas (Monstros)

A Ultimate carrega **apenas quando o ataque acerta**. Se o inimigo esquivar, nenhuma carga é ganha. Usar a Ultimate consome toda a carga acumulada, independentemente de o ataque acertar ou errar.

Cada personagem tem uma Ultimate com efeito único (ver secção de personagens).

### 🧪 Poções — 3 por Combate

Curam **40% do HP máximo efetivo** (não do HP base — penalidades permanentes de HP máximo são respeitadas). A cura nunca excede o teto máximo atual.

> **Mecânica de Roubo:** Se o jogador tenta curar enquanto o inimigo usa Defender, a poção é interceptada e **o inimigo bebe a poção roubada**, curando-se às tuas custas.

### 💨 Esquiva — 2 por Combate

A Esquiva **cancela 100% do dano** do próximo ataque recebido, incluindo os efeitos secundários das Ultimates (escudo estilhaçado, roubo de poção, maldições). Ao esquecer uma esquiva, nenhuma carga de Ultimate é ganha pelo atacante.

Os **monstros também usam esquiva** com as mesmas regras e limite de 2 por combate. A IA decide quando esquivar com base no estado do combate.

### 🩸 Pacto Sombrio — 1 Uso por Combate

Uma ação desesperada. Sacrifica permanentemente **40% do HP máximo** em troca de +1 carga de Ultimate e +1 carga de Escudo. O HP atual é ajustado se exceder o novo teto. Só pode ser usado uma vez por batalha.

---

## 🧙 Personagens Jogáveis

### Arthur — Guerreiro *(padrão)*
- **Stats:** 1150 HP / 140 ATK
- **Passiva:** Nenhuma especial — resistência pura.
- **Ultimate:** Ataque pesado sem efeito secundário.

### Maga Arcana — Mago
- **Stats:** 1000 HP / 185 ATK
- **Passiva — Escudo de Mana:** 20% de chance de anular completamente o dano de uma Ultimate inimiga.
- **Ultimate — Explosão Arcana:** Reduz permanentemente o HP máximo do inimigo em **15%**.

### Robin — Arqueiro
- **Stats:** 1090 HP / 150 ATK
- **Passiva:** Precisão — dano elevado base.
- **Ultimate — Chuva de Flechas:** **Atordoa** o inimigo, forçando-o a perder o próximo turno.

### Rei Soberano — Rei
- **Stats:** 1200 HP / 130 ATK
- **Passiva:** — Nenhuma
- **Ultimate — Golpe de Estado:** Cura **15% do HP máximo efetivo** e restaura o Escudo para 3/3, desde que acerte em cheio.

---

## 👾 Monstros e Chefe

### Monstros Comuns

| Monstro | HP | ATK | Elemento | Ultimate |
|---|---|---|---|---|
| Orc Selvagem | 540 | 148 | Terra | — |
| Caçadora Escura | 520 | 170 | Trevas | **Tiro Sombrio** — Estilhaça completamente o escudo do herói (esquivável) |
| Mago Sombrio | 570 | 168 | Trevas | **Drenagem Vital** — Cura o Mago em 20% do seu HP máximo |
| Verme Gigante | 680 | 145 | Terra | **Terremoto Devorador** — Remove 1 poção do inventário do herói (esquivável) |
| Minotauro Guerreiro | 750 | 165 | Físico | **Golpe Fendido** — Reduz permanentemente o HP máximo do herói em 15% |
| Minotauro Xamã | 820 | 178 | Mágico | **Maldição Espiritual** — Impede o uso de poções por 2 turnos |

### Minotauro Chefe — Boss Adaptativo

- **Stats base:** 1450 HP / 130 ATK
- **Ultimate — Pacto Sombrio:** Drena **35% do HP máximo efetivo** permanentemente e consome toda a carga de Ultimate do herói. Bloqueável com Escudo ou Esquiva.

O Boss tem **4 variantes**, determinadas pelo comportamento histórico do jogador:

| Estilo do Jogador | Variante | Modificação |
|---|---|---|
| Sem histórico | **Sorteado aleatoriamente** | Qualquer das 4 variantes |
| Agressivo (muitos ataques) | **Minotauro Tático** | +200 HP, +30 ATK |
| Defensivo (usa muito escudo) | **Minotauro Quebra-Guarda** | +50 ATK (especializado em quebrar escudos) |
| Equilibrado | **Minotauro Implacável** | +150 HP, +35 ATK |

O Boss começa o combate com **1 carga de Ultimate** e **3 poções**, possuindo a IA mais avançada do jogo.

---

## 🧠 Sistema de Inteligência Artificial

### IA Geral — `RuthlessAiStrategy`

Utilizada pelos monstros comuns. Toma decisões baseadas no estado atual do combate:

1. **Usar Ultimate** se carregada (com análise de contexto)
2. **Esquivar** se o herói tem Ultimate pronta (40% de chance)
3. **Interceptar poção** se o herói tem pouca vida e ainda tem poções
4. **Curar** se o próprio HP está abaixo de 45% e tem poções
5. **Defender** preventivamente se o herói tem Ultimate pronta
6. **Atacar** como ação padrão

### IA do Boss — `LearningBossAiStrategy`

Opera em **3 fases** conforme o HP do Boss decresce:

**Fase 1 (HP > 65%)** — *Aprendizagem*
O Boss é metódico e conservador. Cura com antecedência (HP < 55%) e observa o jogador.

**Fase 2 (35–65% HP)** — *Adaptação*
O Boss começa a usar os dados do `PlayerPatternTracker` contra o jogador:
- Se o jogador é agressivo → aumenta a frequência de bloqueios
- Se o jogador tende a defender quando a Ultimate está pronta → guarda a Ultimate
- Prediz a próxima ação com base em sequências históricas

**Fase 3 (HP < 35%)** — *Enraivecido*
O Boss torna-se caótico mas perigoso: alta prioridade para bloquear Ultimates (85%), curas de emergência e esquivas inteligentes.

### `PlayerPatternTracker` — O Cérebro da IA

| Dado Rastreado | Como é Usado |
|---|---|
| Frequência de cura com HP baixo | Boss decide interceptar poções |
| Frequência de defesa quando Boss tem Ultimate | Boss decide guardar ou lançar a Ultimate |
| Score de agressividade | Boss calibra a frequência de bloqueios |
| Sequências de ações (ex: defender→atacar) | Boss prediz e antecipa a próxima ação |

No final de cada batalha, todos estes dados são enviados para o **Firebase Firestore** para persistência e análise futura.

---

## 🌍 Arena Viva — Eventos Aleatórios

A cada 2–5 turnos, a arena altera as regras do jogo com um evento.

| Evento | Tipo | Efeito |
|---|---|---|
| ☠️ Gás Tóxico | 2 turnos | Poções são completamente ineficazes para ambos |
| ⚡ Tempestade Magnética | Instantâneo | Destrói todos os escudos (herói + monstro) |
| ✨ Bênção de Mana | 2 turnos | Ataques carregam +2 de Ultimate em vez de +1 |
| 🩸 Frenesi de Batalha | 2 turnos | +50% de dano em todos os ataques físicos |
| 🍃 Ventos Curativos | 2 turnos | Ambos os lados recuperam 10% do HP máximo por turno |
| 🌌 Vazio de Mana | Instantâneo | Reseta a carga de Ultimate de ambos os lados para 0 |
| 🎁 Suprimentos | Instantâneo | Ambos recebem +1 poção |
| 🛡️ Égide Divina | Instantâneo | Restaura os escudos de ambos (3/3, cooldown zerado) |

---

## 🌪️ Último Suspiro — Second Wind

Quando qualquer combatente cai abaixo de **20% do HP máximo efetivo**, o **Último Suspiro** é ativado automaticamente (uma vez por combate):

- Cura **25% do HP máximo efetivo** instantaneamente
- Ganha carga de Ultimate extra a cada turno enquanto ativo
- O HP máximo diminui 5% por turno (o transe tem custo)

---

## 🏰 Modo Masmorra

Enfrenta 6 salas consecutivas em ordem crescente de dificuldade:

```
Sala 1 → Verme Gigante
Sala 2 → Mago Sombrio
Sala 3 → Caçadora Escura
Sala 4 → Minotauro Guerreiro
Sala 5 → Minotauro Xamã
Sala 6 → Minotauro Chefe (Boss Adaptativo)
```

O HP do herói **não é restaurado entre salas**. O Boss da Sala 6 já terá o perfil psicológico completo do jogador, construído ao longo de todas as salas anteriores.

---

## 🛠️ Arquitetura e Design Patterns

O backend foi construído aplicando princípios **SOLID** e os seguintes padrões de design:

### Command Pattern
Cada ação de combate (`AttackCommand`, `DefendCommand`, `HealCommand`, `DodgeCommand`) implementa `ICombatCommand`. O `CommandFactory` instancia o comando correto em runtime. Isso desacopla a lógica de execução do motor de combate.

### Strategy Pattern
`PhysicalAttack` e `UltimateAttack` implementam `IAttackStrategy`. `RuthlessAiStrategy` e `LearningBossAiStrategy` implementam `IAiStrategy`. O `GameSession` invoca a estratégia correta sem conhecer os detalhes de implementação.

### Factory Pattern
`CharacterFactory` cria heróis e monstros. `CommandFactory` instancia comandos de combate. Ambos centralizam a criação de objetos.

### Dependency Injection
`ITrainingLogService` é injetado no controller. Em produção usa `FirestoreTrainingLogService`; localmente usa `NullTrainingLogService` (no-op). Nenhuma dependência hardcoded.

```
Arena.Api/
├── Application/
│   ├── Commands/          → Ações de combate (Command Pattern)
│   ├── Services/          → GameSession (motor) + GameManager (sessões)
│   └── Strategies/Ai/     → RuthlessAiStrategy + LearningBossAiStrategy
├── Domain/
│   ├── Entities/          → Hero, Monster, Character, TrainingReport
│   ├── Factories/         → CharacterFactory
│   ├── Interfaces/        → IAttackStrategy, IAiStrategy, ITrainingLogService
│   └── Strategies/        → PhysicalAttack, UltimateAttack
├── Infrastructure/
│   └── Firebase/          → FirestoreTrainingLogService + NullTrainingLogService
└── Controllers/           → GameController (endpoints REST)
```

---

## 🚀 Como Executar (Local)

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- VS Code com extensão **Live Server**

### 1. Backend — API

```bash
cd Arena.Api
dotnet run
```

O servidor inicia em `http://localhost:8080`.

### 2. Frontend — Interface

Abre `Arena.Web/` no VS Code e clica em **Go Live**. O `config.js` já aponta para localhost por defeito no desenvolvimento.

### Endpoints da API

| Método | Endpoint | Descrição |
|---|---|---|
| `POST` | `/api/game/start` | Inicia uma nova sessão de combate |
| `POST` | `/api/game/{id}/attack` | Executa uma ação do jogador |
| `POST` | `/api/game/{id}/bloodpact` | Ativa o Pacto Sombrio |
| `POST` | `/api/game/simulate` | Simula 100 batalhas (balanceamento) |

---

## ☁️ Deploy em Produção

### Stack
| Camada | Plataforma | Motivo |
|---|---|---|
| Frontend | **Vercel** | Static hosting gratuito |
| API | **Fly.io** | Free tier com VM sempre ativa (sem cold start — crítico para sessões em memória) |
| Logs de IA | **Firebase Firestore** | Persistência dos dados de treino |

---

### API — Fly.io

#### Pré-requisitos
```bash
# Windows (PowerShell)
winget install flyctl
# ou
powershell -c "iwr https://fly.io/install.ps1 -useb | iex"
```

#### Deploy

```bash
fly auth login

cd Arena.Api

# Cria o app no Fly.io (pede nome único — altere "arena-rpg-api" se necessário)
fly launch --copy-config --no-deploy

# Variáveis de ambiente (Firebase)
fly secrets set FIREBASE_PROJECT_ID="seu-project-id"
fly secrets set FIREBASE_CREDENTIALS='COLE_AQUI_O_CONTEUDO_COMPLETO_DO_JSON'

# Deploy
fly deploy
```

A URL da API aparece no final do output (`https://arena-rpg-api.fly.dev`).

> **Onde ficam as variáveis:** Fly.io → Dashboard do app → **Secrets** (ou via `fly secrets list`)

#### Após o deploy
Abre `Arena.Web/config.js` e substitui a URL:
```js
window.CONFIG = Object.freeze({
    API_URL: "https://arena-rpg-api.fly.dev/api/game"
});
```

---

### Frontend — Vercel

1. Vercel Dashboard → **New Project** → importa `Arena-RPG`
2. **Root Directory** → `ArenaProject`
3. Deploy — o `vercel.json` configura tudo automaticamente

---

## 👨‍💻 Desenvolvedor

**Henrique Castro**  
Backend Developer — C# / .NET

<div align="center">
  <a href="https://github.com/HenriqueCastro18">
    <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub">
  </a>
  <a href="https://www.linkedin.com/in/henrique-castro-b13798345">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn">
  </a>
</div>
