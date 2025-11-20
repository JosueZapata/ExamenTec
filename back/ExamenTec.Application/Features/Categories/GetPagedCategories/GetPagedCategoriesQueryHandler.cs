using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Categories.GetPagedCategories;

public class GetPagedCategoriesQueryHandler : IRequestHandler<GetPagedCategoriesQuery, PagedResult<CategoryResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CategoryResponseDto>> Handle(GetPagedCategoriesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize > 100 ? 100 : request.PageSize;

        var (categories, totalCount) = await _unitOfWork.Categories.GetPagedAsync(page, pageSize, request.SearchTerm);

        var categoriesDto = categories.Adapt<IEnumerable<CategoryResponseDto>>();

        return new PagedResult<CategoryResponseDto>(categoriesDto, page, pageSize, totalCount);
    }
}

