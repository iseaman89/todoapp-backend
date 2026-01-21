using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Application.ToDos.Commands.DeleteToDo;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Application.Tests.ToDos;

public class DeleteToDoTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserContextService> _userContextMock = new();
    private readonly DeleteToDoCommandHandler _handler;

    public DeleteToDoTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _handler = new DeleteToDoCommandHandler(
            _context,
            _userContextMock.Object
        );
    }
    
    [Fact]
    public async Task Handle_Should_Delete_Todo_When_User_Is_Authorized()
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

        var command = new DeleteToDoCommand(todo.Id);
        
        await _handler.Handle(command, CancellationToken.None);
        
        var deletedTodo = await _context.ToDos.FindAsync(todo.Id);
        deletedTodo.Should().BeNull();
    }
    
    [Fact]
    public async Task Handle_Should_Throw_When_User_Is_Not_Authorized()
    {
        var command = new DeleteToDoCommand(new Guid());

        _userContextMock
            .Setup(x => x.UserId)
            .Returns((Guid?)null);
        
        Func<Task> act = async () =>
            await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}