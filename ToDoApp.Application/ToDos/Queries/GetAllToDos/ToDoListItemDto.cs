namespace ToDoApp.Application.ToDos.Queries.GetAllToDos;

public record ToDoListItemDto(Guid Id, string Title, bool IsCompleted, DateTime CreatedAt);