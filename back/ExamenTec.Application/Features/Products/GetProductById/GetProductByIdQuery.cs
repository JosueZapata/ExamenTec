using ExamenTec.Application.DTOs.Product;
using MediatR;

namespace ExamenTec.Application.Features.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductResponseDto>;

