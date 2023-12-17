using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Serilog;

using TestContainers101.Api.Endpoints;
using TestContainers101.Api.Infra.Extensions;
using TestContainers101.Api.Infra.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContextPool<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Db")));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddDefaultHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHsts();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapDefaultHealthChecksGroup("/_healthz");

app.MapGroup("/api/todo-items")
    .MapTodoItemEndpoints();

app.Run();

public partial class Program
{
    protected Program() { }
}
