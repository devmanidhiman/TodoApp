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
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            todos = new List<TodoItem>();
        }
        return todos;
    }

    private static int GenerateNewId(List<TodoItem> todos)
    {
        return todos.Select(t => t.Id).DefaultIfEmpty(0).Max() + 1;
    }


    public void Add(TodoItem todo)
    {
        if (!TodoValidator.IsValid(todo.Title, todo.DueDate, out string errorMessage))
        {
            _logger.LogWarning("Add(): Validation failed — {Error}", errorMessage);
            return;
        }
        var todos = LoadFromFile();
        todo.Id = GenerateNewId(todos);
        if (todos.Any(t => t.Id == todo.Id))
        {
            _logger.LogWarning("Add(): Todo item with ID {Id} already exists. Skipping add.", todo.Id);
            return;
        }
        todos.Add(todo);
        _logger.LogInformation("Add(): Adding todo item — ID: {Id}, Title: '{Title}'", todo.Id, todo.Title);
        this.todos = todos;
        Save();
        _logger.LogInformation("Add(): Successfully added todo item - ID: {Id}, Title: '{Title}'", todo.Id, todo.Title);
    }
    public bool Update(int id, string? newTitle, TaskStatus? taskStatus, DateTime? dueDate)
    {
        var todos = LoadFromFile();
        var item = todos.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            _logger.LogWarning("Update(): No todo item found with ID {Id}. Update skipped.", id);
            return false;
        }

        string effectiveTitle = newTitle ?? item.Title;
        DateTime? effectiveDueDate = dueDate ?? item.DueDate;

        if (!TodoValidator.IsValid(effectiveTitle, effectiveDueDate, out var error))
        {
            _logger.LogWarning("Update(): Validation failed for ID {Id} — {Error}", id, error);
            return false;
        }

        string oldTitle = item.Title;
        DateTime? oldDueDate = item.DueDate;
        TaskStatus? oldStatus = item.Status;

        var updatedFields = new List<string>();

        if (newTitle != null)
        {
            item.Title = newTitle;
            updatedFields.Add("Title");
        }

        if (taskStatus.HasValue)
        {
            item.Status = taskStatus.Value;
            updatedFields.Add("Status");
        }

        if (dueDate.HasValue)
        {
            item.DueDate = dueDate.Value;
            updatedFields.Add("DueDate");
        }

        _logger.LogInformation("Update(): Fields updated for ID {Id}: {Fields}", id, string.Join(", ", updatedFields));
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
            Console.WriteLine($"Error: No item found with ID {id}.");
            return false;
        }
        try
        {
            todos.Remove(item);
            Save();
            _logger.LogInformation("Delete(): Item with ID {Id} deleted.", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete(): Failed to delete item {Id}", id);
            Console.WriteLine("Error: Failed to delete item.");
        }

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
        var completed = todos.Count(t => t.Status == TaskStatus.Completed);
        var pending = todos.Count(t => t.Status == TaskStatus.Pending);
        var inprogress = todos.Count(t => t.Status == TaskStatus.InProgress);
        _logger.LogInformation("GetAll(): Retrieved {Total} items — {Completed} completed, {Pending} pending, {Inprogress} inprogress", total, completed, pending, inprogress);
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
        return [.. allItems.Where(item => item.Status == TaskStatus.Completed)];
    }

    public void CheckAll()
    {
        Console.WriteLine("Marking all incomplete items as completed...");
        _logger.LogInformation("CheckAll(): Starting to mark all incomplete Todo items as complete.");

        int updatedCount = 0;
        var incomplete = todos.Where(t => t.Status != TaskStatus.Completed).ToList();
        _logger.LogInformation("CheckAll(): Found {Count} incomplete items to update. This include pending and Inprogress tasks.", incomplete.Count);
        foreach (var item in incomplete)
        {
            item.Status = TaskStatus.Completed;
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

