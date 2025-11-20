using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamenTec.Infrastructure.Repositories;

public class LogRepository : GenericRepository<Log>, ILogRepository
{
    public LogRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Log>> GetRecentAsync(int count = 100)
    {
        return await _dbSet
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
