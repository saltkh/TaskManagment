using FluentAssertions;
using TaskManagement.Application.Services;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Tests.Helpers;
using Xunit;

namespace TaskManagement.Tests.Services;

public class ProjectServiceTests
{
    private readonly string _dbName;
    private readonly IMapper _mapper;

    public ProjectServiceTests()
    {
        _dbName = Guid.NewGuid().ToString();
        _mapper = TestMapperFactory.Create();
    }

    private ProjectService CreateService()
    {
        var context = TestDbContextFactory.Create(_dbName);
        var repo    = new ProjectRepository(context);
        return new ProjectService(repo, _mapper, context);
    }

    // ── CREATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedProject()
    {
        // Arrange
        var service = CreateService();
        var request = new CreateProjectRequest { Name = "New Project", Description = "Desc" };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Project");
    }

    // ── READ ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsProject()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Projects.Add(TestData.CreateProject(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Project");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_WithNameFilter_ReturnsMatchingProjects()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Projects.AddRange(
            new Core.Entities.Project { Id = 1, Name = "Alpha Project" },
            new Core.Entities.Project { Id = 2, Name = "Beta Project" },
            new Core.Entities.Project { Id = 3, Name = "Alpha Two" }
        );
        await context.SaveChangesAsync();

        var service    = CreateService();
        var parameters = new ProjectQueryParameters { SearchName = "Alpha" };

        // Act
        var result = await service.GetAllAsync(parameters);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().OnlyContain(p => p.Name.Contains("Alpha"));
    }

    // ── UPDATE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesProject()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Projects.Add(TestData.CreateProject(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();
        var request = new UpdateProjectRequest { Name = "Renamed Project" };

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        result.Name.Should().Be("Renamed Project");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.UpdateAsync(999, new UpdateProjectRequest { Name = "X" }));
    }

    // ── DELETE ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithExistingId_DeletesProject()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Projects.Add(TestData.CreateProject(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();

        // Act
        await service.DeleteAsync(1);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
    }
}
