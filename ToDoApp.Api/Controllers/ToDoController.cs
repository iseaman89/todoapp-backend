using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.ToDos.Commands.CreateToDo;
using ToDoApp.Application.ToDos.Commands.DeleteToDo;
using ToDoApp.Application.ToDos.Commands.UpdateToDo;
using ToDoApp.Application.ToDos.Queries.GetAllToDos;
using ToDoApp.Application.ToDos.Queries.GetToDoById;

namespace ToDoApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ToDoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ToDoController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(GetAllTodosQuery query, CancellationToken cancellationToken)
    {
        var toDos = await _mediator.Send(query, cancellationToken);
        return Ok(toDos);
    }
    
    [HttpGet("id")]
    public async Task<IActionResult> GetById(GetToDoByIdQuery query, CancellationToken cancellationToken)
    {
        var toDo = await _mediator.Send(query, cancellationToken);
        return Ok(toDo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateToDoCommand command, CancellationToken cancellationToken)
    {
        var toDo = await _mediator.Send(command, cancellationToken);
        return Ok(toDo);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateToDoCommand command, CancellationToken cancellationToken)
    {
        var unit = await _mediator.Send(command, cancellationToken);
        return Ok(unit);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteToDoCommand command, CancellationToken cancellationToken)
    {
        var unit = await _mediator.Send(command, cancellationToken);
        return Ok(unit);
    }
}