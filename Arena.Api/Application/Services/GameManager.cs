using System;
using System.Collections.Generic;
using Arena.Api.Domain.Entities;

namespace Arena.Api.Application.Services
{
    public class GameManager
    {
        private readonly Dictionary<Guid, GameSession> _sessions = new();

        public Guid StartNewGame(Hero hero, Monster monster)
        {
            var sessionId = Guid.NewGuid(); // Gera um ID único e impossível de adivinhar
            var session = new GameSession(hero, monster);
            
            _sessions[sessionId] = session; // Salva a sessão na memória
            
            return sessionId;
        }

        public GameSession? GetSession(Guid sessionId)
        {
            // Tenta buscar a sessão. Se não achar, retorna nulo.
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                return session;
            }
            return null;
        }
    }
}