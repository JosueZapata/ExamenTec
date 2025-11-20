using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Products.GetPagedProducts;

public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, PagedResult<ProductResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductResponseDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize > 100 ? 100 : request.PageSize;

        var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(page, pageSize, request.SearchTerm);

        var productsDto = products.Adapt<IEnumerable<ProductResponseDto>>();

        return new PagedResult<ProductResponseDto>(productsDto, page, pageSize, totalCount);
    }
}

