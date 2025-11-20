using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Products.DeleteProduct;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
        _handler = new DeleteProductCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaEliminarProducto_CuandoProductoExiste()
    {
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(r => r.ExistsAsync(productId)).ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteProductCommand(productId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _productRepositoryMock.Verify(r => r.ExistsAsync(productId), Times.Once);
        _productRepositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoProductoNoExiste()
    {
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(r => r.ExistsAsync(productId)).ReturnsAsync(false);

        var command = new DeleteProductCommand(productId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _productRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

