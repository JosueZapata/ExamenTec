using ExamenTec.Application.DTOs;

namespace ExamenTec.Application.Interfaces;

public interface ILogService
{
    Task<IEnumerable<LogDto>> GetAllAsync();
}
