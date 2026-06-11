namespace TaskManagement.Application.Services;

/// <summary>
/// Thrown when a requested resource does not exist in the database.
/// The global exception handler converts this to a 404 response.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, int id)
        : base($"{entityName} with id {id} was not found.") { }
}

/// <summary>
/// Thrown when a request contains invalid business-logic data (not format errors).
/// The global exception handler converts this to a 400 response.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
