using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using ExamenTec.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ExamenTec.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var normalized = name.Trim().ToLower();

        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Name.ToLower() == normalized);
    }

    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId)
    {
      throw new NotImplementedException();
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var normalizedSearch = StringHelper.NormalizeForSearch(search);

            var allItems = await query.ToListAsync();
            var filteredItems = allItems.Where(p =>
                StringHelper.NormalizeForSearch(p.Name).Contains(normalizedSearch) ||
                (p.Description != null && StringHelper.NormalizeForSearch(p.Description).Contains(normalizedSearch)) ||
                (p.Category != null && StringHelper.NormalizeForSearch(p.Category.Name).Contains(normalizedSearch))
            ).ToList();

            var totalCount = filteredItems.Count;

            var items = filteredItems
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, totalCount);
        }

        var totalCountNoFilter = await query.CountAsync();
        var itemsNoFilter = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (itemsNoFilter, totalCountNoFilter);
    }
}

