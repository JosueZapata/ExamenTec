using ExamenTec.Domain.Entities;
using ExamenTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExamenTec.Infrastructure.Logging;

public class DbLogger : ILogger
{
    private readonly string _categoryName;
    private readonly DbLoggerProvider _provider;

    public DbLogger(string categoryName, DbLoggerProvider provider)
    {
        _categoryName = categoryName;
        _provider = provider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        if (_categoryName.Contains("DataSeeder", StringComparison.OrdinalIgnoreCase))
            return false;

        return logLevel >= LogLevel.Information;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (_categoryName.Contains("DataSeeder", StringComparison.OrdinalIgnoreCase))
            return;

        var message = formatter(state, exception);
        var exceptionString = exception?.ToString();

        if (logLevel >= LogLevel.Information)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _provider.WriteLogAsync(new Log
                    {
                        Timestamp = DateTime.UtcNow,
                        Level = logLevel.ToString(),
                        Message = message,
                        Exception = exceptionString,
                        Source = _categoryName
                    });
                }
                catch
                {
                }
            });
        }
    }
}

