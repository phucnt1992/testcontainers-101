namespace TestContainers101.Api.Tests.Fakers;

using Bogus;

using TestContainers101.Api.Entities;

public class TodoItemFaker : Faker<TodoItem>
{
    public TodoItemFaker()
    {
        RuleFor(x => x.Title, f => f.Lorem.Sentence());
        RuleFor(x => x.Note, f => f.Lorem.Paragraphs(3));
    }

    public TodoItemFaker WithRandomIsComplete()
    {
        RuleFor(x => x.IsComplete, f => f.Random.Bool());
        return this;
    }
}
