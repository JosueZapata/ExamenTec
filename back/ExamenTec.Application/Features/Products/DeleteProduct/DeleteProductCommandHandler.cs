using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Domain.Interfaces;
using MediatR;

namespace ExamenTec.Application.Features.Products.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Products.ExistsAsync(request.Id);
        if (!exists)
            throw new NotFoundException($"No se encontró el producto con el id '{request.Id}'");

        await _unitOfWork.Products.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

