using FluentAssertions;
using Moq;
using MapsterMapper;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Application.ToDos.Commands.CreateToDo;
using ToDoApp.Domain.Entities;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Application.Tests.ToDos;

public class CreateToDoTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContextService> _userContextMock = new();

    private readonly CreateToDoHandler _handler;

    public CreateToDoTests()
    {
        _handler = new CreateToDoHandler(
            _contextMock.Object,
            _mapperMock.Object,
            _userContextMock.Object
        );
    }

    [Fact]
    public async Task Handle_Should_Create_Todo_When_User_Is_Authorized()
    {
        var userId = Guid.NewGuid();
        var command = new CreateToDoCommand(){
            Title = "Learn .NET",
            Priority = Priority.MEDIUM,
            Category = Category.PERSONAL
        };

        _userContextMock
            .Setup(x => x.UserId)
            .Returns(userId);

        CreateToDoDto dto = new(userId, command.Title, false, command.Priority, command.Category);

        _mapperMock
            .Setup(m => m.Map<CreateToDoDto>(It.IsAny<ToDo>()))
            .Returns(dto);

        _contextMock
            .Setup(c => c.ToDos.Add(It.IsAny<ToDo>()));

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.Should().NotBeNull();
        result.Title.Should().Be("Learn .NET");

        _contextMock.Verify(
            c => c.ToDos.Add(It.IsAny<ToDo>()),
            Times.Once
        );

        _contextMock.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
    
    [Fact]
    public async Task Handle_Should_Throw_When_User_Is_Not_Authorized()
    {
        var command = new CreateToDoCommand()
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