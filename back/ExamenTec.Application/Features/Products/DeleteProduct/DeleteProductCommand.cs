using MediatR;

namespace ExamenTec.Application.Features.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<Unit>;

