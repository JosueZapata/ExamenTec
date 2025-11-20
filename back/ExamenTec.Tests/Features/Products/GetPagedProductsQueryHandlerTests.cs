using ExamenTec.Application.Features.Products.GetPagedProducts;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Products;

public class GetPagedProductsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetPagedProductsQueryHandler _handler;

    public GetPagedProductsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);
        _handler = new GetPagedProductsQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaRetornarResultadoPaginado_CuandoProductosExisten()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Categoría 1" };
        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Producto 1",
                Price = 100,
                Stock = 10,
                Category = category,
                CreatedDate = DateTime.UtcNow
            }
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((products, 1));

        var query = new GetPagedProductsQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DeberiaAplicarFiltroDeBusqueda_CuandoSeProporcionaTermino()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Electrónica" };
        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop",
                Price = 1000,
                Stock = 5,
                Category = category,
                CreatedDate = DateTime.UtcNow
            }
        };

        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, "Laptop"))
            .ReturnsAsync((products, 1));

        var query = new GetPagedProductsQuery(1, 10, "Laptop");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        _productRepositoryMock.Verify(r => r.GetPagedAsync(1, 10, "Laptop"), Times.Once);
    }
}

