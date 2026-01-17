using MediatR;

namespace ToDoApp.Application.ToDos.Commands.CreateToDo;

public class CreateToDoCommand : IRequest<CreateToDoDto>
{
    public string Title { get; set; } = default!;
}