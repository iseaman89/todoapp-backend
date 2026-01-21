using ToDoApp.Domain.Common;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Domain.Entities;

public class ToDo : EntityBase
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    public Priority Priority { get; private set; }
    public Category Category { get; private set; }

    private ToDo() { }

    private ToDo(Guid userId, string title, Priority priority, Category category)
    {
        UserId = userId;
        Title = title;
        Priority = priority;
        Category = category;
    }

    public static ToDo Create(Guid userId, string title, Priority priority, Category category) => 
        new ToDo(userId, title, priority, category);

    public void Update(string title, bool isCompleted, Priority priority, Category category)
    {
        Rename(title);
        CompleteChange(isCompleted);
        ChangePriority(priority);
        ChangeCategory(category);
    }

    private void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be null or empty.");
        Title = title.Trim();
    }
    
    private void ChangePriority(Priority priority) => Priority = priority;
    
    private void ChangeCategory(Category category) => Category = category;
    
    private void CompleteChange(bool isCompleted) => IsCompleted = isCompleted;
}