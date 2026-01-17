namespace ToDoApp.Application.Common.Interfaces;

public interface IUserContextService
{
    Guid? UserId { get; }
    string? UserName { get; }
}