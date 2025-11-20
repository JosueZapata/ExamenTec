using ExamenTec.Application.DTOs.Category;
using MediatR;

namespace ExamenTec.Application.Features.Categories.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryResponseDto>;

