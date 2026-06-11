using TaskManagement.Core.Enums;

namespace TaskManagement.Core.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    // Foreign key to Project
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Foreign key to User (nullable - task may be unassigned)
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }

    // Navigation property - one task can have many comments
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
