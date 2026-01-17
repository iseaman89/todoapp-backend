using MediatR;

namespace ToDoApp.Application.ToDos.Queries.GetAllToDos;

public record GetAllTodosQuery() : IRequest<List<ToDoListItemDto>>;