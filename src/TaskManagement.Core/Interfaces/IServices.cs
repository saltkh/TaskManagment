using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;

namespace TaskManagement.Core.Interfaces;

public interface IUserService
{
    Task<PagedResponse<UserResponse>> GetAllAsync(UserQueryParameters parameters);
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}

public interface IProjectService
{
    Task<PagedResponse<ProjectResponse>> GetAllAsync(ProjectQueryParameters parameters);
    Task<ProjectResponse> GetByIdAsync(int id);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);
    Task DeleteAsync(int id);
}

public interface ITaskItemService
{
    Task<PagedResponse<TaskItemResponse>> GetAllAsync(TaskItemQueryParameters parameters);
    Task<TaskItemResponse> GetByIdAsync(int id);
    Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request);
    Task<TaskItemResponse> UpdateAsync(int id, UpdateTaskItemRequest request);
    Task DeleteAsync(int id);
}

public interface ICommentService
{
    Task<PagedResponse<CommentResponse>> GetAllAsync(CommentQueryParameters parameters);
    Task<CommentResponse> GetByIdAsync(int id);
    Task<CommentResponse> CreateAsync(CreateCommentRequest request);
    Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request);
    Task DeleteAsync(int id);
}
