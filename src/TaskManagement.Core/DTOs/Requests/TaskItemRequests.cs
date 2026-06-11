using TaskManagement.Core.Enums;

namespace TaskManagement.Core.DTOs.Requests;

public class CreateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
}

public class UpdateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
}
