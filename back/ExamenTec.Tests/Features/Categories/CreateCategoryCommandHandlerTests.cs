using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Categories.CreateCategory;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);
        _handler = new CreateCategoryCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaCrearCategoria_CuandoNombreEsUnico()
    {
        var command = new CreateCategoryCommand("Nueva Categoría", "Descripción");
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Nueva Categoría"))
            .ReturnsAsync((Category?)null);

        Category? createdCategory = null;
        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) =>
            {
                createdCategory = c;
                return c;
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Nueva Categoría");
        result.Description.Should().Be("Descripción");
        _categoryRepositoryMock.Verify(r => r.GetByNameAsync("Nueva Categoría"), Times.Once);
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarConflictException_CuandoNombreCategoriaExiste()
    {
        var existingCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Categoría Existente",
            CreatedDate = DateTime.UtcNow
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Categoría Existente"))
            .ReturnsAsync(existingCategory);

        var command = new CreateCategoryCommand("Categoría Existente", "Descripción");

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
        _categoryRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaRecortarNombreYDescripcion()
    {
        var command = new CreateCategoryCommand("  Categoría Recortada  ", "  Descripción Recortada  ");
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Categoría Recortada"))
            .ReturnsAsync((Category?)null);

        Category? createdCategory = null;
        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) =>
            {
                createdCategory = c;
                return c;
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Categoría Recortada");
        result.Description.Should().Be("Descripción Recortada");
        createdCategory!.Name.Should().Be("Categoría Recortada");
        createdCategory.Description.Should().Be("Descripción Recortada");
    }

    [Fact]
    public async Task Handle_DeberiaEstablecerDescripcionNull_CuandoDescripcionVacia()
    {
        var command = new CreateCategoryCommand("Categoría", "   ");
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Categoría"))
            .ReturnsAsync((Category?)null);

        Category? createdCategory = null;
        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) =>
            {
                createdCategory = c;
                return c;
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Description.Should().BeNull();
        createdCategory!.Description.Should().BeNull();
    }
}

