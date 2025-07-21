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
    public bool Update(int id, string newTitle, bool isCompleted, DateTime dueDate)
    {
        var todos = LoadFromFile();
        var item = todos.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            _logger.LogWarning("Update(): No todo item found with ID {Id}. Update skipped.", id);
            return false;
        }
        string oldTitle = item.Title;
        DateTime? oldDueDate = item.DueDate;
        bool oldStatus = item.IsCompleted; 
        item.Title = newTitle;
        item.IsCompleted = isCompleted;
        item.DueDate = dueDate;
        _logger.LogInformation(
            "Update(): Todo item ID {Id} updated — Title: '{OldTitle}' → '{NewTitle}', Due: {OldDueDate} → {NewDueDate}, Completed: {OldStatus} → {NewStatus}",
            id,
            oldTitle,
            newTitle,
            oldDueDate?.ToString("yyyy-MM-dd"),
            dueDate.ToString("yyyy-MM-dd"),
            oldStatus,
            isCompleted);
        Save();
        return true;
    }

    /// <summary>
    /// Deletes a todo item by ID. Returns true if the item was found and deleted; false otherwise.
    /// </summary>
    public bool Delete(int id)
    {
        var todos = LoadFromFile();
        var item = todos.FirstOrDefault(t => t.Id == id);
        if (item == null)
        {
            _logger.LogWarning("Delete(): No todo item found with ID {Id}. Deletion skipped.", id);
            return false;
        }
        todos.Remove(item);
        _logger.LogInformation("Delete(): Successfully deleted todo item — ID: {Id}, Title: '{Title}'", item.Id, item.Title);
        Save();
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
    public IEnumerable<TodoItem> GetAll()
    {
        var todos = LoadFromFile();
        var total = todos.Count();
        var completed = todos.Count(t => t.IsCompleted);
        var pending = todos.Count(t => !t.IsCompleted);
        _logger.LogInformation("GetAll(): Retrieved {Total} items — {Completed} completed, {Pending} pending.", total, completed, pending);
        return todos;
    }
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
        Console.WriteLine("Marking all incomplete items as completed...");
        _logger.LogInformation("CheckAll(): Starting to mark all incomplete Todo items as complete.");

        int updatedCount = 0;
        var incomplete = todos.Where(t => !t.IsCompleted).ToList();
        _logger.LogInformation("CheckAll(): Found {Count} incomplete items to update.", incomplete.Count);
        foreach (var item in incomplete)
        {
            item.IsCompleted = true;
            updatedCount++;
            _logger.LogInformation("CheckAll(): Marked item as completed ID: {Id}, Title: '{Title}'", item.Id, item.Title);

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
    
    
}

