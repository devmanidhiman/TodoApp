using TodoApp.Core.Interfaces;
using TodoApp.Core.Entities;
using System.Text.Json;

namespace TodoApp.Infrastructure.Repositories;
public class FileTodoRepository //: ITodoRepository
{
    private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "todoitems.json"); // You can make this configurable

    private List<TodoItem> todos = new();

    public FileTodoRepository()
    {
        Console.WriteLine("FileTodoRepository constructor called");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        Console.WriteLine($"Using file path: {filePath}");

        //LoadFromFile();
    }

    public void Add(TodoItem todo)
    {
        var todos = LoadFromFile();
        if (todos.Any(t => t.Id == todo.Id))
        {
            Console.WriteLine($"Todo item with ID {todo.Id} already exists. Skipping add.");
            return;
        }
        todos.Add(todo);
        Console.WriteLine($"Adding todo item: {todo}");
        SaveToFile(todos);
        Console.WriteLine($"Added Todo: [{todo.Id}] {todo.Title}");

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
        SaveToFile(todos);
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
        SaveToFile(todos);
        Console.WriteLine($"Deleted Todo: [{todoToDelete.Id}] {todoToDelete.Title}");
        return true;
    }
    public void ClearAll()
    {
        if (File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
            Console.WriteLine("All todo items cleared.");
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

    private void SaveToFile(List<TodoItem> todos)
    {
        string json = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
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

