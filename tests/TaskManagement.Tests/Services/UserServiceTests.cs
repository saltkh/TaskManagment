using FluentAssertions;
using Moq;
using TaskManagement.Application.Services;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.Interfaces;
using TaskManagement.Tests.Helpers;
using Xunit;

namespace TaskManagement.Tests.Services;

public class UserServiceTests
{
    // Each test gets its own isolated in-memory database
    private readonly string _dbName;
    private readonly IMapper _mapper;

    public UserServiceTests()
    {
        _dbName = Guid.NewGuid().ToString(); // unique name = isolated DB per test
        _mapper = TestMapperFactory.Create();
    }

    private UserService CreateService()
    {
        var context = TestDbContextFactory.Create(_dbName);
        var repo = new UserRepository(context);
        return new UserService(repo, _mapper, context);
    }

//CREATE

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedUser()
    {
        // Arrange
        var service = CreateService();
        var request = new CreateUserRequest
        {
            FirstName = "Jane",
            LastName  = "Smith",
            Email     = "jane@example.com"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ThrowsBadRequestException()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.Add(TestData.CreateUser(email: "duplicate@example.com"));
        await context.SaveChangesAsync();

        var service = CreateService();
        var request = new CreateUserRequest
        {
            FirstName = "Another",
            LastName  = "User",
            Email     = "duplicate@example.com" // igive maili
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => service.CreateAsync(request));
    }

//read
    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.Add(TestData.CreateUser(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResults()
    {
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.AddRange(
            TestData.CreateUser(id: 1, email: "user1@example.com"),
            TestData.CreateUser(id: 2, email: "user2@example.com"),
            TestData.CreateUser(id: 3, email: "user3@example.com")
        );
        await context.SaveChangesAsync();

        var service = CreateService();
        var parameters = new UserQueryParameters { Page = 1, PageSize = 2 };

        var result = await service.GetAllAsync(parameters);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_WithSearchFilter_ReturnsFilteredResults()
    {
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.AddRange(
            new Core.Entities.User { Id = 1, FirstName = "Alice", LastName = "Brown", Email = "alice@example.com" },
            new Core.Entities.User { Id = 2, FirstName = "Bob",   LastName = "Jones", Email = "bob@example.com" }
        );
        await context.SaveChangesAsync();

        var service = CreateService();
        var parameters = new UserQueryParameters { SearchName = "alice" };

        var result = await service.GetAllAsync(parameters);

        result.Items.Should().HaveCount(1);
        result.Items.First().FirstName.Should().Be("Alice");
    }

//update

    [Fact]
    public async Task UpdateAsync_WithExistingId_ReturnsUpdatedUser()
    {
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.Add(TestData.CreateUser(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();
        var request = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName  = "Name",
            Email     = "updated@example.com"
        };

        var result = await service.UpdateAsync(1, request);

    
        result.FirstName.Should().Be("Updated");
        result.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        
        var service = CreateService();
        var request = new UpdateUserRequest
        {
            FirstName = "X",
            LastName  = "Y",
            Email     = "x@example.com"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(999, request));
    }

//delete
    [Fact]
    public async Task DeleteAsync_WithExistingId_DeletesUser()
    {
        var context = TestDbContextFactory.Create(_dbName);
        context.Users.Add(TestData.CreateUser(id: 1));
        await context.SaveChangesAsync();

        var service = CreateService();

        await service.DeleteAsync(1);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ThrowsNotFoundException()
    {
        
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
    }
}
