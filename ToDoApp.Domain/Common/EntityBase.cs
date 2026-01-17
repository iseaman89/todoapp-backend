namespace ToDoApp.Domain.Common;

public class EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}