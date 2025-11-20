using ExamenTec.Application.Common;
using ExamenTec.Application.DTOs;
using ExamenTec.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamenTec.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class LogsController : ControllerBase
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    [HttpGet]
    public async Task<ActionResult<HttpResponse<IEnumerable<LogDto>>>> GetAll()
    {
        var logs = await _logService.GetAllAsync();
        return HttpResponse<IEnumerable<LogDto>>.Ok(logs);
    }
}
