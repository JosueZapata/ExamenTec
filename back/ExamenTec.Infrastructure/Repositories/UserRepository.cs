using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamenTec.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}

