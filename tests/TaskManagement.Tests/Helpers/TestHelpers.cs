using AutoMapper;
using Microsoft.EntityFrameworkCore;
using IMapper = AutoMapper.IMapper;
using TaskManagement.Application.Mappings;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Tests.Helpers;


public static class TestDbContextFactory
{
    public static AppDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}


public static class TestMapperFactory
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }
}

public static class TestData
{
    public static User CreateUser(int id = 1, string email = "test@example.com") => new()
    {
        Id        = id,
        FirstName = "John",
        LastName  = "Doe",
        Email     = email
    };

    public static Project CreateProject(int id = 1) => new()
    {
        Id          = id,
        Name        = "Test Project",
        Description = "A test project"
    };

    public static TaskItem CreateTaskItem(int id = 1, int projectId = 1, int? userId = null) => new()
    {
        Id             = id,
        Title          = "Test Task",
        Description    = "A test task",
        Status         = TaskItemStatus.Todo,
        ProjectId      = projectId,
        AssignedUserId = userId
    };

    public static Comment CreateComment(int id = 1, int taskId = 1) => new()
    {
        Id      = id,
        Content = "Test comment content",
        TaskId  = taskId
    };
}
