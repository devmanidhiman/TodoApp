using TodoApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;
using System.ComponentModel.Design;
using System.Buffers;
using TodoApp.Core.Services;

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
        var logger = host.Services.GetRequiredService<ILogger<TodoService>>();

        var todoService = new TodoService(repo, logger);


        void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("To-Do App - Command Line Interface\n");
            Console.ResetColor();

            Console.WriteLine("Usage:");
            Console.WriteLine("  add <title>               Add a new to-do item");
            Console.WriteLine("  update <id> <title>       Update an existing item");
            Console.WriteLine("  delete <id>               Delete an item by ID");
            Console.WriteLine("  list                      List all to-do items");
            Console.WriteLine("  filter <status>           Filter items by status (completed/pending)");
            Console.WriteLine("  help                      Show this help menu\n");

            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run add \"Buy groceries\"");
            Console.WriteLine("  dotnet run update 3 \"Buy milk instead\"");
            Console.WriteLine("  dotnet run delete 2");

        }

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "add":
                if (args.Length < 2)
                {
                    Console.WriteLine("Error: Title is required for 'add' command.");
                    return;
                }
                todoService.Add(args[1]);
                break;

            case "update":
                if (args.Length < 3 || !int.TryParse(args[1], out int updateId))
                {
                    Console.WriteLine("Error: Valid ID and title are required for 'update'.");
                    return;
                }
                todoService.Update(updateId, args[2]);
                break;

            case "delete":
                if (args.Length < 2 || !int.TryParse(args[1], out int deleteId))
                {
                    Console.WriteLine("Error: Valid ID is required for 'delete'.");
                    return;
                }
                todoService.Delete(deleteId);
                break;

            case "list":
                var items = todoService.GetAll();
                foreach (var item in items)
                {
                    Console.WriteLine($"{item.Id}: {item.Title} [{(item.IsCompleted ? "✓" : " ")}]");
                }
                break;

            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowHelp();
                break;
        }

        // var validTodo = new TodoItem
        // {
        //     Title = "Finish logging refactor",
        //     DueDate = DateTime.Today.AddDays(2),
        //     IsCompleted = false
        // };

        // Console.WriteLine($"Assigned ID: {validTodo.Id}");

        // var invalidTitleTodo = new TodoItem
        // {
        //     Title = "   ", // Invalid title
        //     DueDate = DateTime.Today.AddDays(2),
        //     IsCompleted = false
        // };

        // var pastDueTodo = new TodoItem
        // {
        //     Title = "Submit PR",
        //     DueDate = DateTime.Today.AddDays(-1), // Invalid due date
        //     IsCompleted = false
        // };


        // repo.Add(validTodo);         // Should succeed and assign ID
        // repo.Add(invalidTitleTodo); // Should fail validation
        // repo.Add(pastDueTodo);      // Should fail validation

    }
}