using ToDoApp.Domain.Common;

namespace ToDoApp.Domain.Entities;

public class ToDo : EntityBase
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }

    private ToDo() { }

    private ToDo(Guid userId, string title)
    {
        UserId = userId;
        Title = title;
    }

    public static ToDo Create(Guid userId, string title) => new ToDo(userId, title);

    public void Update(string title)
    {
        Rename(title);
    }

    private void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be null or empty.");
        Title = title.Trim();
    }
    
    public void Complete() => IsCompleted = true;
}