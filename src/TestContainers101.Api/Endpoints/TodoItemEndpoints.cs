using FluentValidation;

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
        if (id <= 0)
        {
            return TypedResults.NotFound(id);
        }

        var todoItem = await dbContext.TodoItems
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), token);

        if (todoItem is null)
        {
            return TypedResults.NotFound(id);
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

        return TypedResults.CreatedAtRoute(todoItem.Id, nameof(GetTodoItemByIdAsync), new { todoItem.Id });
    }

    static async Task<IResult> PutTodoItemAsync(
        [FromServices] AppDbContext dbContext,
        [FromServices] IValidator<UpdateTodoItemRequest> validator,
        [FromRoute] long id,
        [FromBody] UpdateTodoItemRequest updateTodoItemRequest,
        CancellationToken token
    )
    {
        // Id is unsigned, so it can't be negative
        if (id <= 0)
        {
            return TypedResults.NotFound(id);
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
            return TypedResults.NotFound(id);
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
        // Id is unsigned, so it can't be negative
        if (id <= 0)
        {
            return TypedResults.NotFound(id);
        }

        var todoItem = await dbContext.TodoItems
            .SingleOrDefaultAsync(x => x.Id.Equals(id), token);

        if (todoItem is null)
        {
            return TypedResults.NotFound(id);
        }

        dbContext.TodoItems.Remove(todoItem);
        await dbContext.SaveChangesAsync(token);

        return TypedResults.NoContent();
    }
}
