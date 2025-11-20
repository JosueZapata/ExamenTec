using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Domain.Interfaces;
using Mapster;
using MediatR;

namespace ExamenTec.Application.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
        if (product == null)
            throw new NotFoundException($"No se encontró el producto con el id '{request.Id}'");

        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new NotFoundException($"No se encontró la categoría con el id '{request.CategoryId}'");

        product.Name = request.Name.Trim();
        product.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updatedWithRelations = await _unitOfWork.Products.GetByIdAsync(product.Id);
        return updatedWithRelations!.Adapt<ProductResponseDto>();
    }
}

