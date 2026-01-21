using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Application.ToDos.Commands.UpdateToDo;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Application.Tests.ToDos;

public class UpdateToDoTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly UpdateToDoCommandHandler _handler;

    public UpdateToDoTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _handler = new UpdateToDoCommandHandler(
            _context,
            _userContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Update_Todo_When_User_Is_Authorized()
    {
        var userId = Guid.NewGuid();

        var todo = ToDo.Create(
            userId,
            "Old title",
            Priority.MEDIUM,
            Category.PERSONAL
        );

        _context.ToDos.Add(todo);
        await _context.SaveChangesAsync();

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        var command = new UpdateToDoCommand
        {
            Id = todo.Id,
            Title = "Updated title",
            IsCompleted = true,
            Priority = Priority.LOW,
            Category = Category.PERSONAL
        };
        
        await _handler.Handle(command, CancellationToken.None);
        
        var updatedTodo = await _context.ToDos.FirstAsync();

        updatedTodo.Title.Should().Be("Updated title");
        updatedTodo.IsCompleted.Should().BeTrue();
        updatedTodo.Priority.Should().Be(Priority.LOW);
    }
    
    [Fact]
    public async Task Handle_Should_Throw_When_User_Is_Not_Authorized()
    {
        var command = new UpdateToDoCommand()
        {
            Title = "Learn .NET",
            Priority = Priority.MEDIUM,
            Category = Category.PERSONAL
        };

        _userContextMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);
        
        Func<Task> act = async () =>
            await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}