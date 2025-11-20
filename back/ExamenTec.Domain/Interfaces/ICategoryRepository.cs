using ExamenTec.Domain.Entities;

namespace ExamenTec.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
    Task<IEnumerable<Category>> SearchByTermAsync(string searchTerm, int maxResults = 20);
}

