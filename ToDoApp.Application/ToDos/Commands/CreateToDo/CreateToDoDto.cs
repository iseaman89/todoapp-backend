using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.ToDos.Commands.CreateToDo;

public record CreateToDoDto(Guid Id, string Title, bool IsCompleted, Priority Priority, Category Category);
