using FluentAssertions;
using TaskManagement.Application.Services;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.Enums;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Tests.Helpers;
using Xunit;

namespace TaskManagement.Tests.Services;

public class TaskItemServiceTests
{
    private readonly string _dbName;
    private readonly IMapper _mapper;

    public TaskItemServiceTests()
    {
        _dbName = Guid.NewGuid().ToString();
        _mapper = TestMapperFactory.Create();
    }

    private (TaskItemService service, Infrastructure.Data.AppDbContext context) CreateService()
    {
        var context     = TestDbContextFactory.Create(_dbName);
        var taskRepo    = new TaskItemRepository(context);
        var projectRepo = new ProjectRepository(context);
        var userRepo    = new UserRepository(context);
        var service     = new TaskItemService(taskRepo, projectRepo, userRepo, _mapper, context);
        return (service, context);
    }

    // ── CREATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        await context.SaveChangesAsync();

        var request = new CreateTaskItemRequest
        {
            Title     = "New Task",
            ProjectId = 1,
            Status    = TaskItemStatus.Todo
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("New Task");
        result.ProjectId.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingProject_ThrowsBadRequestException()
    {
        // Arrange
        var (service, _) = CreateService();
        var request = new CreateTaskItemRequest
        {
            Title     = "Task",
            ProjectId = 999 // does not exist
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingAssignedUser_ThrowsBadRequestException()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        await context.SaveChangesAsync();

        var request = new CreateTaskItemRequest
        {
            Title          = "Task",
            ProjectId      = 1,
            AssignedUserId = 999 // user does not exist
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(request));
    }

    // ── READ ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsTask()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test Task");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var (service, _) = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.AddRange(
            new Core.Entities.TaskItem { Id = 1, Title = "Todo Task",       Status = TaskItemStatus.Todo,       ProjectId = 1 },
            new Core.Entities.TaskItem { Id = 2, Title = "InProgress Task", Status = TaskItemStatus.InProgress, ProjectId = 1 },
            new Core.Entities.TaskItem { Id = 3, Title = "Done Task",       Status = TaskItemStatus.Done,       ProjectId = 1 }
        );
        await context.SaveChangesAsync();

        var parameters = new TaskItemQueryParameters { Status = TaskItemStatus.Todo };

        // Act
        var result = await service.GetAllAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Status.Should().Be(TaskItemStatus.Todo);
    }

    [Fact]
    public async Task GetAllAsync_WithProjectFilter_ReturnsTasksForProject()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.AddRange(
            TestData.CreateProject(id: 1),
            TestData.CreateProject(id: 2)
        );
        context.TaskItems.AddRange(
            TestData.CreateTaskItem(id: 1, projectId: 1),
            TestData.CreateTaskItem(id: 2, projectId: 1),
            TestData.CreateTaskItem(id: 3, projectId: 2)
        );
        await context.SaveChangesAsync();

        var parameters = new TaskItemQueryParameters { ProjectId = 1 };

        // Act
        var result = await service.GetAllAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(t => t.ProjectId == 1);
    }

    // ── UPDATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesTask()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        await context.SaveChangesAsync();

        var request = new UpdateTaskItemRequest
        {
            Title     = "Updated Title",
            ProjectId = 1,
            Status    = TaskItemStatus.InProgress
        };

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        result.Title.Should().Be("Updated Title");
        result.Status.Should().Be(TaskItemStatus.InProgress);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var (service, _) = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(999, new UpdateTaskItemRequest { Title = "X", ProjectId = 1 }));
    }

    // ── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingId_DeletesTask()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        await context.SaveChangesAsync();

        // Act
        await service.DeleteAsync(1);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var (service, _) = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
    }
}
