using ExamenTec.Api.Controllers;
using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Application.Features.Categories.CreateCategory;
using ExamenTec.Application.Features.Categories.DeleteCategory;
using ExamenTec.Application.Features.Categories.GetCategoryById;
using ExamenTec.Application.Features.Categories.GetPagedCategories;
using ExamenTec.Application.Features.Categories.UpdateCategory;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace ExamenTec.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CategoriesController(_mediatorMock.Object);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task GetAll_DeberiaRetornarOkResult_ConCategoriasPaginadas()
    {
        var categories = new List<CategoryResponseDto>
        {
            new CategoryResponseDto { Id = Guid.NewGuid(), Name = "Categoría 1" },
            new CategoryResponseDto { Id = Guid.NewGuid(), Name = "Categoría 2" }
        };

        var pagedResult = new PagedResult<CategoryResponseDto>(categories, 1, 10, 2);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPagedCategoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetAll(1, 10, null);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Items.Should().HaveCount(2);
        result.Value.Result.TotalCount.Should().Be(2);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetPagedCategoriesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_DeberiaRetornarOkResult_CuandoCategoriaExiste()
    {
        var categoryId = Guid.NewGuid();
        var category = new CategoryResponseDto
        {
            Id = categoryId,
            Name = "Categoría de Prueba",
            Description = "Descripción de Prueba"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _controller.GetById(categoryId);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Id.Should().Be(categoryId);
        _mediatorMock.Verify(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_DeberiaRetornarCreatedResult_CuandoCategoriaCreada()
    {
        var dto = new CategoryRequestDto { Name = "Nueva Categoría", Description = "Descripción" };
        var createdCategory = new CategoryResponseDto
        {
            Id = Guid.NewGuid(),
            Name = "Nueva Categoría",
            Description = "Descripción"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCategory);

        var result = await _controller.Create(dto);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Result.Should().NotBeNull();
        _mediatorMock.Verify(m => m.Send(It.Is<CreateCategoryCommand>(c => c.Name == dto.Name && c.Description == dto.Description), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_DeberiaRetornarOkResult_CuandoCategoriaActualizada()
    {
        var categoryId = Guid.NewGuid();
        var dto = new CategoryRequestDto { Name = "Categoría Actualizada", Description = "Descripción Actualizada" };
        var updatedCategory = new CategoryResponseDto
        {
            Id = categoryId,
            Name = "Categoría Actualizada",
            Description = "Descripción Actualizada"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedCategory);

        var result = await _controller.Update(categoryId, dto);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Name.Should().Be("Categoría Actualizada");
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateCategoryCommand>(c => c.Id == categoryId && c.Name == dto.Name), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_DeberiaRetornarNoContentResult_CuandoCategoriaEliminada()
    {
        var categoryId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediatR.Unit.Value);

        var result = await _controller.Delete(categoryId);

        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        _mediatorMock.Verify(m => m.Send(It.Is<DeleteCategoryCommand>(c => c.Id == categoryId), It.IsAny<CancellationToken>()), Times.Once);
    }
}

