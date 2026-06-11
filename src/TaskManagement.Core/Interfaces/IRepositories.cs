using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithTasksAsync(int id);
}

public interface ITaskItemRepository : IRepository<TaskItem>
{
    Task<TaskItem?> GetByIdWithDetailsAsync(int id);
}

public interface ICommentRepository : IRepository<Comment>
{
    Task<Comment?> GetByIdWithTaskAsync(int id);
}
