namespace TaskManagement.Core.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    // Foreign key to TaskItem
    public int TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;
}
