using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Mappings;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validators;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        services.AddScoped<IUserRepository,     UserRepository>();
        services.AddScoped<IProjectRepository,  ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<ICommentRepository,  CommentRepository>();

        services.AddScoped<IUserService,     UserService>();
        services.AddScoped<IProjectService,  ProjectService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<ICommentService,  CommentService>();

        return services;
    }
}
