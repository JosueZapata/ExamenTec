using ExamenTec.Domain.Entities;

namespace ExamenTec.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

