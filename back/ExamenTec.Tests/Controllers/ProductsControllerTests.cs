using ExamenTec.Api.Controllers;
using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Application.Features.Products.CreateProduct;
using ExamenTec.Application.Features.Products.DeleteProduct;
using ExamenTec.Application.Features.Products.GetPagedProducts;
using ExamenTec.Application.Features.Products.GetProductById;
using ExamenTec.Application.Features.Products.UpdateProduct;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace ExamenTec.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProductsController(_mediatorMock.Object);

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
    public async Task GetAll_DeberiaRetornarOkResult_ConProductosPaginados()
    {
        var products = new List<ProductResponseDto>
        {
            new ProductResponseDto { Id = Guid.NewGuid(), Name = "Producto 1", Price = 100 }
        };

        var pagedResult = new PagedResult<ProductResponseDto>(products, 1, 10, 1);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPagedProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _controller.GetAll(1, 10, null);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Items.Should().HaveCount(1);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetPagedProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_DeberiaRetornarOkResult_CuandoProductoExiste()
    {
        var productId = Guid.NewGuid();
        var product = new ProductResponseDto { Id = productId, Name = "Producto de Prueba", Price = 99.99m };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.GetById(productId);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Id.Should().Be(productId);
        _mediatorMock.Verify(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_DeberiaRetornarCreatedResult_CuandoProductoCreado()
    {
        var dto = new ProductRequestDto
        {
            Name = "Producto Nuevo",
            Price = 100,
            Stock = 10,
            CategoryId = Guid.NewGuid()
        };
        var createdProduct = new ProductResponseDto { Id = Guid.NewGuid(), Name = "Producto Nuevo" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProduct);

        var result = await _controller.Create(dto);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Result.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_DeberiaRetornarOkResult_CuandoProductoActualizado()
    {
        var productId = Guid.NewGuid();
        var dto = new ProductRequestDto
        {
            Name = "Producto Actualizado",
            Price = 150,
            Stock = 20,
            CategoryId = Guid.NewGuid()
        };
        var updatedProduct = new ProductResponseDto { Id = productId, Name = "Producto Actualizado" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedProduct);

        var result = await _controller.Update(productId, dto);

        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value!.Result.Should().NotBeNull();
        result.Value.Result!.Name.Should().Be("Producto Actualizado");
        _mediatorMock.Verify(m => m.Send(It.Is<UpdateProductCommand>(c => c.Id == productId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_DeberiaRetornarNoContentResult_CuandoProductoEliminado()
    {
        var productId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediatR.Unit.Value);

        var result = await _controller.Delete(productId);

        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }
}

