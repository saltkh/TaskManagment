using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ProjectService(IProjectRepository projectRepo, IMapper mapper, AppDbContext context)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PagedResponse<ProjectResponse>> GetAllAsync(ProjectQueryParameters parameters)
    {
        var query = _context.Projects.Include(p => p.Tasks).AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(parameters.SearchName))
            query = query.Where(p => p.Name.ToLower().Contains(parameters.SearchName.ToLower()));

        // Sort
        query = parameters.SortBy?.ToLower() switch
        {
            "name" => parameters.SortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.Id)
        };

        var totalCount = await query.CountAsync();

        var projects = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<ProjectResponse>
        {
            Items = _mapper.Map<IEnumerable<ProjectResponse>>(projects),
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<ProjectResponse> GetByIdAsync(int id)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id)
            ?? throw new NotFoundException("Project", id);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = _mapper.Map<Core.Entities.Project>(request);
        var created = await _projectRepo.CreateAsync(project);
        return _mapper.Map<ProjectResponse>(created);
    }

    public async Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request)
    {
        var project = await _projectRepo.GetByIdWithTasksAsync(id)
            ?? throw new NotFoundException("Project", id);

        _mapper.Map(request, project);
        var updated = await _projectRepo.UpdateAsync(project);
        return _mapper.Map<ProjectResponse>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _projectRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("Project", id);
        await _projectRepo.DeleteAsync(project);
    }
}
