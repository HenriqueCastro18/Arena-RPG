# ⚔️ Arena RPG: Arena Viva


### 🎮 Demonstração do Projeto

![Preview do Jogo](https://github.com/HenriqueCastro18/Arena-RPG/raw/main/Arena.Web/assets/Preview%20RPG%20ARENA.mp4)

---
<br>

<div align="center">
  <img src="./image/banner_arena.png" alt="Arena RPG Gameplay" width="90%">
  <p><i>Motor de combate por turnos com IA Preditiva e Arquitetura Escalável.</i></p>
</div>

---

## 🌟 Diferenciais Técnicos

### 1. Inteligência Artificial Preditiva (Hardcore AI) 🧠
Diferente de RPGs convencionais, os monstros não utilizam ataques aleatórios. O motor utiliza a classe `PlayerPatternTracker` para monitorar o comportamento do usuário:
*   **Leitura de Cura:** Se o jogador tende a curar-se com menos de 20% de HP, o Boss entra em postura defensiva para interceptar a ação.
*   **Análise de Risco:** A IA decide poupar habilidades especiais (Ultimates) se prever que o jogador ativará um escudo, otimizando o gasto de mana.

### 2. Arena Viva e Masmorras Dinâmicas ⛈️
*   **Eventos Aleatórios:** A arena altera o estado do jogo a cada 2-5 turnos (ex: Tempestades que quebram escudos ou Gases Tóxicos que impedem a cura).
*   **Evolução Cognitiva:** Dados acumulados em salas iniciais são utilizados pelo Boss final para construir um perfil tático contra o jogador.

---

## 🛠️ Arquitetura e Engenharia de Software

Este projeto foi construído seguindo rigorosamente os princípios **SOLID** e **Clean Code**, utilizando padrões de projeto para garantir desacoplamento:

*   **Strategy Pattern:** Define comportamentos dinâmicos de IA (`RuthlessAiStrategy`, `LearningBossAiStrategy`).
*   **Command Pattern:** Desacopla a execução das ações (Atacar, Curar, Defender) do motor principal através da `CommandFactory`.
*   **Observer/Tracker:** Gestão centralizada de estatísticas de treino para persistência durante a sessão.

### Fluxo de Decisão da IA


---

## 💻 Stack Técnica

*   **Backend:** ASP.NET Core API (.NET 10) utilizando C#.
*   **Frontend:** Vanilla JavaScript com Sprite Engine customizado em Canvas HTML5.
*   **UI/UX:** Design moderno baseado em *Glassmorphism*, totalmente responsivo.
*   **Data:** Exportação de logs de treino em formato JSON para auditoria de comportamento.

---

## 📁 Estrutura do Projeto

```plaintext
├── Arena.Api
│   ├── Application
│   │   ├── Services (GameSession.cs - Motor Principal)
│   │   └── Strategies (IA e Lógicas de Combate)
│   ├── Domain
│   │   ├── Entities (Hero, Monster, AiDecision)
│   │   └── Interfaces (IAiStrategy, IAttackStrategy)
│   └── Program.cs (Configuração da API e Injeção de Dependência)
└── Frontend
    ├── index.html (UI e Motor Gráfico)
    └── assets (Sprites, SFX e VFX)
```

## 🚀 Como Executar

### 1. Backend
```bash
cd ArenaProject/Arena.Api
dotnet run
```

### 2. Frontend
Abra o arquivo `index.html` (Recomendado usar **Live Server**).

---

## 👨‍💻 Desenvolvedor

**Henrique Castro**  
*Engenharia de Software (7º Semestre) | Desenvolvedor Back-end*

<div align="center">
  <a href="https://github.com/HenriqueCastro18">
    <img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white" alt="GitHub">
  </a>
  <a href="https://www.linkedin.com/in/henrique-castro-b13798345">
    <img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn">
  </a>
</div>
