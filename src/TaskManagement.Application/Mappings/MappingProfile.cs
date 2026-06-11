using AutoMapper;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.DTOs.Responses;
using TaskManagement.Core.Entities;

namespace TaskManagement.Application.Mappings;

/// <summary>
///never manually copy proprty by property
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── User ───────────────────────────────────────────
        CreateMap<User, UserResponse>();
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>();

        // ── Project ────────────────────────────────────────
        CreateMap<Project, ProjectResponse>()
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count));
        CreateMap<CreateProjectRequest, Project>();
        CreateMap<UpdateProjectRequest, Project>();

        // ── TaskItem ───────────────────────────────────────
        CreateMap<TaskItem, TaskItemResponse>()
            .ForMember(dest => dest.ProjectName,
                       opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
            .ForMember(dest => dest.AssignedUserName,
                       opt => opt.MapFrom(src => src.AssignedUser != null
                           ? $"{src.AssignedUser.FirstName} {src.AssignedUser.LastName}"
                           : null))
            .ForMember(dest => dest.CommentCount,
                       opt => opt.MapFrom(src => src.Comments.Count));
        CreateMap<CreateTaskItemRequest, TaskItem>();
        CreateMap<UpdateTaskItemRequest, TaskItem>();

        // ── Comment ────────────────────────────────────────
        CreateMap<Comment, CommentResponse>()
            .ForMember(dest => dest.TaskTitle,
                       opt => opt.MapFrom(src => src.Task != null ? src.Task.Title : string.Empty));
        CreateMap<CreateCommentRequest, Comment>();
        CreateMap<UpdateCommentRequest, Comment>();
    }
}
