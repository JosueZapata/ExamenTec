using ExamenTec.Application.Interfaces;
using ExamenTec.Application.Services;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure.Data;
using ExamenTec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExamenTec.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration? configuration = null)
    {
        var useInMemory = configuration?.GetValue<bool>("Repository:UseInMemory") ?? true;
        var connectionString = configuration?.GetConnectionString("DefaultConnection");

        if (useInMemory || string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ExamenTecDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}

