using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public TaskItemService(
        ITaskItemRepository taskRepo,
        IProjectRepository projectRepo,
        IUserRepository userRepo,
        IMapper mapper,
        AppDbContext context)
    {
        _taskRepo = taskRepo;
        _projectRepo = projectRepo;
        _userRepo = userRepo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PagedResponse<TaskItemResponse>> GetAllAsync(TaskItemQueryParameters parameters)
    {
        var query = _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.AssignedUser)
            .Include(t => t.Comments)
            .AsQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(parameters.SearchTitle))
            query = query.Where(t => t.Title.ToLower().Contains(parameters.SearchTitle.ToLower()));

        if (parameters.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == parameters.ProjectId.Value);

        if (parameters.AssignedUserId.HasValue)
            query = query.Where(t => t.AssignedUserId == parameters.AssignedUserId.Value);

        if (parameters.Status.HasValue)
            query = query.Where(t => t.Status == parameters.Status.Value);

        // Sort
        query = parameters.SortBy?.ToLower() switch
        {
            "title"  => parameters.SortDescending ? query.OrderByDescending(t => t.Title)  : query.OrderBy(t => t.Title),
            "status" => parameters.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            _        => query.OrderBy(t => t.Id)
        };

        var totalCount = await query.CountAsync();

        var tasks = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<TaskItemResponse>
        {
            Items = _mapper.Map<IEnumerable<TaskItemResponse>>(tasks),
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<TaskItemResponse> GetByIdAsync(int id)
    {
        var task = await _taskRepo.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException("TaskItem", id);
        return _mapper.Map<TaskItemResponse>(task);
    }

    public async Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request)
    {
        // Validate that the project exists
        if (!await _projectRepo.ExistsAsync(request.ProjectId))
            throw new BadRequestException($"Project with id {request.ProjectId} does not exist.");

        // Validate that the assigned user exists (if provided)
        if (request.AssignedUserId.HasValue && !await _userRepo.ExistsAsync(request.AssignedUserId.Value))
            throw new BadRequestException($"User with id {request.AssignedUserId.Value} does not exist.");

        var task = _mapper.Map<Core.Entities.TaskItem>(request);
        var created = await _taskRepo.CreateAsync(task);

        // Reload with navigation properties for full response
        return await GetByIdAsync(created.Id);
    }

    public async Task<TaskItemResponse> UpdateAsync(int id, UpdateTaskItemRequest request)
    {
        var task = await _taskRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("TaskItem", id);

        // Validate project exists
        if (!await _projectRepo.ExistsAsync(request.ProjectId))
            throw new BadRequestException($"Project with id {request.ProjectId} does not exist.");

        // Validate assigned user exists (if provided)
        if (request.AssignedUserId.HasValue && !await _userRepo.ExistsAsync(request.AssignedUserId.Value))
            throw new BadRequestException($"User with id {request.AssignedUserId.Value} does not exist.");

        _mapper.Map(request, task);
        await _taskRepo.UpdateAsync(task);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _taskRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("TaskItem", id);
        await _taskRepo.DeleteAsync(task);
    }
}
