using MediatR;

namespace ExamenTec.Application.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Unit>;

