using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.ToDos.Queries.GetToDoById;

public record ToDoDetailsDto(Guid Id, string Title, bool IsCompleted, Priority Priority, Category Category, DateTime CreatedAt);