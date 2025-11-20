using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Domain.Interfaces;
using MediatR;

namespace ExamenTec.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Categories.ExistsAsync(request.Id);
        if (!exists)
            throw new NotFoundException($"No se encontró la categoría con el id '{request.Id}'");

        await _unitOfWork.Categories.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

