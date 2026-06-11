using TaskManagement.Core.Enums;

namespace TaskManagement.Core.DTOs.Requests;

/// <summary>
/// Base query parameters shared by all list endpoints
/// </summary>
public class QueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

public class UserQueryParameters : QueryParameters
{
    public string? SearchName { get; set; }   // filter by first or last name
    public string? SearchEmail { get; set; }  // filter by email
}

public class ProjectQueryParameters : QueryParameters
{
    public string? SearchName { get; set; }   // filter by project name
}

public class TaskItemQueryParameters : QueryParameters
{
    public string? SearchTitle { get; set; }           // filter by title
    public int? ProjectId { get; set; }                // filter by project
    public int? AssignedUserId { get; set; }           // filter by assigned user
    public TaskItemStatus? Status { get; set; }        // filter by status
}

public class CommentQueryParameters : QueryParameters
{
    public int? TaskId { get; set; }                   // filter by task
}
