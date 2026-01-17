using FluentValidation;

namespace ToDoApp.Application.ToDos.Commands.UpdateToDo;

public class UpdateToDoCommandValidator : AbstractValidator<UpdateToDoCommand>
{
    public UpdateToDoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}