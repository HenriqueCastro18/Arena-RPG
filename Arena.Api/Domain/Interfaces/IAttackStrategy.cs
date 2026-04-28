using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Interfaces
{
    public interface IAttackStrategy
    {
        // Retorna o dano final causado para enviarmos ao log do Frontend
        int Execute(Character attacker, Character target);
    }
}