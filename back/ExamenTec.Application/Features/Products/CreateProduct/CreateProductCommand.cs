using ExamenTec.Application.DTOs.Product;
using MediatR;

namespace ExamenTec.Application.Features.Products.CreateProduct;

public record CreateProductCommand(string Name, string? Description, decimal Price, int Stock, Guid CategoryId) : IRequest<ProductResponseDto>;

