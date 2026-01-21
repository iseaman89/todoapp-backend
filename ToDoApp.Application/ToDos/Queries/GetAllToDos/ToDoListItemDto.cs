using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.ToDos.Queries.GetAllToDos;

public record ToDoListItemDto(Guid Id, string Title, bool IsCompleted, Priority Priority, Category Category, DateTime CreatedAt);