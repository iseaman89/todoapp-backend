using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ToDo> ToDos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}