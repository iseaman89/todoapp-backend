using MediatR;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.ToDos.Commands.CreateToDo;

public class CreateToDoCommand : IRequest<CreateToDoDto>
{
    public string Title { get; set; } = default!;
    public Priority Priority { get; set; }
    public Category Category { get; set; }
}