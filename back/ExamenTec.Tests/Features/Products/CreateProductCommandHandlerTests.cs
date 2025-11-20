using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Application.Features.Products.CreateProduct;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExamenTec.Tests.Features.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);

        _handler = new CreateProductCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaCrearProducto_CuandoDatosSonValidos()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Categoría", CreatedDate = DateTime.UtcNow };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.GetByNameAsync("Producto")).ReturnsAsync((Product?)null);

        Product? createdProduct = null;
        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                createdProduct = p;
                p.Id = Guid.NewGuid();
                return p;
            });

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => createdProduct);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateProductCommand("Producto", "Descripción", 100, 10, categoryId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("Producto");
        createdProduct.Price.Should().Be(100);
        createdProduct.Stock.Should().Be(10);
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoCategoriaNoExiste()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var command = new CreateProductCommand("Producto", "Descripción", 100, 10, categoryId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaLimpiarNombreYDescripcion()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Categoría", CreatedDate = DateTime.UtcNow };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.GetByNameAsync("Producto")).ReturnsAsync((Product?)null);

        Product? createdProduct = null;
        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                createdProduct = p;
                p.Id = Guid.NewGuid();
                return p;
            });

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => createdProduct);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateProductCommand("  Producto  ", "  Descripción  ", 100, 10, categoryId);

        await _handler.Handle(command, CancellationToken.None);

        createdProduct!.Name.Should().Be("Producto");
        createdProduct.Description.Should().Be("Descripción");
    }

    [Fact]
    public async Task Handle_DeberiaLanzarConflictException_CuandoNombreProductoExiste()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Categoría", CreatedDate = DateTime.UtcNow };
        var existingProduct = new Product { Id = Guid.NewGuid(), Name = "Producto", CategoryId = categoryId };

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _productRepositoryMock.Setup(r => r.GetByNameAsync("Producto")).ReturnsAsync(existingProduct);

        var command = new CreateProductCommand("Producto", "Descripción", 100, 10, categoryId);

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

