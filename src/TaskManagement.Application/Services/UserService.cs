using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public UserService(IUserRepository userRepo, IMapper mapper, AppDbContext context)
    {
        _userRepo = userRepo;
        _mapper = mapper;
        _context = context;
    }

    public async Task<PagedResponse<UserResponse>> GetAllAsync(UserQueryParameters parameters)
    {
        // Start with all users
        var query = _context.Users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(parameters.SearchName))
        {
            var search = parameters.SearchName.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchEmail))
            query = query.Where(u => u.Email.ToLower().Contains(parameters.SearchEmail.ToLower()));

        // Apply sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "firstname" => parameters.SortDescending
                ? query.OrderByDescending(u => u.FirstName)
                : query.OrderBy(u => u.FirstName),
            "lastname" => parameters.SortDescending
                ? query.OrderByDescending(u => u.LastName)
                : query.OrderBy(u => u.LastName),
            "email" => parameters.SortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            _ => query.OrderBy(u => u.Id)
        };

        var totalCount = await query.CountAsync();

        // Apply pagination
        var users = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResponse<UserResponse>
        {
            Items = _mapper.Map<IEnumerable<UserResponse>>(users),
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        if (await _userRepo.EmailExistsAsync(request.Email))
            throw new BadRequestException($"A user with email '{request.Email}' already exists.");

        var user = _mapper.Map<Core.Entities.User>(request);
        var created = await _userRepo.CreateAsync(user);
        return _mapper.Map<UserResponse>(created);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await _userRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);

        if (await _userRepo.EmailExistsAsync(request.Email, excludeId: id))
            throw new BadRequestException($"A user with email '{request.Email}' already exists.");

        _mapper.Map(request, user);
        var updated = await _userRepo.UpdateAsync(user);
        return _mapper.Map<UserResponse>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id)
            ?? throw new NotFoundException("User", id);
        await _userRepo.DeleteAsync(user);
    }
}
