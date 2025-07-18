using Microsoft.Extensions.Logging;
using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;
using System.Text.Json;

namespace TodoApp.Infrastructure.Repositories;
public class FileTodoRepository : ITodoRepository
{
    private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "todoitems.json"); // You can make this configurable
    private readonly ILogger<FileTodoRepository> _logger;
    private List<TodoItem> todos = new();

    public FileTodoRepository(ILogger<FileTodoRepository> logger)
    {
        _logger = logger;
        Console.WriteLine("Logger injected successfully."); // Add this temporarily
    }

    /// <summary>
    /// Saves the current list of todo items to the persistent storage file.
    /// </summary>
    /// <remarks>
    /// This method serializes the in-memory <c>todos</c> list to JSON and writes it to the file
    /// specified by <c>filePath</c>. It logs the result of the operation.
    /// </remarks>
    private void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            _logger.LogInformation("Save(): Todo items successfully saved to file.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Save(): Failed to save todo items to file.");
        }
    }


    public void Add(TodoItem todo)
    {
        var todos = LoadFromFile();
        if (todos.Any(t => t.Id == todo.Id))
        {
            _logger.LogWarning("Add(): Todo item with ID {Id} already exists. Skipping add.", todo.Id);
            return;
        }
        todos.Add(todo);
        _logger.LogInformation("Add(): Adding todo item — ID: {Id}, Title: '{Title}'", todo.Id, todo.Title);
        this.todos = todos;
        Save();
        _logger.LogInformation("Add(): Successfully added todo item - ID: {id}, Title: '{title}'", todo.Id, todo.Title);
    }
    public bool Update(TodoItem item)
    {
        var todos = LoadFromFile();
        var index = todos.FindIndex(t => t.Id == item.Id);
        if (index == -1)
        {
            Console.WriteLine($"Todo item with ID {item.Id} not found. Update failed.");
            return false;
        }
        todos[index] = item;
        Save();
        Console.WriteLine($"Updated Todo: [{item.Id}] {item.Title}");
        return true;
    }
    public bool Delete(int id)
    {
        var todos = LoadFromFile();
        var todoToDelete = todos.FirstOrDefault(t => t.Id == id);
        if (todoToDelete == null)
        {
            Console.WriteLine($"Todo item with ID {id} not found. Deletion failed.");
            return false;
        }
        todos.Remove(todoToDelete);
        Save();
        Console.WriteLine($"Deleted Todo: [{todoToDelete.Id}] {todoToDelete.Title}");
        return true;
    }
    public void ClearAll()
    {
        Console.WriteLine("ClearAll() was called.");
        if (File.Exists(filePath))
        {
            _logger.LogInformation("ClearAll(): File found at path {Path}. Starting deletion of all todo items.", filePath);
            File.WriteAllText(filePath, "[]");
            _logger.LogInformation("ClearAll(): Todo file successfully cleared.");
        }
        else
        {
            _logger.LogWarning("ClearAll(): Todo file not found at path {Path}. Nothing to clear.", filePath);
        }


    }
    public bool Exists(int id)
    {
        var items = LoadFromFile();
        return items.Any(item => item.Id == id);
    }
    public IEnumerable<TodoItem> GetAll() => LoadFromFile();
    public List<TodoItem> GetTodoItems()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Todo items file does not exist. Returning empty list.");
            return new List<TodoItem>();
        }
        var json = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(json))
        {
            Console.WriteLine("Todo items file is empty. Returning empty list.");
            return new List<TodoItem>();
        }
        return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
    }

    public TodoItem? GetTodoById(int id)
    {
        var todos = LoadFromFile();
        var todo = todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            Console.WriteLine($"Todo item with ID {id} not found.");
        }
        else
        {
            Console.WriteLine($"Found Todo: [{todo.Id}] {todo.Title}");
        }
        return todo;
    }
    public IEnumerable<TodoItem> GetCompletedItems()
    {
        var allItems = LoadFromFile();
        return [.. allItems.Where(item => item.IsCompleted)];
    }

    public void CheckAll()
    {
        Console.WriteLine("CheckAll() was called.");
        _logger.LogInformation("CheckAll(); Starting to mark all incomplete Todo items as complete.");

        int updatedCount = 0;
        foreach (var item in todos)
        {
            if (!item.IsCompleted)
            {
                item.IsCompleted = true;
                updatedCount++;
                _logger.LogInformation("CheckAll(): Marked item as completed ID: {Id}, Title: '{Title}'", item.Id, item.Title);
            }

        }

        Save();
        if (updatedCount > 0)
        {
            _logger.LogInformation("CheckAll(): Completed {Count} items.", updatedCount);
        }
        else
        {
            _logger.LogInformation("CheckAll(): No incomplete items found.");
        }

    }

    private List<TodoItem> LoadFromFile()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("JSON file not found. Returning empty list.");
            return new List<TodoItem>();
        }

        string json = File.ReadAllText(filePath); ;
        try
        {
            todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            Console.WriteLine($"Loaded {todos.Count} todo items from file.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            todos = new List<TodoItem>();
        }
        return todos;
    }
}

