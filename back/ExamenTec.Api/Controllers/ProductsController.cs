using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Product;
using ExamenTec.Application.Features.Products.CreateProduct;
using ExamenTec.Application.Features.Products.DeleteProduct;
using ExamenTec.Application.Features.Products.GetProductById;
using ExamenTec.Application.Features.Products.GetPagedProducts;
using ExamenTec.Application.Features.Products.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExamenTec.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "ProductAccess")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<PagedResult<ProductResponseDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        return HttpResponse<PagedResult<ProductResponseDto>>.Ok(await _mediator.Send(new GetPagedProductsQuery(page, pageSize, searchTerm)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HttpResponse<ProductResponseDto>>> GetById(Guid id)
    {
        return HttpResponse<ProductResponseDto>.Ok(await _mediator.Send(new GetProductByIdQuery(id)));
    }

    [HttpPost]
    public async Task<ActionResult<HttpResponse<ProductResponseDto>>> Create([FromBody] ProductRequestDto dto)
    {
        return HttpResponse<ProductResponseDto>.Created(await _mediator.Send(new CreateProductCommand(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId)));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HttpResponse<ProductResponseDto>>> Update(Guid id, [FromBody] ProductRequestDto dto)
    {
        return HttpResponse<ProductResponseDto>.Ok(await _mediator.Send(new UpdateProductCommand(id, dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId)));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<HttpResponse<object>>> Delete(Guid id)
    {
        await _mediator.Send(new DeleteProductCommand(id));
        return StatusCode((int)HttpStatusCode.NoContent, HttpResponse<object>.Ok());
    }
}
