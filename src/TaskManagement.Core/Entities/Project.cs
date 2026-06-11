namespace TaskManagement.Core.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation property - one project has many tasks
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
