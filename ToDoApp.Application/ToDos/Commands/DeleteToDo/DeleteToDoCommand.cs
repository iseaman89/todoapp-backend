using MediatR;

namespace ToDoApp.Application.ToDos.Commands.DeleteToDo;

public record DeleteToDoCommand(Guid Id) : IRequest<Unit>;