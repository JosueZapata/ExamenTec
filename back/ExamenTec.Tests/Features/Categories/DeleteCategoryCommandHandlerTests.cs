using ExamenTec.Application.Common.Exceptions;
using ExamenTec.Application.Features.Categories.DeleteCategory;
using ExamenTec.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamenTec.Tests.Features.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepositoryMock.Object);
        _handler = new DeleteCategoryCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeberiaEliminarCategoria_CuandoCategoriaExiste()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteCategoryCommand(categoryId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _categoryRepositoryMock.Verify(r => r.ExistsAsync(categoryId), Times.Once);
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(categoryId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeberiaLanzarNotFoundException_CuandoCategoriaNoExiste()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepositoryMock
            .Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        var command = new DeleteCategoryCommand(categoryId);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
