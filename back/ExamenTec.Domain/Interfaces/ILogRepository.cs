using ExamenTec.Domain.Entities;

namespace ExamenTec.Domain.Interfaces;

public interface ILogRepository : IRepository<Log>
{
    Task<IEnumerable<Log>> GetRecentAsync(int count = 100);
}
