using ExamenTec.Application.DTOs;
using ExamenTec.Application.Interfaces;
using ExamenTec.Domain.Interfaces;

namespace ExamenTec.Application.Services;

public class LogService : ILogService
{
    private readonly IUnitOfWork _unitOfWork;

    public LogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<LogDto>> GetAllAsync()
    {
        var logs = await _unitOfWork.Logs.GetRecentAsync(1000);
        return logs.Select(MapToDto);
    }

    private static LogDto MapToDto(Domain.Entities.Log log)
    {
        return new LogDto
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            Message = log.Message,
            Exception = log.Exception,
            Source = log.Source,
            Method = log.Method,
            User = log.User,
            RequestPath = log.RequestPath,
            StatusCode = log.StatusCode
        };
    }
}
