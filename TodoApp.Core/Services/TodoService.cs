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

        var item = new TodoItem { Title = title };
        _repo.Add(item);
        _logger.LogInformation("Added item: {Title}", title);
    }

    public void Update(int id, string? title, bool? isCompleted, DateTime? dueDate)
    {
        if (title is not null && string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Attempted to Update with empty title.");
            Console.WriteLine("Error: Title cannot be empty.");
            return;
        }

        _repo.Update(id, title, isCompleted, dueDate);
        _logger.LogInformation("Updated item {Id} with values: Title='{Title}', Completed={Completed}, DueDate={DueDate}",
            id, title, isCompleted, dueDate);

    }

    public bool Delete(int id)
    {
        var success = _repo.Delete(id);
        if (!success)
        {
            _logger.LogWarning("TodoService: Delete failed for ID {Id}", id);
            return false;
        }
        _logger.LogInformation("Deleted item with ID: {Id}", id);
        return true;
    }

    public IEnumerable<TodoItem> GetAll()
    {
        return _repo.GetAll();
    }

}