using System.Threading.Tasks;
using Arena.Api.Domain.Entities;
using Arena.Api.Domain.Interfaces;

namespace Arena.Api.Infrastructure.Firebase
{
    public sealed class NullTrainingLogService : ITrainingLogService
    {
        public Task SaveAsync(TrainingReport report) => Task.CompletedTask;
    }
}
