using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Products.UpdateProduct;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);

        _handler = new UpdateProductCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaActualizarProducto_CuandoProductoExiste()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Nombre Antiguo",
            Price = 50,
            Stock = 5,
            CategoryId = categoryId,
            CreatedDate = DateTime.UtcNow
        };
        var category = new Category { Id = categoryId, Name = "Categoría", CreatedDate = DateTime.UtcNow };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

        _productRepositoryMock
            .SetupSequence(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product)
            .ReturnsAsync(() =>
            {
                return new Product
                {
                    Id = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    CreatedDate = product.CreatedDate
                };
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateProductCommand(productId, "Nombre Nuevo", "Descripción Nueva", 100, 10, categoryId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        product.Name.Should().Be("Nombre Nuevo");
        product.Price.Should().Be(100);
        product.Stock.Should().Be(10);
        _productRepositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoProductoNoExiste()
    {
        var productId = Guid.NewGuid();
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

        var command = new UpdateProductCommand(productId, "Nombre", "Descripción", 100, 10, Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoCategoriaNoExiste()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Producto", CreatedDate = DateTime.UtcNow };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var command = new UpdateProductCommand(productId, "Nombre", "Descripción", 100, 10, categoryId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}

