using FluentValidation;

namespace TestContainers101.Api.Models;

public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItemRequest>
{

    public UpdateTodoItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Note)
            .MaximumLength(1000);

        RuleFor(x => x.IsComplete)
            .NotNull();
    }
}
