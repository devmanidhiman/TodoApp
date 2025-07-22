using TodoApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;

class Program
{
    static void Main(string[] args)
    {
        // Create a HostBuilder to set up dependency injection and logging
        var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        { services.AddSingleton<ITodoRepository, FileTodoRepository>(); })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        }).Build();
        // Resolve the ITodoRepository from the service provider
        var repo = host.Services.GetRequiredService<ITodoRepository>();
        Console.WriteLine($"Resolved repo type: {repo.GetType().Name}");

        var validTodo = new TodoItem
        {
            Title = "Finish logging refactor",
            DueDate = DateTime.Today.AddDays(2),
            IsCompleted = false
        };

        Console.WriteLine($"Assigned ID: {validTodo.Id}");

        var invalidTitleTodo = new TodoItem
        {
            Title = "   ", // Invalid title
            DueDate = DateTime.Today.AddDays(2),
            IsCompleted = false
        };

        var pastDueTodo = new TodoItem
        {
            Title = "Submit PR",
            DueDate = DateTime.Today.AddDays(-1), // Invalid due date
            IsCompleted = false
        };


        repo.Add(validTodo);         // Should succeed and assign ID
        repo.Add(invalidTitleTodo); // Should fail validation
        repo.Add(pastDueTodo);      // Should fail validation

    }
}