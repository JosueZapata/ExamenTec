using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Categories.GetCategoryById;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Categories;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);
        _handler = new GetCategoryByIdQueryHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaRetornarCategoria_CuandoCategoriaExiste()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Categoría de Prueba",
            Description = "Descripción de Prueba",
            CreatedDate = DateTime.UtcNow
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        var query = new GetCategoryByIdQuery(categoryId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(categoryId);
        result.Name.Should().Be("Categoría de Prueba");
        result.Description.Should().Be("Descripción de Prueba");
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoCategoriaNoExiste()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        var query = new GetCategoryByIdQuery(categoryId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
    }
}

