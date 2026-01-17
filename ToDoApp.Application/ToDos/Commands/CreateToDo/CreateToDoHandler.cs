using MapsterMapper;
using MediatR;
using ToDoApp.Domain.Entities;
using ToDoApp.Application.Common.Interfaces;

namespace ToDoApp.Application.ToDos.Commands.CreateToDo;

public class CreateToDoHandler : IRequestHandler<CreateToDoCommand, CreateToDoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;

    public CreateToDoHandler(IApplicationDbContext context, IMapper mapper, IUserContextService _userContextService)
    {
        _context = context;
        _mapper = mapper;
        this._userContextService = _userContextService;
    }
    
    public async Task<CreateToDoDto> Handle(CreateToDoCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        if (userId is null) throw new UnauthorizedAccessException();

        var toDo = ToDo.Create(userId.Value, request.Title);

        _context.ToDos.Add(toDo);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CreateToDoDto>(toDo);
    }
}