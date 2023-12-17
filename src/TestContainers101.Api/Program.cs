using FluentValidation;

using Microsoft.EntityFrameworkCore;

using Serilog;

using TestContainers101.Api.Endpoints;
using TestContainers101.Api.Infra.Extensions;
using TestContainers101.Api.Infra.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Config logging
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddDbContextPool<AppDbContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("Db")))
    .AddValidatorsFromAssemblyContaining<Program>()
    .AddProblemDetails()
    .AddDefaultHealthChecks();

#if DEBUG
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
#endif

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseSerilogRequestLogging()
    .UseExceptionHandler()
    .UseHsts()
    .UseStatusCodePages();

#if DEBUG
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage()
        .UseMigrationsEndPoint();
}
#endif

// Setup endpoints
app.MapDefaultHealthChecksGroup("/_healthz")
    .MapGroup("/api/todo-items")
        .MapTodoItemEndpoints();

app.Run();

public partial class Program
{
    protected Program() { }
}
