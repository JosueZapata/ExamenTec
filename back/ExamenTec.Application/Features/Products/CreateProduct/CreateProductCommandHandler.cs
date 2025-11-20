using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Products.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException($"No se encontró la categoría con el id '{request.CategoryId}'");

        var normalizedName = request.Name.Trim();
        var existingProduct = await _unitOfWork.Products.GetByNameAsync(normalizedName);
        if (existingProduct != null)
        {
            throw new ConflictException($"Ya existe un producto con el nombre '{normalizedName}'");
        }

        var product = new Product
        {
            Name = normalizedName,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var createdWithRelations = await _unitOfWork.Products.GetByIdAsync(created.Id);
        return createdWithRelations!.Adapt<ProductResponseDto>();
    }
}

