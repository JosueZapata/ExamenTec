using ExamenTec.Application.DTOs.Product;
using MediatR;

namespace ExamenTec.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, string? Description, decimal Price, int Stock, Guid CategoryId) : IRequest<ProductResponseDto>;

