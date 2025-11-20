using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Category;
using ExamenTec.Application.Features.Categories.CreateCategory;
using ExamenTec.Application.Features.Categories.DeleteCategory;
using ExamenTec.Application.Features.Categories.GetCategoryById;
using ExamenTec.Application.Features.Categories.GetPagedCategories;
using ExamenTec.Application.Features.Categories.SearchCategories;
using ExamenTec.Application.Features.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExamenTec.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin,Product,Category")]
    public async Task<ActionResult<HttpResponse<IEnumerable<CategoryLookupDto>>>> Search([FromQuery] string searchTerm, [FromQuery] int maxResults = 20)
    {
        return HttpResponse<IEnumerable<CategoryLookupDto>>.Ok(await _mediator.Send(new SearchCategoriesQuery(searchTerm, maxResults)));
    }

    [HttpGet]
    [Authorize(Policy = "CategoryAccess")]
    public async Task<ActionResult<HttpResponse<PagedResult<CategoryResponseDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        return HttpResponse<PagedResult<CategoryResponseDto>>.Ok(await _mediator.Send(new GetPagedCategoriesQuery(page, pageSize, searchTerm)));
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "CategoryAccess")]
    public async Task<ActionResult<HttpResponse<CategoryResponseDto>>> GetById(Guid id)
    {
        return HttpResponse<CategoryResponseDto>.Ok(await _mediator.Send(new GetCategoryByIdQuery(id)));
    }

    [HttpPost]
    [Authorize(Policy = "CategoryAccess")]
    public async Task<ActionResult<HttpResponse<CategoryResponseDto>>> Create([FromBody] CategoryRequestDto dto)
    {
        return HttpResponse<CategoryResponseDto>.Created(await _mediator.Send(new CreateCategoryCommand(dto.Name, dto.Description)));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CategoryAccess")]
    public async Task<ActionResult<HttpResponse<CategoryResponseDto>>> Update(Guid id, [FromBody] CategoryRequestDto dto)
    {
        return HttpResponse<CategoryResponseDto>.Ok(await _mediator.Send(new UpdateCategoryCommand(id, dto.Name, dto.Description)));
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CategoryAccess")]
    public async Task<ActionResult<HttpResponse<object>>> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return StatusCode((int)HttpStatusCode.NoContent, HttpResponse<object>.Ok());
    }
}
