using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoApp.Api.Controllers;
using ToDoApp.Application.ToDos.Commands.CreateToDo;
using ToDoApp.Application.ToDos.Commands.DeleteToDo;
using ToDoApp.Application.ToDos.Commands.UpdateToDo;
using ToDoApp.Application.ToDos.Queries.GetAllToDos;
using ToDoApp.Application.ToDos.Queries.GetToDoById;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Api.Tests;

public class ToDoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly ToDoController _controller;

    public ToDoControllerTests()
    {
        _controller = new ToDoController(_mediatorMock.Object);
    }
    
    [Fact]
    public async Task GetAll_Should_Return_Ok_With_Data()
    {
        var response = new List<ToDoListItemDto>();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTodosQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        var result = await _controller.GetAll(CancellationToken.None);
        
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);
        ok.Value.Should().Be(response);
    }
    
    [Fact]
    public async Task GetById_Should_Return_Ok()
    {
        var id = Guid.NewGuid();
        var dto = new ToDoDetailsDto(id, "Test", false, Priority.LOW, Category.WORK, DateTime.Now);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetToDoByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetById(id, CancellationToken.None);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().Be(dto);
    }
    
    [Fact]
    public async Task Create_Should_Return_Ok()
    {
        var command = new CreateToDoCommand();
        var dto = new CreateToDoDto(new Guid(), "Test", true, Priority.LOW, Category.WORK);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Create(command, CancellationToken.None);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().Be(dto);
    }
    
    [Fact]
    public async Task Update_Should_Return_Ok()
    {
        var command = new UpdateToDoCommand { Title = "Updated" };

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _controller.Update(command, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task Delete_Should_Call_Mediator_And_Return_Ok()
    {
        var id = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteToDoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _controller.Delete(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>();

        _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<DeleteToDoCommand>(c => c.Id == id),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}