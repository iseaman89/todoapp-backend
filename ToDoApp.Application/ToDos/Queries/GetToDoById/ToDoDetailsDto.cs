namespace ToDoApp.Application.ToDos.Queries.GetToDoById;

public record ToDoDetailsDto(Guid Id, string Title, bool IsCompleted, DateTime CreatedAt);