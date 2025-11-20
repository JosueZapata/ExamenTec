using ExamenTec.Application.DTOs.Category;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Categories.SearchCategories;

public class SearchCategoriesQueryHandler : IRequestHandler<SearchCategoriesQuery, IEnumerable<CategoryLookupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryLookupDto>> Handle(SearchCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.SearchByTermAsync(request.SearchTerm, request.MaxResults);
        return categories.Adapt<IEnumerable<CategoryLookupDto>>();
    }
}


