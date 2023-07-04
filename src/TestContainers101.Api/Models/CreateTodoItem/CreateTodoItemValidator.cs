namespace TestContainers101.Api.Models;

using FluentValidation;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemRequest>
{
    public CreateTodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Note)
            .MaximumLength(1000);
    }
}
