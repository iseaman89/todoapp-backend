using MediatR;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.ToDos.Commands.UpdateToDo;

public class UpdateToDoCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public Priority Priority { get; set; }
    public Category Category { get; set; }
    
}