using ExamenTec.Application.DTOs.Category;
using MediatR;

namespace ExamenTec.Application.Features.Categories.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<CategoryResponseDto>;

