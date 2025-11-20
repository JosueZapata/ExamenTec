using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Category;
using MediatR;

namespace ExamenTec.Application.Features.Categories.GetPagedCategories;

public record GetPagedCategoriesQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null) : IRequest<PagedResult<CategoryResponseDto>>;

