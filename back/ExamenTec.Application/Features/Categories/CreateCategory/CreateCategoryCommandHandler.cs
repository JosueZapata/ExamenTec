using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(request.Name);
        if (existingCategory != null)
        {
            throw new ConflictException($"Ya existe una categoría con el nombre '{request.Name}'");
        }

        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CreatedDate = DateTime.UtcNow
        };

        var created = await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return created.Adapt<CategoryResponseDto>();
    }
}

