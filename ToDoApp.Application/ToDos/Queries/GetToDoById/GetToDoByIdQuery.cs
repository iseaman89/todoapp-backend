using MediatR;

namespace ToDoApp.Application.ToDos.Queries.GetToDoById;

public record GetToDoByIdQuery(Guid Id) : IRequest<ToDoDetailsDto>;