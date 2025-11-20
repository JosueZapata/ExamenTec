using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Product;
using MediatR;

namespace ExamenTec.Application.Features.Products.GetPagedProducts;

public record GetPagedProductsQuery(int Page = 1, int PageSize = 10, string? SearchTerm = null) : IRequest<PagedResult<ProductResponseDto>>;

