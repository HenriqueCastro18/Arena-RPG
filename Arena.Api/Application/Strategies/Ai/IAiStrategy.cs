using Arena.Api.Application.Services;
using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Interfaces
{
    public interface IAiStrategy
    {
        AiDecision DecideNextMove(GameSession session);
    }
}