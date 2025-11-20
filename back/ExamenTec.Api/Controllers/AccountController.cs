using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs.Auth;
using ExamenTec.Application.Features.Account.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExamenTec.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<HttpResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto dto)
    {
        return HttpResponse<LoginResponseDto>.Ok(await _mediator.Send(new LoginQuery(dto.Email, dto.Password)));
    }
}

