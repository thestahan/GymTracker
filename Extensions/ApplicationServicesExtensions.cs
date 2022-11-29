using FluentValidation;
using GymTracker.Behaviors;
using GymTracker.Behaviors.Filters;
using GymTracker.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace GymTracker.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config,
        bool isDevelopment)
    {
        var currentAssembly = Assembly.GetExecutingAssembly();

        services.AddControllers(opt =>
        {
            opt.Filters.Add<FluentValidationExceptionFilter>();
            opt.Filters.Add<BadRequestExceptionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(cfg =>
        {
            cfg.SwaggerDoc("v1", new OpenApiInfo { Title = "Tournament4You API", Version = "v1" });
            cfg.CustomSchemaIds(type => type.ToString());
        });

        services.AddAutoMapper(currentAssembly);

        services.AddMediatR(currentAssembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(currentAssembly);

        services.AddDbContext<ApiDbContext>(opt =>
        {
            if (isDevelopment)
            {
                opt.UseNpgsql(config.GetConnectionString("DbConnection"));
            }
        });

        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://localhost:3000");
            });
        });

        return services;
    }
}
