using FluentAssertions;
using TaskManagement.Application.Services;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Tests.Helpers;
using Xunit;

namespace TaskManagement.Tests.Services;

public class CommentServiceTests
{
    private readonly string _dbName;
    private readonly IMapper _mapper;

    public CommentServiceTests()
    {
        _dbName = Guid.NewGuid().ToString();
        _mapper = TestMapperFactory.Create();
    }

    private (CommentService service, Infrastructure.Data.AppDbContext context) CreateService()
    {
        var context     = TestDbContextFactory.Create(_dbName);
        var commentRepo = new CommentRepository(context);
        var taskRepo    = new TaskItemRepository(context);
        var service     = new CommentService(commentRepo, taskRepo, _mapper, context);
        return (service, context);
    }

    // ── CREATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedComment()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        await context.SaveChangesAsync();

        var request = new CreateCommentRequest
        {
            Content = "This is a comment",
            TaskId  = 1
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Content.Should().Be("This is a comment");
        result.TaskId.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingTask_ThrowsBadRequestException()
    {
        // Arrange
        var (service, _) = CreateService();
        var request = new CreateCommentRequest
        {
            Content = "Comment",
            TaskId  = 999 // does not exist
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(request));
    }

    // ── READ ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsComment()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        context.Comments.Add(TestData.CreateComment(id: 1, taskId: 1));
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Content.Should().Be("Test comment content");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var (service, _) = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_WithTaskIdFilter_ReturnsCommentsForTask()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.AddRange(
            TestData.CreateTaskItem(id: 1, projectId: 1),
            TestData.CreateTaskItem(id: 2, projectId: 1)
        );
        context.Comments.AddRange(
            TestData.CreateComment(id: 1, taskId: 1),
            TestData.CreateComment(id: 2, taskId: 1),
            TestData.CreateComment(id: 3, taskId: 2)
        );
        await context.SaveChangesAsync();

        var parameters = new CommentQueryParameters { TaskId = 1 };

        // Act
        var result = await service.GetAllAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(c => c.TaskId == 1);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedResults()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        for (int i = 1; i <= 5; i++)
            context.Comments.Add(TestData.CreateComment(id: i, taskId: 1));
        await context.SaveChangesAsync();

        var parameters = new CommentQueryParameters { Page = 1, PageSize = 3 };

        // Act
        var result = await service.GetAllAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(2);
    }

    // ── UPDATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesComment()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        context.Comments.Add(TestData.CreateComment(id: 1, taskId: 1));
        await context.SaveChangesAsync();

        var request = new UpdateCommentRequest { Content = "Updated content" };

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        result.Content.Should().Be("Updated content");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var (service, _) = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(999, new UpdateCommentRequest { Content = "X" }));
    }

    // ── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingId_DeletesComment()
    {
        // Arrange
        var (service, context) = CreateService();
        context.Projects.Add(TestData.CreateProject(id: 1));
        context.TaskItems.Add(TestData.CreateTaskItem(id: 1, projectId: 1));
        context.Comments.Add(TestData.CreateComment(id: 1, taskId: 1));
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
