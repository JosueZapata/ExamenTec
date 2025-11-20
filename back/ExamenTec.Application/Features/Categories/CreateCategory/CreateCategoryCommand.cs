using ExamenTec.Application.DTOs.Category;
using MediatR;

namespace ExamenTec.Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryResponseDto>;

