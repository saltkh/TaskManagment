namespace TaskManagement.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation property - one user can have many assigned tasks
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
