using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponseDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (category == null)
            throw new NotFoundException($"No se encontró la categoría con el id '{request.Id}'");
        
        return category.Adapt<CategoryResponseDto>();
    }
}

