using TodoApp.Core.Entities;
using TodoApp.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace TodoApp.Core.Services;

public class TodoService
{
    private readonly ITodoRepository _repo;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ITodoRepository repo, ILogger<TodoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public void Add(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Attempted to add empty title.");
            Console.WriteLine("Error: Title cannot be empty.");
            return;
        }
        try
        {
            var item = new TodoItem { Title = title };
            _repo.Add(item);
            _logger.LogInformation("Added new to-do item: {Title} with ID {Id}", title, item.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add todo item with title: {Title}", title);
        }
    }

    public bool Update(int id, string? title, TaskStatus? taskStatus, DateTime? dueDate)
    {
        if (title is not null && string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Attempted to Update with empty title.");
            Console.WriteLine("Error: Title cannot be empty.");
            return false;
        }
        try
        {
            bool updated = _repo.Update(id, title, taskStatus, dueDate);
            if (!updated)
            {
                _logger.LogWarning("Update failed for item {Id}", id);
                return false;
            }
            _logger.LogInformation("Updated item {Id} with values: Title='{Title}', Status={Status}, DueDate={DueDate}",
            id, title, taskStatus, dueDate);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating item {Id}", id);
            return false;
        }

    }

    public bool Delete(int id)
    {
        try
        {
            var success = _repo.Delete(id);
            if (!success)
            {
                _logger.LogWarning("TodoService: Delete failed for ID {Id}", id);
                return false;
            }
            _logger.LogInformation("Deleting to-do item with ID: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting item {Id}", id);
            return false;
        }
        
    }

    public IEnumerable<TodoItem> GetByStatus(TaskStatus status)
    {
        try
        {
            var filtered = GetAll().Where(t => t.Status == status).ToList();
            _logger.LogDebug("Found {Count} items with status {Status}", filtered.Count, status);
            return filtered;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while fetching items by status {Status}", status);
            return Enumerable.Empty<TodoItem>();
        }
        
    }

    public TodoItem? GetById(int id)
    {
        try
        {
            var todos = _repo.GetAll();
            var item = todos.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                _logger.LogWarning("No to-do item found with ID {Id}", id);
            }
            else
            {
                _logger.LogInformation("Fetched task with ID {Id}", id);
            }
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while fetching item by ID {Id}", id);
            return null;
        }
    }

    public IEnumerable<TodoItem> GetAll()
    {
        try
        {
            var all = _repo.GetAll().ToList();
            _logger.LogDebug("Fetched {Count} total to-do items", all.Count);
            return all;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while fetching all to-do items");
            return Enumerable.Empty<TodoItem>();

        }
    }

}