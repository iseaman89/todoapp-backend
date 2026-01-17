using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Common.Interfaces;

namespace ToDoApp.Application.ToDos.Queries.GetToDoById;

public class GetToDoByIdQueryHandler : IRequestHandler<GetToDoByIdQuery, ToDoDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetToDoByIdQueryHandler(IApplicationDbContext context, IUserContextService userContextService, IMapper mapper)
    {
        _context = context;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    
    public async Task<ToDoDetailsDto> Handle(GetToDoByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        if (userId is null) throw new UnauthorizedAccessException();

        var toDo = await _context.ToDos.Where(t => t.Id == request.Id && t.UserId == userId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (toDo is null) throw new Exception("ToDo not found");

        return _mapper.Map<ToDoDetailsDto>(toDo);
    }
}