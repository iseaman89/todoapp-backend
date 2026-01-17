using FluentValidation;

namespace ToDoApp.Application.ToDos.Commands.CreateToDo;

public class CreateToDoCommandValidator : AbstractValidator<CreateToDoCommand>
{
    public CreateToDoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}