using System.Threading.Tasks;
using Arena.Api.Domain.Entities;

namespace Arena.Api.Domain.Interfaces
{
    public interface ITrainingLogService
    {
        Task SaveAsync(TrainingReport report);
    }
}
