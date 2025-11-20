using ExamenTec.Api.Configurations;
using ExamenTec.Api.Middleware;
using ExamenTec.Application.Common.Behaviors;
using ExamenTec.Application.Mappings;
using ExamenTec.Domain.Interfaces;
using ExamenTec.Infrastructure;
using ExamenTec.Infrastructure.Data;
using ExamenTec.Infrastructure.Logging;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
    });

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization usando el esquema Bearer. Ingrese 'Bearer' [espacio] y luego su token en el campo de texto a continuación.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddValidatorsFromAssembly(typeof(ExamenTec.Application.AssemblyReference).Assembly);
FluentValidationMvcExtensions.AddFluentValidationClientsideAdapters(builder.Services);

MapsterConfig.Configure();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ExamenTec.Application.AssemblyReference).Assembly));

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ExamenTecApi", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductAccess", policy => policy.RequireRole("Admin", "Product"));
    options.AddPolicy("CategoryAccess", policy => policy.RequireRole("Admin", "Category"));
});

var serviceProvider = builder.Services.BuildServiceProvider();
var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
var dbLoggerProvider = new DbLoggerProvider(serviceScopeFactory);
builder.Logging.AddProvider(dbLoggerProvider);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbLoggerProvider.SetSeedingInProgress(true);

    try
    {
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var seeder = new DataSeeder(unitOfWork);

        await seeder.SeedCategoriesAsync();
        await seeder.SeedProductsAsync();
        await seeder.SeedUsersAsync();
    }
    finally
    {
        DbLoggerProvider.SetSeedingInProgress(false);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseRouting();

app.UseCors("ExamenTecApi");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
