using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs.Requests;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskService;
    private readonly IValidator<CreateTaskItemRequest> _createValidator;
    private readonly IValidator<UpdateTaskItemRequest> _updateValidator;

    public TaskItemsController(
        ITaskItemService taskService,
        IValidator<CreateTaskItemRequest> createValidator,
        IValidator<UpdateTaskItemRequest> updateValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>Get all tasks with optional filtering (by project, user, status), sorting and pagination.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskItemQueryParameters parameters)
    {
        var result = await _taskService.GetAllAsync(parameters);
        return Ok(result);
    }

    /// <summary>Get a single task by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _taskService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>Create a new task.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskItemRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await _taskService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Update an existing task.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskItemRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await _taskService.UpdateAsync(id, request);
        return Ok(result);
    }

    /// <summary>Delete a task by id.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteAsync(id);
        return NoContent();
    }
}
