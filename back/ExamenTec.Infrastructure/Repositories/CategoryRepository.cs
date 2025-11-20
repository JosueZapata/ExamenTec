using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using ExamenTec.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ExamenTec.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => EF.Functions.Like(c.Name, name));
    }

    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim();
            var normalizedSearch = StringHelper.NormalizeForSearch(search);

            var allItems = await query.ToListAsync();
            var filteredItems = allItems.Where(c =>
                StringHelper.NormalizeForSearch(c.Name).Contains(normalizedSearch) ||
                (c.Description != null && StringHelper.NormalizeForSearch(c.Description).Contains(normalizedSearch))
            ).ToList();

            var totalCount = filteredItems.Count;

            var items = filteredItems
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (items, totalCount);
        }

        var totalCountNoFilter = await query.CountAsync();
        var itemsNoFilter = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (itemsNoFilter, totalCountNoFilter);
    }

    public async Task<IEnumerable<Category>> SearchByTermAsync(string searchTerm, int maxResults = 20)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<Category>();
        }

        var normalizedSearch = StringHelper.NormalizeForSearch(searchTerm.Trim());

      return await _dbSet
            .Where(c => StringHelper.NormalizeForSearch(c.Name).Contains(normalizedSearch))
            .OrderBy(c => c.Name)
            .Take(maxResults)
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }
}

