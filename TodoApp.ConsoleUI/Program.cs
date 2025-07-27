using TodoApp.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;
using System.ComponentModel.Design;
using System.Buffers;
using System.Linq;
using TodoApp.Core.Services;
using System.Runtime.CompilerServices;

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
        Console.WriteLine("Args: " + string.Join(" | ", args));


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
            Console.WriteLine("  complete <id>             Mark item as completed");
            Console.WriteLine("  pending <id>              Mark item as pending");
            Console.WriteLine("  filter <status>           Filter items by status (completed/pending)/inprogress");
            Console.WriteLine("  help                      Show this help menu\n");

            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run add \"Buy groceries\"");
            Console.WriteLine("  dotnet run update 3 \"Buy milk instead\"");
            Console.WriteLine("  dotnet run list");
            Console.WriteLine("  dotnet run delete 2");
            Console.WriteLine("  dotnet run complete 1");
            Console.WriteLine("  dotnet run pending 4");
            Console.WriteLine("  dotnet run filter completed");
        }

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        static void DisplayTodos(IEnumerable<TodoItem> todos)
        {
            foreach (var todo in todos)
            {
                Console.ForegroundColor = todo.Status switch
                {
                    TaskStatus.Pending => ConsoleColor.Yellow,
                    TaskStatus.InProgress => ConsoleColor.Cyan,
                    TaskStatus.Completed => ConsoleColor.Green,
                    _ => ConsoleColor.White
                };

                Console.WriteLine($"{todo.Id} {todo.Title} - {todo.Status}");
            }
            Console.ResetColor();
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
                if (args.Length < 2 || !int.TryParse(args[1], out int updateId))
                {
                    Console.WriteLine("Error: Valid ID is required for 'update'.");
                    return;
                }
                string? title = null;
                TaskStatus? status = null;
                DateTime? dueDate = null;
                for (int i = 2; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--title":
                            if (i + 1 < args.Length)
                            {
                                title = args[i + 1];
                                i++;
                            }
                            break;

                        case "--status":
                            if (i + 1 < args.Length)
                            {
                                string statusArg = args[i + 1].ToLower();
                                status = statusArg switch
                                {
                                    "completed" => TaskStatus.Completed,
                                    "pending" => TaskStatus.Pending,
                                    "inprogress" => TaskStatus.InProgress,
                                    _ => null
                                };

                                if (status == null)
                                {
                                    Console.WriteLine("Error: Invalid status. Use 'pending', 'inprogress', or 'completed'.");
                                    return;
                                }

                                i++;

                            }
                            break;

                        case "--due":
                            if (i + 1 < args.Length && DateTime.TryParse(args[i + 1], out DateTime parsedDate))
                            {
                                dueDate = parsedDate;
                                i++;
                            }
                            break;
                    }
                }
                todoService.Update(updateId, title, status, dueDate);
                break;

            case "delete":
                if (args.Length < 2 || !int.TryParse(args[1], out int deleteId))
                {
                    Console.WriteLine("Error: Valid ID is required for 'delete'.");
                    return;
                }
                Console.WriteLine("Are you sure you want to delete item {deletedId}? (y/n)");
                var repsone = Console.ReadLine();
                if (repsone?.ToLower() != "y")
                {
                    Console.WriteLine("Delete Cancelled.");
                    return;
                }
                var success = todoService.Delete(deleteId);
                if (success)
                {
                    Console.WriteLine($"Item with ID {deleteId} has been deleted.");
                }
                break;

            case "list":
                var items = todoService.GetAll();
                if (!items.Any())
                {
                    Console.WriteLine("No To-Do items found.");
                    return;
                }
                Console.WriteLine("Your To-Do list items:");
                foreach (var item in items)
                {
                    Console.ForegroundColor = item.Status == TaskStatus.Completed ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.WriteLine($"{item.Id}: {item.Title} [{(item.Status == TaskStatus.Completed ? "✔" : "⏳")}]");
                    Console.ResetColor();
                }

                Console.WriteLine($"\nTotal items: {items.Count()}");
                break;

            case "complete":
                if (args.Length < 2 || !int.TryParse(args[1], out var completeId))
                {
                    Console.WriteLine("❌ Invalid command. Usage: complete <id>\nExample: complete 3");
                    break;
                }
                todoService.Update(completeId, null, TaskStatus.Completed, null);
                Console.WriteLine($"✅ Task {completeId} marked as completed.");
                break;

            case "pending":
                if (args.Length < 2 || !int.TryParse(args[1], out var pendingId))
                {
                    Console.WriteLine("❌ Invalid command. Usage: pending <id>\nExample: pending 3");
                    break;
                }
                todoService.Update(pendingId, null, TaskStatus.Pending, null);
                Console.WriteLine($"✅ Task {pendingId} marked as pending.");
                break;

            case "inprogress":
                if (args.Length < 2 || !int.TryParse(args[1], out var inProgressId))
                {
                    Console.WriteLine("❌ Invalid command. Usage: inprogress <id>\nExample: inprogress 3");
                    break;
                }
                bool updated = todoService.Update(inProgressId, null, TaskStatus.InProgress, null);
                if (updated)
                    Console.WriteLine($"🔄 Task {inProgressId} marked as in progress.");
                else
                    Console.WriteLine($"⚠️ Task {inProgressId} not found or update failed.");
                break;


            case "filter":
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: filter <status> \nStatus must be 'pending', 'inprogress', or 'completed'");
                    break;
                }
                var argStatus = args[1].ToLower();

                if (!Enum.TryParse<TaskStatus>(argStatus, true, out var taskStatus))
                {
                    Console.WriteLine("Invalid Status. Use 'pending', 'inprogress', or 'complete'.");
                    break;
                }

                var filteredTasks = todoService.GetByStatus(taskStatus);
                DisplayTodos(filteredTasks);
                break;

            case "status":
                if (args.Length < 2 || !int.TryParse(args[1], out var statusId))
                {
                    Console.WriteLine("❌ Invalid command. Usage: status <id>\nExample: status 4");
                    break;
                }

                var task = todoService.GetById(statusId);
                if (task == null)
                {
                    Console.WriteLine($"⚠️ Task with ID {statusId} not found.");
                    break;
                }

                Console.WriteLine($"📝 Task {task.Id} — {task.Title}");
                Console.WriteLine($"Status: {task.Status}");
                Console.WriteLine($"Due: {(task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : "None")}");
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