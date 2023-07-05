using FluentValidation;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Infra.Persistence;
using TestContainers101.Api.Models;

namespace TestContainers101.Api.Endpoints;

public static class TodoItemEndpoints
{
    public static RouteGroupBuilder MapTodoItemEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllTodoItemAsync);

        group.MapGet("/{id}", GetTodoItemByIdAsync)
            .WithName(nameof(GetTodoItemByIdAsync));

        group.MapPost("/", PostTodoItemAsync);

        group.MapPut("/{id}", PutTodoItemAsync);

        group.MapDelete("/{id}", DeleteTodoItemByIdAsync);

        return group;
    }

    static private ProblemHttpResult NotFoundProblem(long id)
        => TypedResults.Problem($"TodoItem with id={id} is not found.", statusCode: StatusCodes.Status404NotFound);

    static private bool ValidateTodoId(long id)
        => id <= 0;

    static async Task<IResult> GetAllTodoItemAsync(
        [FromServices] AppDbContext dbContext,
        CancellationToken token
    )
    {
        var todoItems = await dbContext.TodoItems
            .AsNoTracking()
            .ToListAsync(token);

        return TypedResults.Ok(todoItems);
    }

    static async Task<IResult> GetTodoItemByIdAsync(
        [FromServices] AppDbContext dbContext,
        [FromRoute] long id,
        CancellationToken token
    )
    {
        // Id is unsigned, so it can't be negative
        if (ValidateTodoId(id))
        {
            return NotFoundProblem(id);
        }

        var todoItem = await dbContext.TodoItems
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), token);

        if (todoItem is null)
        {
            return NotFoundProblem(id);
        }

        return TypedResults.Ok(todoItem);
    }

    static async Task<IResult> PostTodoItemAsync(
        [FromServices] AppDbContext dbContext,
        [FromServices] IValidator<CreateTodoItemRequest> validator,
        [FromBody] CreateTodoItemRequest createTodoItemRequest,
        CancellationToken token
    )
    {
        var validationResult = await validator.ValidateAsync(createTodoItemRequest, token);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todoItem = new TodoItem
        {
            Title = createTodoItemRequest.Title,
            Note = createTodoItemRequest.Note,
        };

        await dbContext.TodoItems.AddAsync(todoItem, token);
        await dbContext.SaveChangesAsync(token);

        return TypedResults.CreatedAtRoute(nameof(GetTodoItemByIdAsync), new { todoItem.Id });
    }

    static async Task<IResult> PutTodoItemAsync(
        [FromServices] AppDbContext dbContext,
        [FromServices] IValidator<UpdateTodoItemRequest> validator,
        [FromRoute] long id,
        [FromBody] UpdateTodoItemRequest updateTodoItemRequest,
        CancellationToken token
    )
    {
        if (ValidateTodoId(id))
        {
            return NotFoundProblem(id);
        }

        var validationResult = await validator.ValidateAsync(updateTodoItemRequest, token);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var todoItem = await dbContext.TodoItems
            .SingleOrDefaultAsync(x => x.Id.Equals(id), token);

        if (todoItem is null)
        {
            return NotFoundProblem(id);
        }

        todoItem.Title = updateTodoItemRequest.Title;
        todoItem.Note = updateTodoItemRequest.Note;
        todoItem.IsComplete = updateTodoItemRequest.IsComplete;
        todoItem.ModifiedOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(token);

        return TypedResults.Ok(todoItem);
    }

    static async Task<IResult> DeleteTodoItemByIdAsync(
        [FromServices] AppDbContext dbContext,
        [FromRoute] long id,
        CancellationToken token
    )
    {
        if (ValidateTodoId(id))
        {
            return NotFoundProblem(id);
        }

        var todoItem = await dbContext.TodoItems
            .SingleOrDefaultAsync(x => x.Id.Equals(id), token);

        if (todoItem is null)
        {
            return NotFoundProblem(id);
        }

        dbContext.TodoItems.Remove(todoItem);
        await dbContext.SaveChangesAsync(token);

        return TypedResults.NoContent();
    }
}
