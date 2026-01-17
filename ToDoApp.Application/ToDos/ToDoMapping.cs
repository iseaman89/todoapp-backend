using Mapster;
using ToDoApp.Application.ToDos.Commands.CreateToDo;
using ToDoApp.Application.ToDos.Queries.GetAllToDos;
using ToDoApp.Application.ToDos.Queries.GetToDoById;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.ToDos;

public class ToDoMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ToDo, CreateToDoDto>();
        config.NewConfig<ToDo, ToDoDetailsDto>();
        config.NewConfig<ToDo, ToDoListItemDto>();
    }
}