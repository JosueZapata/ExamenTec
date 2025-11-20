using ExamenTec.Application.DTOs.Category;
using MediatR;

namespace ExamenTec.Application.Features.Categories.SearchCategories;

public record SearchCategoriesQuery(string SearchTerm, int MaxResults = 20) : IRequest<IEnumerable<CategoryLookupDto>>;


