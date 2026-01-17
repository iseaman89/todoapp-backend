using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Common.Interfaces;

namespace ToDoApp.Application.ToDos.Queries.GetAllToDos;

public class GetAllToDosQueryHandler : IRequestHandler<GetAllTodosQuery, List<ToDoListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public GetAllToDosQueryHandler(IApplicationDbContext context, IUserContextService userContextService, IMapper mapper)
    {
        _context = context;
        _userContextService = userContextService;
        _mapper = mapper;
    }
    
    public async Task<List<ToDoListItemDto>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        if (userId is null) throw new UnauthorizedAccessException();

        var toDos = await _context.ToDos.Where(t => t.UserId == userId.Value).ToListAsync(cancellationToken);

        return _mapper.Map<List<ToDoListItemDto>>(toDos);
    }
}