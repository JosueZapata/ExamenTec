using ExamenTec.Domain.Entities;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExamenTec.Infrastructure.Logging;

public class DbLoggerProvider : ILoggerProvider
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static bool _seedingInProgress = false;

    public DbLoggerProvider(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public static void SetSeedingInProgress(bool isSeeding)
    {
        _seedingInProgress = isSeeding;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DbLogger(categoryName, this);
    }

    public async Task WriteLogAsync(Log log)
    {
        if (_seedingInProgress)
            return;

        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            await unitOfWork.Logs.AddAsync(log);
            await unitOfWork.SaveChangesAsync();
        }
        catch
        {
        }
    }

    public void Dispose()
    {
    }
}

