using ExamenTec.Domain.Entities;

namespace ExamenTec.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByNameAsync(string name);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
}

