using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Categories.UpdateCategory;
using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Categories;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);
        _handler = new UpdateCategoryCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaActualizarCategoria_CuandoCategoriaExiste()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Nombre Antiguo",
            Description = "Descripción Antigua",
            CreatedDate = DateTime.UtcNow
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Nombre Nuevo"))
            .ReturnsAsync((Category?)null);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateCategoryCommand(categoryId, "Nombre Nuevo", "Descripción Nueva");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Nombre Nuevo");
        result.Description.Should().Be("Descripción Nueva");
        category.Name.Should().Be("Nombre Nuevo");
        category.Description.Should().Be("Descripción Nueva");
        _categoryRepositoryMock.Verify(r => r.UpdateAsync(category), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoCategoriaNoExiste()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        var command = new UpdateCategoryCommand(categoryId, "Nombre Nuevo", "Descripción");

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _categoryRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarConflictException_CuandoNombreExisteOtraCategoria()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Nombre Antiguo",
            CreatedDate = DateTime.UtcNow
        };

        var existingCategory = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Nombre Existente",
            CreatedDate = DateTime.UtcNow
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Nombre Existente"))
            .ReturnsAsync(existingCategory);

        var command = new UpdateCategoryCommand(categoryId, "Nombre Existente", "Descripción");

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DeberiaPermitirMismoNombre_CuandoActualizaMismaCategoria()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Mismo Nombre",
            Description = "Descripción Antigua",
            CreatedDate = DateTime.UtcNow
        };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Mismo Nombre"))
            .ReturnsAsync(category);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateCategoryCommand(categoryId, "Mismo Nombre", "Descripción Nueva");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Mismo Nombre");
        result.Description.Should().Be("Descripción Nueva");
    }
}

