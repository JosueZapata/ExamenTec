using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Products.GetProductById;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
        _handler = new GetProductByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaRetornarProducto_CuandoProductoExiste()
    {
        var productId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), Name = "Categoría 1" };
        var product = new Product
        {
            Id = productId,
            Name = "Producto de Prueba",
            Description = "Descripción de Prueba",
            Price = 99.99m,
            Stock = 10,
            Category = category,
            CreatedDate = DateTime.UtcNow
        };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var query = new GetProductByIdQuery(productId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Producto de Prueba");
        result.Price.Should().Be(99.99m);
        result.CategoryName.Should().Be("Categoría 1");
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoProductoNoExiste()
    {
        var productId = Guid.NewGuid();
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery(productId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}

