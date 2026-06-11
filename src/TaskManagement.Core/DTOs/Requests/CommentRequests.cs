namespace TaskManagement.Core.DTOs.Requests;

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
}

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
}
