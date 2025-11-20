using ExamenTec.Application.Features.Categories.GetPagedCategories;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Categories;

public class GetPagedCategoriesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetPagedCategoriesQueryHandler _handler;

    public GetPagedCategoriesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);
        _handler = new GetPagedCategoriesQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaRetornarResultadoPaginado_CuandoExistenCategorias()
    {
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Categoría 1", Description = "Descripción 1", CreatedDate = DateTime.UtcNow },
            new Category { Id = Guid.NewGuid(), Name = "Categoría 2", Description = "Descripción 2", CreatedDate = DateTime.UtcNow }
        };

        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((categories, 2));

        var query = new GetPagedCategoriesQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DeberiaAplicarPaginacion_CuandoSeEnviaPaginaYTamano()
    {
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Categoría 1", CreatedDate = DateTime.UtcNow }
        };

        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(2, 5, null))
            .ReturnsAsync((categories, 10));

        var query = new GetPagedCategoriesQuery(2, 5);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_DeberiaAplicarFiltroDeBusqueda_CuandoSeProporcionaTermino()
    {
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Electrónica", Description = "Artículos tecnológicos", CreatedDate = DateTime.UtcNow }
        };

        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, "Electrónica"))
            .ReturnsAsync((categories, 1));

        var query = new GetPagedCategoriesQuery(1, 10, "Electrónica");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        _categoryRepositoryMock.Verify(r => r.GetPagedAsync(1, 10, "Electrónica"), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaNormalizarPagina_CuandoPaginaMenorQueUno()
    {
        var categories = new List<Category>();
        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((categories, 0));

        var query = new GetPagedCategoriesQuery(0, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Page.Should().Be(1);
        _categoryRepositoryMock.Verify(r => r.GetPagedAsync(1, 10, null), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLimitarTamanoPagina_CuandoSuperaMaximo()
    {
        var categories = new List<Category>();
        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 100, null))
            .ReturnsAsync((categories, 0));

        var query = new GetPagedCategoriesQuery(1, 150);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.Should().Be(100);
        _categoryRepositoryMock.Verify(r => r.GetPagedAsync(1, 100, null), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaRetornarResultadoVacio_CuandoNoExistenCategorias()
    {
        var categories = new List<Category>();
        _categoryRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10, null))
            .ReturnsAsync((categories, 0));

        var query = new GetPagedCategoriesQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}

