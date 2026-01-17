using MediatR;

namespace ToDoApp.Application.ToDos.Commands.UpdateToDo;

public class UpdateToDoCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    
}