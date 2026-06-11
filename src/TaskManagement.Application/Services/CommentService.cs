using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly ITaskItemRepository _taskRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public CommentService(
        ICommentRepository commentRepo,
        ITaskItemRepository taskRepo,
        IMapper mapper,
        AppDbContext context)
    {
        _commentRepo = commentRepo;
        _taskRepo = taskRepo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PagedResponse<CommentResponse>> GetAllAsync(CommentQueryParameters parameters)
    {
        var query = _context.Comments
            .Include(c => c.Task)
            .AsQueryable();

        // Filter by task
        if (parameters.TaskId.HasValue)
            query = query.Where(c => c.TaskId == parameters.TaskId.Value);

        // Sort
        query = parameters.SortBy?.ToLower() switch
        {
            "taskid" => parameters.SortDescending
                ? query.OrderByDescending(c => c.TaskId)
                : query.OrderBy(c => c.TaskId),
            _ => query.OrderBy(c => c.Id)
        };

        var totalCount = await query.CountAsync();

        var comments = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<CommentResponse>
        {
            Items = _mapper.Map<IEnumerable<CommentResponse>>(comments),
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<CommentResponse> GetByIdAsync(int id)
    {
        var comment = await _commentRepo.GetByIdWithTaskAsync(id)
            ?? throw new NotFoundException("Comment", id);
        return _mapper.Map<CommentResponse>(comment);
    }

    public async Task<CommentResponse> CreateAsync(CreateCommentRequest request)
    {
        // Validate that the task exists
        if (!await _taskRepo.ExistsAsync(request.TaskId))
            throw new BadRequestException($"TaskItem with id {request.TaskId} does not exist.");

        var comment = _mapper.Map<Core.Entities.Comment>(request);
        var created = await _commentRepo.CreateAsync(comment);

        // Reload with navigation properties for full response
        return await GetByIdAsync(created.Id);
    }

    public async Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request)
    {
        var comment = await _commentRepo.GetByIdWithTaskAsync(id)
            ?? throw new NotFoundException("Comment", id);

        _mapper.Map(request, comment);
        await _commentRepo.UpdateAsync(comment);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var comment = await _commentRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Comment", id);
        await _commentRepo.DeleteAsync(comment);
    }
}
