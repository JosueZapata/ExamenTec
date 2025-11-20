using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (category == null)
            throw new NotFoundException($"No se encontró la categoría con el id '{request.Id}'");

        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(request.Name);
        if (existingCategory != null && existingCategory.Id != request.Id)
        {
            throw new ConflictException($"Ya existe una categoría con el nombre '{request.Name}'");
        }

        category.Name = request.Name.Trim();
        category.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Adapt<CategoryResponseDto>();
    }
}

