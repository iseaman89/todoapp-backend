using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Common.Interfaces;
using ToDoApp.Application.Exceptions;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.ToDos.Commands.DeleteToDo;

public class DeleteToDoCommandHandler : IRequestHandler<DeleteToDoCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContextService _userContextService;

    public DeleteToDoCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }
    
    public async Task<Unit> Handle(DeleteToDoCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        if (userId is null) throw new UnauthorizedAccessException();

        var toDo = await _context.ToDos.Where(t => t.Id == request.Id && t.UserId == userId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (toDo is null) throw new NotFoundException(nameof(ToDo), request.Id);

        _context.ToDos.Remove(toDo);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}