namespace TaskManagement.Application.Services;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, int id)
        : base($"{entityName} with id {id} was not found.") { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
