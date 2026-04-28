# ⚔️ Arena RPG: Arena Viva

<div align="center">
  <img src="https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" alt="JavaScript" />
  <img src="https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white" alt="HTML5" />
  <img src="https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=white" alt="CSS3" />
</div>

<br>

> A **Arena RPG** é um motor de combate por turnos focado em estratégia avançada e inteligência artificial adaptativa. O projeto foi concebido para desafiar o jogador através de oponentes que aprendem com os seus hábitos, eventos climáticos que mudam as regras do jogo e um sistema de habilidades únicas (Ultimates) que exigem gestão rigorosa de recursos.

---

## 🌟 Principais Funcionalidades

### 1. Inteligência Artificial Preditiva (Hardcore AI)
Diferente de RPGs convencionais, os monstros não usam ataques aleatórios. O motor utiliza uma classe `PlayerPatternTracker` para monitorizar o comportamento do jogador:
* **Leitura de Cura:** Se o jogador tende a curar-se quando tem pouca vida, o Boss entrará em postura defensiva para interceptar e roubar a poção.
* **Mind Games:** O Boss pode guardar a sua Ultimate se perceber que o jogador usa o escudo de forma previsível, forçando o gasto de recursos à toa.

### 2. Sistema de Masmorra Dinâmica
* **Progressão em Ondas:** Enfrenta 6 salas consecutivas, desde o Verme Gigante até ao Minotauro Tático.
* **Evolução Cognitiva:** Os dados de treino acumulados nas primeiras salas são utilizados pelo Boss final para construir um perfil psicológico do jogador.

### 3. Arena Viva (Eventos Aleatórios)
A arena altera o estado do jogo a cada 2-5 turnos com eventos como:
* ⚡ **Tempestade Magnética:** Estilhaça todos os escudos.
* ☠️ **Gás Tóxico:** Impede qualquer tipo de cura por poções.
* ✨ **Bênção de Mana:** Carregamento acelerado de Ultimates.

### 4. Ultimates Criativas & Status
Cada personagem possui mecânicas exclusivas que vão além do dano:
* **Atordoamento (Stun):** Faz o inimigo perder o turno.
* **Redução de HP Máximo:** Diminui permanentemente a resistência do alvo.
* **Silenciamento:** Impede o uso de itens consumíveis.

---

## 🛠️ Tecnologias e Arquitetura

O projeto segue rigorosamente os princípios de **Clean Code** e **SOLID**, garantindo um Back-end robusto e escalável utilizando diversos Design Patterns:

* **Strategy Pattern:** Utilizado para definir diferentes comportamentos de IA (`RuthlessAiStrategy`, `LearningBossAiStrategy`) e tipos de ataque.
* **Factory Pattern:** Implementado na `CommandFactory` para instanciar comandos de combate dinamicamente.
* **Command Pattern:** Desacopla a lógica de execução das ações (Atacar, Curar, Defender) do motor principal.
* **Singleton/Static Tracker:** Gestão centralizada de estatísticas de treino para persistência durante a sessão.

### 💻 Stack Técnica
* **Backend:** ASP.NET Core API (.NET 10) utilizando C#.
* **Frontend:** Vanilla JavaScript com Sprite Engine customizado para renderização de frames em Canvas HTML5.
* **UI/UX:** Design moderno baseado em Glassmorphism, responsivo para Mobile e Desktop (CSS3).
* **Data:** Exportação de logs de treino em formato JSON para auditoria de comportamento da IA.

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
│   ├── LogsDeTreino (Ficheiros JSON gerados pela IA)
│   └── Program.cs (Configuração da API e CORS)
└── Frontend
    ├── index.html (UI e Motor Gráfico)
    └── assets (Sprites e VFX)

## 🚀 Como Executar

### Backend
1. Navegue até a pasta da API:
   ```bash
   cd ArenaProject/Arena.Api
Execute o servidor .NET:

Bash
```bash
dotnet run
Bash

Frontend
Abra o ficheiro index.html no seu navegador.

Recomendação: Utilize um servidor local como o Live Server do VS Code (certifique-se de adicionar a pasta LogsDeTreino ao ficheiro de ignore do Live Server para evitar reloads desnecessários quando a IA gerar um novo log).

👨‍💻 Desenvolvedor
Henrique Castro Lead Software Engineer & Back-end Developer
